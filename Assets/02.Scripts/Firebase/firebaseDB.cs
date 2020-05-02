using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;

public class firebaseDB : MonoBehaviour
{
    public GameObject RegistrationUI;
    public GameObject LoadingUI;

    public InputField nameInputField;
    public Text scoreText;
    public Button OKBtn;

    public Text[] RankNameTxt;
    public Text[] RankScoreTxt;

    public string[] rankName = new string[10];
    public string[] rankScore = new string[10];

    int lastPlace;
    int score;
    string userName;

    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://puzzle2048-56b51.firebaseio.com/");

        OKBtn.onClick.AddListener(() => { OkBtn(); });
        ShowRankList();
        //FirebaseEventListner();
    }

    public void ScoreCheck(int _score)
    {
        FirebaseDatabase.DefaultInstance.GetReference("top10scores").OrderByChild("score").LimitToFirst(1)
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // Handle the error... 
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    foreach (var item in snapshot.Children)
                    {
                        lastPlace = System.Convert.ToInt32(item.Child("score").Value);

                    }
                    // Do something with snapshot... 
                }
            });

        if (_score > lastPlace)
        {
            score = _score;
            ShowRegistration();
        }
    }

    void ShowRegistration()
    {
        scoreText.text = score.ToString();
        RegistrationUI.SetActive(true);
    }

    void OkBtn()
    {
        userName = nameInputField.text;
        OnClickMaxScores();
        RegistrationUI.SetActive(false);
    }

    public void ShowRankList()
    {
        StartCoroutine("Cor_RankList");
    }

    IEnumerator Cor_RankList()
    {
        LoadingUI.SetActive(true);

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://puzzle2048-56b51.firebaseio.com/");

        FirebaseDatabase.DefaultInstance.GetReference("top10scores").OrderByChild("score").LimitToFirst(10)
    .GetValueAsync().ContinueWith(task =>
    {
        if (task.IsFaulted)
        {
            // Handle the error... 
        }
        else if (task.IsCompleted)
        {
            int i = 9;
            DataSnapshot snapshot = task.Result;
            foreach (var item in snapshot.Children)
            {
                rankName[i] = item.Child("name").Value.ToString();
                rankScore[i] = item.Child("score").Value.ToString();
                i--;
            }
            // Do something with snapshot... 
        }
    });

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < 10; i++)
        {
            RankNameTxt[i].text = rankName[i];
            RankScoreTxt[i].text = rankScore[i];
        }

        LoadingUI.SetActive(false);
    }

    public void OnClickMaxScores()
    {
        const int MaxScoreRecordCount = 10;

        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        mDatabaseRef.Child("top10scores").RunTransaction(mutableData =>
        {
            List<object> leaders = mutableData.Value as List<object>;


            if (leaders == null)
            {
                leaders = new List<object>();
            }
            else if (mutableData.ChildrenCount >= MaxScoreRecordCount)
            {
                long minScore = long.MaxValue;
                object minVal = null;
                foreach (var child in leaders)
                {
                    if (!(child is Dictionary<string, object>))
                        continue;
                    long childScore = (long)((Dictionary<string, object>)child)["score"];
                    if (childScore < minScore)
                    {
                        minScore = childScore;
                        minVal = child;
                    }
                }
                if (minScore > score)
                {
                    // The new score is lower than the existing 5 scores, abort. 
                    return TransactionResult.Abort();
                }


                // Remove the lowest score. 
                leaders.Remove(minVal);
            }


            Dictionary<string, object> entryValues = new Dictionary<string, object>();
            entryValues.Add("score", score);
            entryValues.Add("name", userName);
            leaders.Add(entryValues);

            mutableData.Value = leaders;
            return TransactionResult.Success(mutableData);
        }).ContinueWith(
            task =>
            {
                Debug.Log(string.Format("OnClickMaxScores::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            }
        );
    }


    //public void FirebaseEventListner()
    //{

    //    DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("top10scores");

    //    reference.ValueChanged += HandleValueChanged;
    //}

    ////users 데이터 전체의 변화에 수신
    //void HandleValueChanged(object sender, ValueChangedEventArgs args)
    //{
    //    if (args.DatabaseError != null)
    //    {
    //        Debug.LogError(args.DatabaseError.Message);
    //        return;
    //    }
    //    SnapshotAllDataRead(args.Snapshot);
    //}

    //public void SnapshotAllDataRead(DataSnapshot Snapshot)
    //{
    //    foreach (var item in Snapshot.Children)
    //    {
    //        Debug.Log(item.Child("name").Value + " : " + item.Child("score").Value);
    //    }
    //}


}
