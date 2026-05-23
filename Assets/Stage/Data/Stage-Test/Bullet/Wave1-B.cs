using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Wave1-B", menuName = "STG/Bullet Spawn/Extra Stage/1-B")]
[System.Serializable]
public class Wave1B : EnemyBulletSpawner
{
    public static float firetimes = 0f;

    private IEnumerator generate(EBContext context)
    {
        EBulletBatch batch = new EBulletBatch();
        //float angle = GetAngleToPlayer(context.bullet.position);

        batch.AddBullet(
            GetBullet()
            .SetPosition(context.bullet.position)
            .SetAccelarate(new Vector2(0f, -2.0f))
            .SetSpeed(0.8f, context.bullet.GetReflectAngle() + UnityEngine.Random.Range(90f, -90f))
        );

        batch.Packed("Small-1", new Vector3(0, 0, 255f)).Active();
        yield break;
    }

    bool check(in EBContext context)
    {
        return context.bullet.state == EBulletState.ReachBound
          && context.bullet.boundType != EBBoundType.Top
          && context.bullet.boundType != EBBoundType.Bottom;
    }

    public IEnumerator Fire(ESContext context)
    {
        Vector3 pos = context.self.position;
        for (int i = 0; i < 8; i++)
        {
            EBulletBatch batch = new EBulletBatch();
            batch.AddBullet(
                GetBullet()
                .SetPosition(pos)
                .SetSpeed(2.8f, 0f)
            ).AddBullet(
                GetBullet()
                .SetPosition(pos)
                .SetSpeed(2.8f, 180f)
            );

            batch.AttachCallBack(check, (context) => generate(context));
            batch.Packed("Medium", new Vector3(0f, 0f, 255f)).Active();

            yield return new WaitForSeconds(0.08f);
        }
        //yield return new WaitForSeconds(UnityEngine.Random.Range(1, 9) * 0.2f);
    }

    public IEnumerator BulletSpawn(ESContext context)
    {
        firetimes += 0.2f;
        if (firetimes > 2f) firetimes = 0.2f;
        yield return new WaitForSeconds(firetimes);
        //yield return new WaitForSeconds(0.5f);
        StartCoroutine(Fire(context));
    }

    public override List<Func<ESContext, IEnumerator>> SpawnerList()
    {
        return new List<Func<ESContext, IEnumerator>>
        {
          (context) => BulletSpawn(context)
        };
    }

}
