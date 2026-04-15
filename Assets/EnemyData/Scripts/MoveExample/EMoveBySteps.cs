using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Step
{
    public float direction;
    public float distance;

    public float speed = 3f;
    public float waitTime = 0f;  // 到达后停留时间（用于驻点射击）
}

[CreateAssetMenu(fileName = "MoveBySteps", menuName = "STG/Move Pattern/By Steps")]
public class EMoveBySteps : EnemyMotion
{

    [SerializeField] private List<Step> steps = new();
    public override IEnumerator Move(EMContext context)
    {
        foreach (var step in steps)
        {
            Vector3 position = context.self.position + new Vector3(Mathf.Cos(step.direction * Mathf.Deg2Rad), Mathf.Sin(step.direction * Mathf.Deg2Rad), 0) * step.distance;
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
