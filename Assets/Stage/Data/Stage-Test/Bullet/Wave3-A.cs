using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Wave3-A", menuName = "STG/Bullet Spawn/Wave/3-A")]
[System.Serializable]
public class Wave3A : EnemyBulletSpawner
{
    public override IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitForSeconds(2f);
        int bulletCount = 9; // 一圈 36 发 (每 10 度一发)

        float angleStep = 360f / bulletCount;

        float offsetAng = 0f;
        while (true)
        {
            offsetAng += 15f;
            for (int k = 0; k < 4; k++)
            {
                EBulletBatch batch = new EBulletBatch();
                for (int i = 0; i < bulletCount; i++)
                {
                    float finalAngle = angleStep * i + offsetAng;
                    float radians = finalAngle * Mathf.Deg2Rad;
                    Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0).normalized * 0.1f;

                    batch.AddBullet(context.self.position + offsetPos, 5.5f, finalAngle);
                }

                EBulletManager.Instance.SpawnBullet(
                    batch.Packed("Rice", new Vector3(0f, 0f, 255f)));

                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(0.03f);
        }

    }

}
