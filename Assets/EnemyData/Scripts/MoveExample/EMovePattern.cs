using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum MovePattern
{
    WayPoint,
    Relative
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

        foreach (var step in moveSteps)
        {

            Vector3 position = new();
            switch (step.pattern)
            {
                case MovePattern.Relative:
                    position = context.self.position + new Vector3(Mathf.Cos(step.direction * Mathf.Deg2Rad), Mathf.Sin(step.direction * Mathf.Deg2Rad), 0) * step.distance;
                    break;
                case MovePattern.WayPoint:
                    position = step.position;
                    break;
            }

            while (Vector2.Distance(context.self.position, position) > 0.05f)
            {
                context.self.position = Vector2.MoveTowards(
                    context.self.position, position, step.speed * Time.deltaTime
                );
                yield return null;
            }
            if (step.waitTime > 0f)
                yield return new WaitForSeconds(step.waitTime);
        }
        yield break;
    }
}
