using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum MovePattern
{
    WayPoint,
    Relative,
    Random
}

[System.Serializable]
public class MoveSteps
{
    public MovePattern pattern;

    [Header("路径点用")]
    public Vector2 position;

    [Header("相对运动用")]
    public float direction;
    public float distance;

    [Header("随机用范围限制 (以初始位置为中心)")]
    public Vector2 range = new Vector2(5f, 5f);
    public bool isAlways = false;

    [Header("通用")]
    public float speed = 3f;
    public float waitTime = 0f;  // 到达后停留时间（用于驻点射击）

}

[CreateAssetMenu(fileName = "MovePattern", menuName = "STG/Move Pattern/General Pattern")]
[System.Serializable]
public class EMovePattern : EnemyMotion
{
    [SerializeField] private List<MoveSteps> moveSteps = new();

    public override IEnumerator Move(EMContext context)
    {
        for (int i = 0; i < moveSteps.Count; i++)
        {
            var step = moveSteps[i];
            Vector3 position = new();
            switch (step.pattern)
            {
                case MovePattern.Relative:
                    position = context.self.position + new Vector3(Mathf.Cos(step.direction * Mathf.Deg2Rad), Mathf.Sin(step.direction * Mathf.Deg2Rad), 0) * step.distance;
                    break;
                case MovePattern.WayPoint:
                    position = step.position;
                    break;
                case MovePattern.Random:
                    Vector3 randomOffset = new Vector3(
                        Random.Range(-step.range.x, step.range.x),
                        Random.Range(-step.range.y, step.range.y),
                        0f
                    );
                    position = context.self.position + randomOffset;
                    position.x = Mathf.Clamp(position.x, -3.0f, 3.0f);
                    position.y = Mathf.Clamp(position.y, 1.5f, 3.5f);
                    if (step.isAlways)
                    {
                        i--;
                    }
                    break;
            }

            while (Vector2.Distance(context.self.position, position) > 0.05f)
            {
                context.self.position = Vector2.MoveTowards(
                    context.self.position, position, step.speed * Time.deltaTime
                );
                yield return null;
            }
            if (context.spawner != null)
                context.spawner.NextMoveStep();
            if (step.waitTime > 0f)
                yield return new WaitForSeconds(step.waitTime);
        }
        yield break;
    }
}
