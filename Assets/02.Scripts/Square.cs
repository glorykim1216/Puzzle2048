using UnityEngine;

public class Square : MonoBehaviour
{
    public bool isMerge;
    public bool isDestory;

    Transform tr;
    public bool isMoving;
    int targetX; // 이동 될 좌표
    int targetY; // 이동 될 좌표

    void Start()
    {
        tr = this.transform;
    }

    void Update()
    {
        if (isMoving == true)
        {
            Move(targetX, targetY);
        }
    }

    // [x, y] 이동 될 좌표
    public void Move(int x, int y)
    {
        isMoving = true;
        targetX = x;
        targetY = y;
        tr.position = Vector3.MoveTowards(tr.position, new Vector3(1.2f * targetX - 1.8f, 1.2f * targetY - 0.81f, 0), 0.3f);

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
}
