using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Button RankDoneBtn;
    public Button RankBtn;
    public Button NewGameBtn;
    public Button RestartBtn;
    public Text ScoreText;
    public Text BestScoreText;
    public Text PlusText;
    public GameObject RankUI;
    public GameObject GameOverUI;
    public firebaseDB firebase;

    GameObject[,] square = new GameObject[4, 4];
    int score;
    int bestScore;
    int plusScore;
    int adjoinCount; // 인접한 블럭에 같은 숫자 갯수
    bool isDragging;
    bool isMoving;
    bool isEmptySqure;  // 빈공간 확인
    bool isGameOver;
    bool isShowRankUI;
    Vector3 firstPos;
    Vector3 dragDirection;

    void Start()
    {
        DBManager.Instance.Load();
        bestScore = DBManager.Instance.ItemList.score;
        BestScoreText.text = bestScore.ToString();

        RankDoneBtn.onClick.AddListener(() => { RankDone(); });
        RankBtn.onClick.AddListener(() => { ShowRankUI(); });
        NewGameBtn.onClick.AddListener(() => { Restart(); });
        RestartBtn.onClick.AddListener(() => { Restart(); });

        // 광고 초기화
        AdmobBanner.Instance.Init();
        AdmobReward.Instance.Init();

        Spawn();
        Spawn();


        //DB 초기화 (Score)
        //DBManager.Instance.UpdateItemTable(0);

    }

    // Update is called once per frame
    void Update()
    {
        // 뒤로가기
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (isGameOver == true || isShowRankUI == true)
            return;

        // Input (mouse & touch)
        if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            isDragging = true;
            firstPos = Input.GetMouseButtonDown(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position;
        }
        if (Input.GetMouseButton(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            dragDirection = (Input.GetMouseButton(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position) - firstPos;
            if (dragDirection.magnitude < 100)
                return;
            dragDirection.Normalize();

            if (isDragging == true)
            {
                isDragging = false;

                // Up
                if (dragDirection.y > 0 && dragDirection.x > -0.5f && dragDirection.x < 0.5f)
                {
                    for (int x = 0; x <= 3; x++)
                    {
                        for (int y = 0; y <= 2; y++)
                        {
                            for (int i = 3; i > y; i--)
                            {
                                MoveOrMerge(x, i - 1, x, i);
                            }
                        }
                    }
                }
                // Down
                else if (dragDirection.y < 0 && dragDirection.x > -0.5f && dragDirection.x < 0.5f)
                {
                    for (int x = 0; x <= 3; x++)
                    {
                        for (int y = 3; y >= 1; y--)
                        {
                            for (int i = 0; i < y; i++)
                            {
                                MoveOrMerge(x, i + 1, x, i);
                            }
                        }
                    }
                }
                // Right
                else if (dragDirection.x > 0 && dragDirection.y > -0.5f && dragDirection.y < 0.5f)
                {
                    for (int y = 0; y <= 3; y++)
                    {
                        for (int x = 0; x <= 2; x++)
                        {
                            for (int i = 3; i > x; i--)
                            {
                                MoveOrMerge(i - 1, y, i, y);
                            }
                        }
                    }
                }
                // Left
                else if (dragDirection.x < 0 && dragDirection.y > -0.5f && dragDirection.y < 0.5f)
                {
                    for (int y = 0; y <= 3; y++)
                    {
                        for (int x = 3; x >= 1; x--)
                        {
                            for (int i = 0; i <= x - 1; i++)
                                MoveOrMerge(i + 1, y, i, y);
                        }
                    }
                }
                else
                    return;

                if (isMoving == true)
                {
                    isMoving = false;
                    isEmptySqure = false;
                    adjoinCount = 0;
                    Spawn();
                    Score();

                    for (int x = 0; x <= 3; x++)
                    {
                        for (int y = 0; y <= 3; y++)
                        {
                            if (square[x, y] != null)
                            {
                                if (square[x, y].GetComponent<Square>().isMerge == true)
                                    square[x, y].GetComponent<Square>().isMerge = false;
                            }
                            else
                            {
                                // 비어있는 블럭이 있으면 true
                                isEmptySqure = true;
                            }
                        }
                    }

                    if (isEmptySqure == false)
                    {
                        // 가로, 세로 인접한 블럭이 같은 숫자이면 adjoinCount++
                        for (int y = 0; y <= 3; y++)
                        {
                            for (int x = 0; x <= 2; x++)
                            {
                                if (square[x, y].name == square[x + 1, y].name)
                                    adjoinCount++;
                            }
                        }
                        for (int x = 0; x <= 3; x++)
                        {
                            for (int y = 0; y <= 2; y++)
                            {
                                if (square[x, y].name == square[x, y + 1].name)
                                    adjoinCount++;
                            }
                        }

                        // 인접한 블럭이 하나도 없으면 게임오버
                        if (adjoinCount == 0)
                        {
                            isGameOver = true;
                            StartCoroutine("Cor_GameOver");
                        }
                    }
                }
            }
        }
    }

    // [x1, y1] 이동 전 좌표, [x2, y2] 이동 될 좌표
    public void MoveOrMerge(int x1, int y1, int x2, int y2)
    {
        // 이동 (이동 될 좌표는 비어있고, 이동 전 좌표가 존재하면)
        if (square[x2, y2] == null && square[x1, y1] != null)
        {
            isMoving = true;
            square[x1, y1].GetComponent<Square>().Move(x2, y2);
            square[x2, y2] = square[x1, y1];
            square[x1, y1] = null;
        }

        // 결합 (둘다 같은 숫자이면)
        if (square[x1, y1] != null && square[x2, y2] != null && square[x1, y1].name == square[x2, y2].name
            && square[x1, y1].GetComponent<Square>().isMerge == false && square[x2, y2].GetComponent<Square>().isMerge == false)
        {
            isMoving = true;
            int equalNum = 0;
            for (int i = 0; i <= 16; i++)
            {
                // i : 블럭(2, 4, 8, 16, ...)
                if (square[x2, y2].name == ObjectPoolManager.Instance.poolList[i].name)
                {
                    equalNum = i;
                    int a = (int)Mathf.Pow(2, equalNum + 2);
                    break;
                }
            }
            square[x1, y1].GetComponent<Square>().isDestory = true;
            square[x1, y1].GetComponent<Square>().Move(x2, y2);
            square[x1, y1] = null;

            square[x2, y2].GetComponent<Square>().Free();

            square[x2, y2] = ObjectPoolManager.Instance.Get(equalNum + 1);
            square[x2, y2].transform.position = new Vector3(1.2f * x2 - 1.8f, 1.2f * y2 - 0.81f, 0);
            square[x2, y2].GetComponent<Square>().isMerge = true;
            square[x2, y2].GetComponent<Square>().Merge();
            //square[x2, y2].GetComponent<Animator>().SetTrigger("Merge");

            plusScore += (int)Mathf.Pow(2, equalNum + 2);
        }
    }

    public void Spawn()
    {
        int x = 0;
        int y = 0;
        while (isGameOver == false)
        {
            x = Random.Range(0, 4);
            y = Random.Range(0, 4);
            if (square[x, y] == null)
                break;
        }
        if (isGameOver == false)
        {
            square[x, y] = Random.Range(0, 8) > 1 ? ObjectPoolManager.Instance.Get(0) : ObjectPoolManager.Instance.Get(1);
            square[x, y].transform.position = new Vector3(1.2f * x - 1.8f, 1.2f * y - 0.81f, 0);
            square[x, y].GetComponent<Square>().Spawn();
            //square[x, y].GetComponent<Animator>().SetTrigger("Spawn");
        }
    }

    public void Score()
    {
        if (plusScore > 0)
        {
            PlusText.text = "+" + plusScore;
            PlusText.GetComponent<Animator>().SetTrigger("PlusBack");
            PlusText.GetComponent<Animator>().SetTrigger("Plus");
            score += plusScore;
            ScoreText.text = score.ToString();
            if (score > bestScore)
            {
                bestScore = score;
                BestScoreText.text = bestScore.ToString();
            }
            plusScore = 0;
        }
    }

    public void RankDone()
    {
        isShowRankUI = false;
        RankUI.SetActive(false);
    }

    public void ShowRankUI()
    {
        isShowRankUI = true;
        firebase.ShowRankList();
        RankUI.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene("puzzle2048");
    }

    IEnumerator Cor_GameOver()
    {
        firebase.ScoreCheck(score);
        yield return new WaitForSeconds(1);
        DBManager.Instance.UpdateItemTable(bestScore);
        GameOverUI.SetActive(true);

        AdmobReward.Instance.ShowRewardAd();
    }
}
