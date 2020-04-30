using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public bool isMerge;
    public bool isDestory;

    SpriteRenderer spriteRenderer;
    Sprite[] sprites;
    int spriteCount;

    Transform tr;
    public bool isMoving;
    int targetX; // 이동 될 좌표
    int targetY; // 이동 될 좌표

    public void Init(int num)
    {
        tr = this.transform;
        spriteRenderer = this.GetComponent<SpriteRenderer>();

        sprites = Resources.LoadAll<Sprite>("Sprites/" + num);
    }

    void Update()
    {
        if (isMoving == true)
        {
            Move(targetX, targetY);
        }
        SpriteChange(2);
    }

    // [x, y] 이동 될 좌표
    public void Move(int x, int y)
    {
        isMoving = true;
        targetX = x;
        targetY = y;
        tr.position = Vector3.MoveTowards(tr.position, new Vector3(1.2f * targetX - 1.8f, 1.2f * targetY - 0.81f, 0), 0.4f);

        if (tr.position == new Vector3(1.2f * targetX - 1.8f, 1.2f * targetY - 0.81f, 0))
        {
            isMoving = false;
            if (isDestory == true)
            {
                Free();
            }
        }
    }

    public void Free()
    {
        isMoving = false;
        isDestory = false;
        ObjectPoolManager.Instance.Free(this.gameObject);
    }

    public void SpriteChange(int _num)
    {
        if (spriteCount != ObjectPoolManager.Instance.SpriteNum)
        {
            spriteCount = ObjectPoolManager.Instance.SpriteNum;
            spriteRenderer.sprite = sprites[ObjectPoolManager.Instance.SpriteNum];
        }
    }

    public void Spawn()
    {
        StartCoroutine("Cor_Spawn");
    }

    IEnumerator Cor_Spawn()
    {
        bool isSpawnComplete = false;
        tr.localScale = Vector3.zero;
        while (isSpawnComplete == false)
        {
            tr.localScale = Vector3.Lerp(tr.localScale, Vector3.one, 0.28f);
            if (tr.localScale == Vector3.one)
                isSpawnComplete = true;
            yield return null;
        }
    }

    public void Merge()
    {
        StartCoroutine("Cor_Merge");
    }

    IEnumerator Cor_Merge()
    {
        bool isMergeComplete = false;
        while (isMergeComplete == false)
        {
            tr.localScale = Vector3.Lerp(tr.localScale, new Vector3(1.3f, 1.3f, 1.3f), 0.28f);
            //tr.localScale = Mathf.SmoothDamp(tr.localScale, new Vector3(1.3f, 1.3f, 1.3f),);
            if (tr.localScale.x >= 1.25f)
            {
                isMergeComplete = true;
                tr.localScale = Vector3.one;
            }
            yield return null;
        }
    }

}
