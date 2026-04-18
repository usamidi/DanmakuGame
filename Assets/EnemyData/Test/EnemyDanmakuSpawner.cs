using System.Collections;
using UnityEngine;

public class EnemyDanmakuSpawner : MonoBehaviour
{
    public enum PatternType
    {
        AimingSpread, // 自机狙散弹 (瞄准玩家的扇形)
        Spiral,       // 螺旋弹幕 (旋转喷射)
        CircleBurst,   // 环形爆发弹幕
        SepllCard1,
        SepllCard2,
        SepllCard3,
        SepllCard4,
        SepllCard5,
    }

    [Header("=== 基础设置 ===")]
    [Tooltip("选择要演示的弹幕类型")]
    public PatternType currentPattern = PatternType.AimingSpread;

    [Tooltip("对应 BulletManager 中的 registeredStyles 数组索引 (比如 0是红弹, 1是蓝弹)")]
    public int bulletStyleIndex = 0;

    [Tooltip("玩家的 Transform，用于瞄准 (自机狙)")]
    public Transform player;

    [Header("=== 弹幕参数 ===")]
    public float bulletSpeed = 4f;       // 子弹速度
    public float fireRate = 0.5f;        // 发射间隔 (秒)
    public Vector3 bulletColor = new Vector3(255, 0, 0);

    [Header("参数: 散弹 (AimingSpread)")]
    public int spreadLines = 5;          // 散弹的条数 (奇数必有一发正对玩家)
    public float spreadAngle = 60f;      // 扇形总角度

    [Header("参数: 螺旋 (Spiral)")]
    public int spiralWays = 4;           // 螺旋的臂数 (几条线一起转)
    public float spiralSpinSpeed = 15f;  // 每次发射旋转的角度差

    private Coroutine activePattern;

    void Start()
    {
        // 如果没有手动拖拽玩家，尝试通过标签自动寻找
        if (player == null)
        {
            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) player = pObj.transform;
        }

