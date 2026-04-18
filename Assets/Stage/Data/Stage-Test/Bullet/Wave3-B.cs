using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Wave3-B", menuName = "STG/Bullet Spawn/Wave/3-B")]
[System.Serializable]
public class Wave3B : EnemyBulletSpawner
{
    public int bulletCount = 3;
    public float range = 0.8f;

    private IEnumerator generate(EBContext context)
    {
        EBulletBatch batch = new EBulletBatch();
        float angle = EBulletManager.GetAngleToPlayer(context.bullet.position);
        batch.AddBullet(context.bullet.position, 3.2f, angle);
        EBulletManager.Instance.SpawnBullet(batch.Packed("Amulet", new Vector3(255f, 0, 0)));
        yield break;
    }

    bool check(in EBContext context)
    {
        return context.bullet.state == EBulletState.ReachBound && context.bullet.boundType == EBBoundType.Top;
    }


    public override IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitForSeconds(2f);


        float angleStep = 5f;
        float offsetAng = 0f;
        int param = 0;
        while (true)
        {
            EBulletBatch batch = new EBulletBatch();
            for (int i = 0; i < bulletCount; i++)
            {
                float finalAngle = angleStep * param + offsetAng;
                float radians = finalAngle * Mathf.Deg2Rad;
                Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0).normalized * range;

                batch.AddBullet(context.self.position + offsetPos, 2.5f, 15f * param
                    ).AddBullet(context.self.position - offsetPos, 2.5f, 15f * param);
            }
            batch.AttachCallBack(check, (context) => generate(context));

            EBulletManager.Instance.SpawnBullet(
                batch.Packed("Amulet", new Vector3(204f, 52f, 0f)));

            yield return new WaitForSeconds(0.03f);
            param++;
        }

    }

}
