using UnityEngine;
using System.Collections;

public class RandomMover : MonoBehaviour
{
    [Header("移动设置")]
    public float minWaitTime = 1.0f;
    public float maxWaitTime = 3.0f;
    public float moveSpeed = 2.0f;

    [Header("范围限制 (以初始位置为中心)")]
    public Vector2 range = new Vector2(5f, 5f);
    
    private Vector2 originPosition;

    void Start()
    {
        originPosition = transform.position;
        // 启动协程
        StartCoroutine(RandomMoveRoutine());
    }

    IEnumerator RandomMoveRoutine()
    {
        while (true) // 无限循环
        {
            // 1. 等待随机时间
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // 2. 计算随机目标点 (在限制范围内)
            Vector2 randomOffset = new Vector2(
                Random.Range(-range.x, range.x),
                Random.Range(-range.y, range.y)
            );
            Vector2 targetPos = originPosition + randomOffset;

            // 3. 执行移动过程 (平滑移动到目标点)
            yield return StartCoroutine(MoveToPosition(targetPos));
        }
    }

    IEnumerator MoveToPosition(Vector2 target)
    {
        // 直到距离足够近为止
        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position, 
                target, 
                moveSpeed * Time.deltaTime
            );
            yield return null; // 等待下一帧
        }
    }
}
