using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "5-waysAiming", menuName = "STG/Bullet Spawn/5-ways Aiming")]
[System.Serializable]
public class AimingSpread : EnemyBulletSpawner
{
    [Header("散弹的条数 (奇数必有一发正对玩家)")]
    public int spreadLines = 5;
    [Header("扇形总角度")]
    public float spreadAngle = 60f;

    public override IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitForSeconds(1.5f);
        float speed = 3.5f;
        Vector3 firePos = context.self.position;
        float baseAngle = EBulletManager.GetAngleToPlayer(firePos);

        for (int n = 0; n < 50; n++)
        {
            float finalSpeed = speed + (float)n * 0.30f;
            float startAngle = baseAngle - (spreadAngle / 2f);
            float angleStep = spreadLines > 1 ? spreadAngle / (spreadLines - 1) : 0f;
            EBulletBatch batch = new EBulletBatch();
            for (int i = 0; i < spreadLines; i++)
            {
                float currentAngle = startAngle + (angleStep * i);
                batch.AddBullet(
                    firePos, finalSpeed, currentAngle);
            }
            EBulletManager.Instance.SpawnBullet(batch.Packed(1, new Vector3(255f, 10f, 10f)));

            yield return new WaitForSeconds(0.015f);
        }
        yield return new WaitForSeconds(1f);
    }
}

