using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Wave3-B", menuName = "STG/Bullet Spawn/Extra Stage/3-B")]
[System.Serializable]
public class Wave3B : EnemyBulletSpawner
{
    public int bulletCount = 3;
    public float range = 0.8f;

    private IEnumerator generate(EBContext context)
    {
        context.bullet.state = EBulletState.Dying;
        EBulletBatch batch = new EBulletBatch();
        //float angle = GetAngleToPlayer(context.bullet.position);
        float angle = 270f;

        batch.AddBullet(
            GetBullet()
            .SetPosition(context.bullet.position)
            .SetSpeed(1.0f, angle)
            .SetSpeedRate(1.5f)
            .SetSpinRate(270f)
            );

        Vector3[] colors = new Vector3[]
        {
            new Vector3(255f, 0, 0),
            new Vector3(255f, 255f, 0),
            new Vector3(0, 255f, 255f),
            new Vector3(0, 0, 255f)
        };
        batch.Packed("StarSmall", colors[UnityEngine.Random.Range(0, colors.Length)]).Active();
        yield break;
    }

    bool check(in EBContext context)
    {
        return context.bullet.state == EBulletState.ReachBound && context.bullet.boundType == EBBoundType.Top;
        //return context.bullet.isGrazed;
    }


    public IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);

        float angleStep = 5f;
        float offsetAng = 0f;
        int param = 0;
        while (true)
        {
            EBulletBatch batch = new EBulletBatch();
            for (int i = 0; i < bulletCount; i++)
            {
                float finalAngle = angleStep * param * (i + 1) + offsetAng;
                float radians = finalAngle * Mathf.Deg2Rad;
                Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0).normalized * range * Mathf.Cos(radians);

                batch.AddBullet(
                    GetBullet()
                    .SetPosition(context.self.position + offsetPos)
                    .SetSpeed(2.5f, 15f * param)
                    ).AddBullet(
                    GetBullet()
                    .SetPosition(context.self.position - offsetPos)
                    .SetSpeed(2.5f, 15f * param)
                );
            }
            batch.AttachCallBack(check, (context) => generate(context));

            batch.Packed("Amulet", new Vector3(204f, 52f, 0f)).Active();

            yield return new WaitForSeconds(0.03f);
            param++;
        }

    }

    public override List<Func<ESContext, IEnumerator>> SpawnerList()
    {
        return new List<Func<ESContext, IEnumerator>>
        {
          (context) => BulletSpawn(context)
        };
    }

}
