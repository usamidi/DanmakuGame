using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Spell Card9", menuName = "STG/Bullet Spawn/Extra Stage/Spell Card9")]
[System.Serializable]
public class SpellCard9 : EnemyBulletSpawner
{
    [Header("扇形总角度")]
    public float spreadAngle = 360f;
    [Header("发射波数")]
    public int wave = 48;

    private IEnumerator func(EBContext context)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.0f, 2f));
        context.bullet.state = EBulletState.Dying;
        EBulletBatch batch = new EBulletBatch();
        for (int i = 0; i < 2; i++)
        {
            batch.AddBullet(
              GetBullet()
                .SetPosition(context.bullet.position)
                .SetSpeed(0.2f, UnityEngine.Random.Range(225f, 315f))
                .SetLimit(new Vector2(0f, 1.4f))
                .SetTurnRate(UnityEngine.Random.Range(-15f, 15f))
                .SetAccelarate(new Vector2(0f, -0.4f))
              );
        }

        batch.Packed("Human", new Vector3(0f, 0f, 255f)).Active();
        yield break;
    }

    bool check(in EBContext context)
    {
        return context.bullet.timer >= 1.5f;
    }

    public IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        float baseAngle = 0f;
        float time = 8f;
        int bulletCount = 2;
        Vector3 firePos = new();
        while (true)
        {
            GetLaser()
              .SetWarning()
              .SetPosition(new Vector3(-3.9f, 3.2f, 0f))
              .SetSpeed(0, 0f)
              .SetArea(12f, 1.2f)
              .SetDuration(1.5f)
              .SetAppearance("Laser", new Vector3(255f, 128f, 0f))
              .Active();
            yield return new WaitForSeconds(0.8f);

            float speed = 8f;
            for (int t = 0; t < 16; t++)
            {
                EBulletBatch batch1 = new EBulletBatch();
                EBulletBatch batch2 = new EBulletBatch();
                for (int i = 0; i < bulletCount / 2; i++)
                {
                    firePos.x = -3.9f;
                    firePos.y = UnityEngine.Random.Range(3.0f, 3.8f);
                    float angle = 0f + UnityEngine.Random.Range(-3f, 3f);
                    batch1.AddBullet(
                        GetBullet()
                        .SetPosition(firePos)
                        .SetSpeed((speed - (t * 6 / (16 - 1))) * UnityEngine.Random.Range(0.8f, 1.2f), angle)
                        .SetLimit(new Vector2(0.1f, -1f))
                        .SetSpeedRate(-2.0f)
                    );
                }
                for (int i = 0; i < bulletCount / 2; i++)
                {
                    firePos.x = -3.9f;
                    firePos.y = UnityEngine.Random.Range(3.0f, 3.8f);
                    float angle = 0f + UnityEngine.Random.Range(-3f, 3f);
                    batch2.AddBullet(
                        GetBullet()
                        .SetPosition(firePos)
                        .SetSpeed((speed - (t * 6 / (16 - 1))) * UnityEngine.Random.Range(0.8f, 1.2f), angle)
                        .SetLimit(new Vector2(0.1f, -1f))
                        .SetSpeedRate(-2.0f)
                    );
                }
                batch1.AttachCallBack(check, (context) => func(context));
                batch2.AttachCallBack(check, (context) => func(context));
                batch1.Packed("Big", new Vector3(204f, 204f, 204f)).Active();
                batch2.Packed("Medium", new Vector3(233f, 233f, 233f)).Active();
                yield return new WaitForSeconds(0.02f);
            }

            yield return new WaitForSeconds(time);
            time -= 0.4f;
            bulletCount += 1;
            if (time < 0.8f) time = 0.8f;
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
