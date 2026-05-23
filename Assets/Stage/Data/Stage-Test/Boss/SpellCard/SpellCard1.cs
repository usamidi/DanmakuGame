using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Spell Card1", menuName = "STG/Bullet Spawn/Extra Stage/Spell Card1")]
[System.Serializable]
public class SpellCard1 : EnemyBulletSpawner
{
    [Header("散弹的条数 (奇数必有一发正对玩家)")]
    public int spreadLines = 48;
    [Header("扇形总角度")]
    public float spreadAngle = 360f;
    [Header("发射波数")]
    public int wave = 12;
    [Header("减速参数")]
    public float param = 1.8f;
    [Header("颜色")]
    public Vector3 color;

    private IEnumerator func(EBContext context)
    {
        context.bullet.state = EBulletState.Dying;
        EBulletBatch batch = new EBulletBatch();
        for (int i = 0; i < 2; i++)
        {
            batch.AddBullet(
              GetBullet()
                .SetPosition(context.bullet.position)
                .SetSpeed(1.0f, UnityEngine.Random.Range(0f, 360f))
                .SetAccelarate(new Vector2(0f, -0.6f))
              );
        }

        batch.Packed("Rice", new Vector3(0f, 0f, 204f)).Active();
        yield break;
    }

    bool check(in EBContext context)
    {
        return context.bullet.timer >= 2.5f;
        //return context.bullet.speed <= 0.1f;
    }

    public IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        while (true)
        {
            float speed = 2f;
            Vector3 playerPos = context.player.position;
            Vector3 firePos = new Vector3();
            firePos.y = 3.8f;
            float baseAngle = 270f - 15f;
            for (int n = 0; n < wave; n++)
            {
                EBulletBatch batch = new EBulletBatch();
                for (int i = 0; i < 4; i++)
                {
                    float currentAngle = baseAngle + n * 3f;
                    //float baseAngle = GetAngleToPosition(firePos, playerPos);
                    firePos.x = UnityEngine.Random.Range(-3.8f, 3.8f);
                    batch.AddBullet(
                        GetBullet()
                        .SetPosition(firePos)
                        .SetSpeedRate(-2.0f)
                        .SetSpeed(speed + UnityEngine.Random.Range(-0.8f, 0.8f), currentAngle)
                    );
                }
                batch.AttachCallBack(check, (context) => func(context));

                batch.Packed("Big", new Vector3(10f, 10f, 10f)).Active();

                yield return new WaitForSeconds(0.05f);
            }

            Vector3 laserPos = new Vector3();
            for (int i = 0; i < 4; i++)
            {
                laserPos.y = 3.8f;
                laserPos.x = -3.8f + (7.4f / 3f) * i + UnityEngine.Random.Range(-0.2f, 0.2f);
                GetLaser()
                  .SetInstant()
                  .SetPosition(laserPos)
                  .SetSpeed(3f, 270f + UnityEngine.Random.Range(-8f, 8f))
                  .SetArea(4f, 0.3f)
                  .SetAppearance("Rice", new Vector3(0f, 0f, 204f))
                  .Active();

                laserPos.y = 4.8f;
                laserPos.x = -3.8f + (7.4f / 3f) * i + UnityEngine.Random.Range(-0.2f, 0.2f);
                GetLaser()
                  .SetWarning()
                  .SetPosition(laserPos)
                  .SetSpeed(0, 270f + UnityEngine.Random.Range(-3f, 3f))
                  .SetArea(12f, 0.3f)
                  .SetDuration(3f)
                  .SetAppearance("Rice", new Vector3(0f, 0f, 255f))
                  .Active();
            }
            yield return new WaitForSeconds(3f);
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
