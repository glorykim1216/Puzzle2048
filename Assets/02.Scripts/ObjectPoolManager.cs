// Rock 생성 시 프레임드랍을 최소화 하기 위해 사용

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    public GameObject source;
    public int maxAmount;       // 풀의 오브젝트 갯수
    public GameObject folder;   // Hierarchy에서 구별을 위한 빈오브젝트

    public List<GameObject> unusedList = new List<GameObject>();    // 미사용 오브젝트 리스트
}

public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
    public int defaultAmount = 0;
    public GameObject[] poolList = new GameObject[17];    // 오브젝트 리스트
    public int[] poolAmount;        // 갯수
    public List<ObjectPool> objectPoolList = new List<ObjectPool>();
    public int SpriteNum;
    public override void Init()
    {
    }
    private void Awake()
    {
        // Prefabs 로드
        for (int i = 0; i < 17; i++)
        {
            poolList[i] = Resources.Load<GameObject>("Prefabs/n_" + i);
        }
        InitObjectPool();
        StartCoroutine("Cor_ChangeSpriteNum");
    }

    // 오브젝트를 불러옴
    public GameObject Get(int _num)
    {
        if (objectPoolList.Count < _num)
        {
            Debug.LogError("ObjectPoolManager Can't Find ObjectPool - " + _num);
        }

        ObjectPool pool = objectPoolList[_num];

        GameObject obj = null;

        if (pool.unusedList.Count > 0)
        {
            obj = pool.unusedList[0];
            pool.unusedList.RemoveAt(0);
            obj.SetActive(true);
        }
        else    // 사용 가능한 오브젝트가 없을떄
        {
            obj = Instantiate(pool.source);
            obj.GetComponent<Square>().Init((int)Mathf.Pow(2, _num + 1));
            obj.transform.parent = pool.folder.transform;
            obj.name = pool.folder.name;
        }

        return obj;
    }

    // 해제
    public void Free(GameObject _obj)
    {
        string[] sp = _obj.transform.name.Split('_');
        int _num = int.Parse(sp[1]);

        if (objectPoolList.Count < _num)
        {
            Debug.LogError("ObjectPoolManager Can't Find ObjectPool - " + _num);
        }

        ObjectPool pool = objectPoolList[_num];
        pool.unusedList.Add(_obj);
        _obj.SetActive(false);
    }

    // 풀 생성
    void InitObjectPool()
    {
        for (int i = 0; i < poolList.Length; i++)
        {
            ObjectPool objectPool = new ObjectPool();
            objectPool.source = poolList[i];
            objectPoolList.Add(objectPool);

            // Hierarchy에 추가
            GameObject folder = new GameObject();
            folder.name = poolList[i].name;
            folder.transform.parent = this.transform;
            objectPool.folder = folder;

            int amount = defaultAmount;
            if (poolAmount[i] > amount && poolAmount[i] != 0)
                amount = poolAmount[i];

            for (int j = 0; j < amount; j++)
            {
                GameObject go = Instantiate(objectPool.source);
                // 2의 n승
                go.GetComponent<Square>().Init((int)Mathf.Pow(2, i + 1));
                go.SetActive(false);
                go.transform.parent = folder.transform;
                go.name = folder.name;
                objectPool.unusedList.Add(go);

                // 한번에 풀을 생성할때 부하를 줄이기 위해서 코루틴 사용
                // yield return new WaitForEndOfFrame();
            }

            objectPool.maxAmount = amount;
        }
    }
    IEnumerator Cor_ChangeSpriteNum()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.03f);

            if (SpriteNum < 49)
                SpriteNum++;
            else
                SpriteNum = 0;
        }
    }
}
