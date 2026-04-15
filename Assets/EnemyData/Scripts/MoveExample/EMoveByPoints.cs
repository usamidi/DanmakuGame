using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint
{
    public Vector2 position;
    public float speed = 3f;
    public float waitTime = 0f;  // 到达后停留时间（用于驻点射击）
}


[CreateAssetMenu(fileName = "MoveByPoints", menuName = "STG/Move Pattern/By Points")]
[System.Serializable]
public class EMoveByPoints : EnemyMotion
{
    [SerializeField] private List<Waypoint> waypoints = new();

    public override IEnumerator Move(EMContext context)
    {
        foreach (var wp in waypoints)
        {
            while (Vector2.Distance(context.self.position, wp.position) > 0.05f)
            {
                context.self.position = Vector2.MoveTowards(
                    context.self.position, wp.position, wp.speed * Time.deltaTime
                );
                yield return null;
            }
            if (wp.waitTime > 0f)
                yield return new WaitForSeconds(wp.waitTime);
        }
        yield break;
    }
}