        // 启动选择的弹幕模式
        StartPattern(currentPattern);
    }

    /// <summary>
    /// 切换并启动弹幕模式
    /// </summary>
    public void StartPattern(PatternType type)
    {
        if (activePattern != null) StopCoroutine(activePattern);

        switch (type)
        {
            case PatternType.AimingSpread:
                activePattern = StartCoroutine(FireAimingSpread());
                break;
            case PatternType.Spiral:
                activePattern = StartCoroutine(FireSpiral());
                break;
            case PatternType.CircleBurst:
                activePattern = StartCoroutine(FireCircleBurst());
                break;
            case PatternType.SepllCard1:
                activePattern = StartCoroutine(SpellCard1());
                break;
            case PatternType.SepllCard2:
                activePattern = StartCoroutine(SpellCard2());
                break;
            case PatternType.SepllCard3:
                activePattern = StartCoroutine(SpellCard3());
                break;
            case PatternType.SepllCard4:
                activePattern = StartCoroutine(SpellCard4());
                break;
            case PatternType.SepllCard5:
                activePattern = StartCoroutine(SpellCard5());
                break;
        }
    }

    // ==========================================
    // 经典弹幕模式 1：自机狙散弹 (N-Way)
    // ==========================================
    private IEnumerator FireAimingSpread()
    {

        for (int t = 0; t < 32; t++)
        {
            float speed = 3.5f;
            // float baseAngle = GetAngleToPlayer();
            Vector3 firePos1 = transform.position + new Vector3(-3f, 0f, 0f);
            Vector3 firePos2 = transform.position + new Vector3(3f, 0f, 0f);

            float baseAngle1 = GetAngleToPlayer(firePos1);
            float baseAngle2 = GetAngleToPlayer(firePos2);

            for (int n = 0; n < 50; n++)
            {

                float finalSpeed = speed + (float)n * 0.30f;
                // 1. 获取指向玩家的基础角度

                // 2. 计算扇形的起始角度和每条线的角度间隔
                float startAngle1 = baseAngle1 - (spreadAngle / 2f);
                float startAngle2 = baseAngle2 - (spreadAngle / 2f);

                float angleStep = spreadLines > 1 ? spreadAngle / (spreadLines - 1) : 0f;
                // 3. 循环生成多发子弹
                EBulletBatch batch = new EBulletBatch();
                for (int i = 0; i < spreadLines; i++)
                {
                    float currentAngle1 = startAngle1 + (angleStep * i);
                    float currentAngle2 = startAngle2 + (angleStep * i);
                    // 调用我们的 DOD 管理器发射子弹
                    batch.AddBullet(
                        firePos1, finalSpeed, currentAngle1
                        ).AddBullet(firePos2, finalSpeed, currentAngle2);
                    //BulletManager.Instance.SpawnBullet(bulletStyleIndex, transform.position, currentAngle, bulletSpeed, bulletColor);
                }
                //EBulletManager.Instance.SpawnBullet(batch, bulletStyleIndex, bulletColor);
                EBulletManager.Instance.SpawnBullet(batch.Packed("Rice", new Vector3(255f, 10f, 10f)));

                // 4. 等待一段时间再次发射
                yield return new WaitForSeconds(0.015f);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    // ==========================================
    // 经典弹幕模式 2：多臂螺旋弹 (Spiral)
    // ==========================================
    private IEnumerator FireSpiral()
    {

        // 螺旋弹通常发射极快，为了演示，我们覆盖一下 fireRate
        float fastFireRate = 0.05f;
        float currentRotation = 0f; // 记录当前的旋转偏移量

        while (true)
        {
            float angleStep = 360f / spiralWays; // 把 360 度平分给几条旋臂

            EBulletBatch batch = new EBulletBatch();
            for (int i = 0; i < spiralWays; i++)
            {
                float finalAngle = currentRotation + (angleStep * i);
                batch.AddBullet(transform.position, bulletSpeed, finalAngle);
                //BulletManager.Instance.SpawnBullet(bulletStyleIndex, transform.position, finalAngle, bulletSpeed, bulletColor);
            }
            EBulletManager.Instance.SpawnBullet(batch.Packed("Rice", new Vector3(0f, 0f, 255f)));

            // 每次发射后，让偏移量增加，形成螺旋效果
            currentRotation += spiralSpinSpeed;

            yield return new WaitForSeconds(fastFireRate);
        }
    }

    // ==========================================
    // 经典弹幕模式 3：环形爆发弹 (Circle Burst)
    // ==========================================
    private IEnumerator FireCircleBurst()
    {
        int bulletCount = 8; // 一圈 36 发 (每 10 度一发)

        float speed = 4.0f;
        float angleStep = 360f / bulletCount;

        IEnumerator generate(EBContext context)
        {

            float oldSpeed = context.bullet.speed;
            float duration = 2.0f;
            float t = context.bullet.timer / duration;
            while (t <= 1f)
            {
                context.bullet.speed = Mathf.Lerp(oldSpeed, 0f, t);
                yield return new WaitForSeconds(context.dt);
                t = context.bullet.timer / duration;
            }
            context.bullet.speed = 0f;
            yield return new WaitForSeconds(0.5f);
            context.bullet.SetDirection(GetAngleToPlayer(context.bullet.position));
            context.bullet.speed = 5.0f;



            /*
            //while (context.bullet.state != EBulletState.ReachBound) yield return new WaitForSeconds(0.5f);
            //context.bullet.state = EBulletState.Dying;
            EBulletBatch batch = new EBulletBatch();
            float speed = 3.2f;
            float angle = 270f;
            //angle = GetAngleToPlayer(context.bullet.position);
            //for (int i = 0; i < Random.Range(1, 3); i++)
            //float offsetAng = Random.Range(-10f, 10f);
            float offsetAng = 0f;
            //float finalAngle = angle + offsetAng;
            float finalAngle = angle + offsetAng;
            batch.AddBullet(context.bullet.position, speed, finalAngle);
            EBulletManager.Instance.SpawnBullet(batch.Packed(2, new Vector3(9f, 247f, 247f)));
            */
            yield break;
        }

        bool check(in EBContext context)
        {
            //return context.bullet.state == EBulletState.ReachBound && context.bullet.boundType == EBBoundType.Top;
            return true;
            //   return context.bullet.position.y < 0f;
        }

        while (true)
        {
            float offsetAng = Random.Range(-30f, 30f);
            for (int k = 0; k < 4; k++)
            {
                EBulletBatch batch = new EBulletBatch();
                for (int i = 0; i < bulletCount; i++)
                {
                    float finalAngle = angleStep * i + offsetAng;
                    float radians = finalAngle * Mathf.Deg2Rad;
                    Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0).normalized * 0.5f;

                    batch.AddBullet(transform.position + offsetPos + new Vector3(0f, 0f, 2.0f), speed, finalAngle);
                    //BulletManager.Instance.SpawnBullet(bulletStyleIndex, transform.position + offsetPos, finalAngle, speed, bulletColor);
                }

                EBulletManager.Instance.SpawnBullet(
                    batch.AttachCallBack(check, (context) => generate(context)
                      ).Packed("Scale", new Vector3(0f, 255f, 0f)));

                // 爆发弹通常需要蓄力/间隔长一点
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator RandomFire(int bulletCount)
    {
        while (true)
        {
            EBulletBatch batch = new EBulletBatch();
            for (int i = 0; i < bulletCount; i++)
            {
                Vector3 pos1 = UnityEngine.Random.insideUnitCircle * 0.3f;
                Vector3 pos2 = UnityEngine.Random.insideUnitCircle * 0.3f;

                float angle1 = Random.Range(0f, 360f);
                float angle2 = Random.Range(0f, 360f);

                float speed1 = Mathf.Lerp(1.2f, 4.8f, Random.Range(0f, 1f));
                float speed2 = Mathf.Lerp(1.2f, 4.8f, Random.Range(0f, 1f));
                //float speed2 = Random.Range(1.2f, 3.6f);
                //EBulletBatch batch2 = new EBulletBatch(new Vector3(255f, 10f, 10f));
                batch.AddBullet(
                    pos1 + new Vector3(transform.position.x - 0.5f, transform.position.y, 1f), speed1, angle1
                ).AddBullet(
                    pos2 + new Vector3(transform.position.x + 0.5f, transform.position.y, 1f), speed2, angle2
                );
            }
            EBulletManager.Instance.SpawnBullet(batch.Packed("Small-1", new Vector3(0, 0, 255f)));
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator SpellCard1()
    {
        //StartCoroutine(FireAimingSpread());
        StartCoroutine(RandomFire(8));
        yield return FireCircleBurst();
    }


    private IEnumerator SpellCard2()
    {
        int bulletCount = 5; // 一圈 36 发 (每 10 度一发)

        float speed = 1f;
        float angleStep = 360f / bulletCount;

        IEnumerator generate(EBContext context)
        {
            float speed = 1.8f;

            EBulletBatch batch1 = new EBulletBatch();
            EBulletBatch batch2 = new EBulletBatch();
            yield return new WaitForSeconds(2.5f);
            Debug.Log(context.bullet.velocity);

            if (context.bullet.state != EBulletState.Normal)
            {
                yield break;
            }
            else
            {
                context.bullet.velocity = Vector3.zero;

                for (int i = 0; i < 24; i++)
                {
                    Vector3 pos = UnityEngine.Random.insideUnitCircle * 0.3f;
                    float angle = Random.Range(0f, 360f);
                    float s = Mathf.Lerp(1.2f, 4.8f, Random.Range(0f, 1f));
                    batch1.AddBullet(
                        pos + new Vector3(context.bullet.position.x, context.bullet.position.y, 0f), s, angle
                    );
                }

                for (int i = 0; i < 8; i++)
                {
                    Vector3 pos = UnityEngine.Random.insideUnitCircle * 0.3f;
                    float angle = Random.Range(0f, 360f);
                    float s = Mathf.Lerp(1.2f, 4.8f, Random.Range(0f, 1f));
                    batch2.AddBullet(
                        pos + new Vector3(context.bullet.position.x, context.bullet.position.y, 0f), s, angle
                    );
                }

                EBulletManager.Instance.SpawnBullet(batch1.Packed("Small-1", new Vector3(0, 0, 233f)));
                EBulletManager.Instance.SpawnBullet(batch2.Packed("Small-2", new Vector3(233f, 0, 233f)));

                yield return new WaitForSeconds(0.2f);
                context.bullet.state = EBulletState.Dying;
                yield break;
            }
        }
        bool check(in EBContext context)
        {
            return true;
        }

        while (true)
        {
            float offsetAng = Random.Range(-30f, 30f);
            EBulletBatch batch = new EBulletBatch();
            for (int i = 0; i < bulletCount; i++)
            {
                float finalAngle = angleStep * i + offsetAng;
                float radians = finalAngle * Mathf.Deg2Rad;
                Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0).normalized * 0.5f;

                batch.AddBullet(transform.position + offsetPos, speed, finalAngle);
                //BulletManager.Instance.SpawnBullet(bulletStyleIndex, transform.position + offsetPos, finalAngle, speed, bulletColor);
            }

            EBulletManager.Instance.SpawnBullet(
                batch.AttachCallBack(check, (context) => generate(context)
                  ).Packed("Big", new Vector3(255f, 0f, 0f))
                );

            // 爆发弹通常需要蓄力/间隔长一点
            yield return new WaitForSeconds(3.6f);
        }
    }

    private IEnumerator SpellCard3()
    {
        int bulletCount = 6; // 一圈 36 发 (每 10 度一发)

        float speed = 4.0f;

        IEnumerator generate(EBContext context)
        {
            //while (context.bullet.state != EBulletState.ReachBound) yield return new WaitForSeconds(0.5f);
            context.bullet.state = EBulletState.Dying;
            EBulletBatch batch = new EBulletBatch();
            float speed = 3.2f;
            float angle = 270f;
            angle = GetAngleToPlayer(context.bullet.position);
            //for (int i = 0; i < Random.Range(1, 3); i++)
            {
                //float offsetAng = Random.Range(-30f, 30f);
                //float finalAngle = angle + offsetAng;
                float finalAngle = angle;
                batch.AddBullet(context.bullet.position, speed, finalAngle);
            }
            EBulletManager.Instance.SpawnBullet(batch.Packed("Rice", new Vector3(9f, 247f, 247f)));
            yield break;
        }

        bool check(in EBContext context)
        {
            //return context.bullet.state == EBulletState.ReachBound;
            return context.bullet.position.y < -2f;
        }

        float posStep = 14.0f / bulletCount;
        float startPos = -7.0f;

        while (true)
        {
            float offsetAng = Random.Range(-30f, 30f);

            EBulletBatch batch = new EBulletBatch();
            for (int i = 0; i < bulletCount; i++)
            {
                //Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0).normalized * 0.5f;
                Vector3 offsetPos = UnityEngine.Random.insideUnitCircle * 0.3f;
                float finalPos = startPos + posStep * i;

                batch.AddBullet(new Vector3(finalPos, 3.5f, 0f) + offsetPos, speed, 270f);
            }

            EBulletManager.Instance.SpawnBullet(
                batch.AttachCallBack(check, (context) => generate(context)
                  ).Packed("Small-1", new Vector3(255f, 255f, 0f)));

            // 爆发弹通常需要蓄力/间隔长一点
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator SpellCard4()
    {
        int bulletCount = 5; // 一圈 36 发 (每 10 度一发)

        float angleStep = 360f / bulletCount;

        IEnumerator generate(EBContext context)
        {
            EBulletBatch batch = new EBulletBatch();
            float angle = 270f;
            //angle = GetAngleToPlayer(context.bullet.position);
            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                float offsetAng = Random.Range(-10f, 10f);
                //float offsetAng = 0f;
                float finalAngle = angle + offsetAng;
                batch.AddBullet(context.bullet.position, 3.2f, finalAngle);
            }
            EBulletManager.Instance.SpawnBullet(batch.Packed("Chain", new Vector3(9f, 247f, 247f)));
            yield break;
        }
        bool check(in EBContext context)
        {
            return context.bullet.state == EBulletState.ReachBound && context.bullet.boundType != EBBoundType.Bottom;
        }

        while (true)
        {
            float offsetAng = GetAngleToPlayer();
            for (int k = 0; k < 6; k++)
            {
                EBulletBatch batch = new EBulletBatch();
                for (int i = 0; i < bulletCount; i++)
                {
                    float finalAngle = angleStep * i + offsetAng;
                    float radians = finalAngle * Mathf.Deg2Rad;
                    Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0).normalized * 0.1f;

                    batch.AddBullet(transform.position + offsetPos, 5.5f, finalAngle);
                    //BulletManager.Instance.SpawnBullet(bulletStyleIndex, transform.position + offsetPos, finalAngle, speed, bulletColor);
                }

                EBulletManager.Instance.SpawnBullet(
                    batch.AttachCallBack(check, (context) => generate(context)
                      ).Packed("Chain", new Vector3(255f, 255f, 0f)));

                // 爆发弹通常需要蓄力/间隔长一点
                yield return new WaitForSeconds(0.06f);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator SpellCard5()
    {
        int bulletCount = 280; // 一圈 36 发 (每 10 度一发)

        float angleStep = 360f / bulletCount;

        float offsetAng = 0f;

        while (true)
        {
            for (int k = 0; k < 12; k++)
            {
                offsetAng += 15f;
                EBulletBatch batch = new EBulletBatch();
                for (int i = 0; i < bulletCount; i++)
                {
                    float finalAngle = angleStep * i + offsetAng;
                    float radians = finalAngle * Mathf.Deg2Rad;
                    Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0).normalized * 0.1f;

                    batch.AddBullet(transform.position + offsetPos, 1f, finalAngle);
                    //BulletManager.Instance.SpawnBullet(bulletStyleIndex, transform.position + offsetPos, finalAngle, speed, bulletColor);
                }

                EBulletManager.Instance.SpawnBullet(
                    batch.Packed("Scale", new Vector3(255f, 255f, 0f)));

                yield return new WaitForSeconds(0.08f);
            }
            yield return new WaitForSeconds(6f);
        }

    }


    // ==========================================
    // 辅助数学计算：获取从敌人指向玩家的角度 (度数)
    // ==========================================
    private float GetAngleToPlayer()
    {
        if (player == null) return 270f; // 如果玩家死了或不存在，默认向下射击 (270度)

        Vector2 dir = player.position - transform.position;
        // Mathf.Atan2 返回的是弧度，需要乘以 Mathf.Rad2Deg 转换为度数
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle;
    }

    private float GetAngleToPlayer(Vector3 position)
    {
        if (player == null) return 270f; // 如果玩家死了或不存在，默认向下射击 (270度)

        Vector2 dir = player.position - position;
        // Mathf.Atan2 返回的是弧度，需要乘以 Mathf.Rad2Deg 转换为度数
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle;
    }
}
