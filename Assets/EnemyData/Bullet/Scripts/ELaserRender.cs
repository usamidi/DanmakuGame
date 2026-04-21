using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public struct LaserStylePair
{
    public string name;
    public ELaserStyle style;
}

public partial class EBulletManager : MonoBehaviour
{
    [Header("=== 注册所有激光样式 ===")]
    [Tooltip("将创建的所有 ELaserStyle ScriptableObject 拖进来")]
    public List<LaserStylePair> registeredLaserStyles;

    [Header("=== 激光设置 ===")]
    [SerializeField] private float spawnDuration;
    [SerializeField] private float dieDuration;


    [Header("=== 激光预警线设置 ===")]
    [SerializeField] private float warningWidth;
    [SerializeField] private float warningDuration;
    [SerializeField] private float widenDuration;
    public float warningSpeed = 16f;


    private Dictionary<string, ELaserStyle> laserStyleDict = new();
    private readonly List<ELaserData> lasers = new();
    private MaterialPropertyBlock propBlock;

    private const int MAX_LASERS = 256;
    private const float MIN_SPLIT_LENGTH = 0.3f;
    private const float GRAZE_COOLDOWN = 0.15f;
    // =========================================================
    // 初始化（在已有 Awake() 里调用一次即可）
    // =========================================================
    private void InitLaserStyleMap()
    {
        foreach (var p in registeredLaserStyles)
            laserStyleDict[p.name] = p.style;
    }

    private void InitLaserList()
    {
        propBlock = new MaterialPropertyBlock();
        for (int i = 0; i < MAX_LASERS; i++)
        {
            lasers.Add(new ELaserData());
        }
    }

    private ELaserData GetLaser()
    {
        return lasers.FirstOrDefault((l) => !l.isAcive);
    }

    private void ReleaseLaser(ELaserData laser)
    {
        laser.Clear();
    }

    // =========================================================
    // 外部 API
    // =========================================================
    public EBulletManager SpawnInstantLaser(
        string styleName, Vector3 position, float speed, float angle,
        float length, float width, Vector3 color)
    {
        if (!laserStyleDict.ContainsKey(styleName))
        {
            Debug.LogWarning($"[EBulletManager] Unknown laser style: {styleName}");
            return this;
        }

        ELaserData laser = GetLaser();

        if (laser == null)
        {
            Debug.LogWarning($"[EBulletManager] laser full");
            return this;
        }

        laser.ActiveInstant(styleName, position, speed, angle, length, width, color);
        return this;
    }

    public EBulletManager SpawnWarningLaser(
        string styleName, Vector3 position, float duration,
        float angle, float length, float width, Vector3 color)
    {
        if (!laserStyleDict.ContainsKey(styleName))
        {
            Debug.LogWarning($"[EBulletManager] Unknown laser style: {styleName}");
            return this;
        }

        ELaserData laser = GetLaser();

        if (laser == null)
        {
            Debug.LogWarning($"[EBulletManager] laser full");
            return this;
        }

        laser.ActiveWarning(styleName, position, duration, angle, length, width, color);
        return this;
    }

    public void KillAllLasers()
    {
        for (int i = lasers.Count - 1; i >= 0; i--)
            ReleaseLaser(lasers[i]);
    }
    // =========================================================
    // 主循环（在已有 Update() 里调用一次）
    // =========================================================
    public void UpdateLasers(float dt, Vector2 playerPos)
    {
        propBlock.Clear();
        for (int i = lasers.Count - 1; i >= 0; i--)
        {
            ELaserData laser = lasers[i];
            if (laser.isAcive == false) continue;

            laser.timer += dt;
            switch (laser.state)
            {
                case ELaserState.Spawning:
                    laser.currentLength = Mathf.Lerp(0f, laser.length,
                                                 laser.timer / spawnDuration);
                    switch (laser.type)
                    {
                        case ELaserType.Instant:
                            DoHitAndGraze(laser, playerPos, dt);
                            break;
                        case ELaserType.Warning:
                            laser.width = warningWidth;
                            break;
                    }

                    if (laser.timer >= spawnDuration)
                    {
                        laser.currentLength = laser.length;
                        laser.state = ELaserState.Normal;
                        laser.timer = 0f;
                    }
                    break;
                case ELaserState.Normal:
                    switch (laser.type)
                    {
                        case ELaserType.Instant:
                            laser.Move(dt);
                            if ((laser.position.x < boundsMin.x) || (laser.position.x > boundsMax.x)
                                || (laser.position.y < boundsMin.y) || (laser.position.y > boundsMax.y))
                            {
                                laser.state = ELaserState.Dying;
                                laser.timer = 0f;
                                break;
                            }
                            DoHitAndGraze(laser, playerPos, dt);
                            break;
                        case ELaserType.Warning:
                            if (laser.currentLength < laser.length)
                            {
                                laser.currentLength += dt * warningSpeed;

                            }
                            else
                            {
                                laser.currentLength = laser.length;

                            }

                            if (laser.timer < warningDuration) break;

                            if (laser.timer < widenDuration + warningDuration)
                            {
                                float w = Mathf.Clamp01((laser.timer - warningDuration) / widenDuration);
                                laser.width = Mathf.Lerp(warningWidth, laser.fullWidth, w);
                                if (w >= 0.5f) DoHitAndGraze(laser, playerPos, dt);
                            }
                            else if (laser.timer < widenDuration + warningDuration + laser.duration)
                            {
                                laser.width = laser.fullWidth;
                                DoHitAndGraze(laser, playerPos, dt);
                            }
                            else
                            {
                                laser.state = ELaserState.Dying;
                                laser.timer = 0f;
                            }
                            break;
                    }
                    break;
                case ELaserState.Dying:
                    float t = Mathf.Clamp01(laser.timer / dieDuration);
                    laser.width = Mathf.Lerp(laser.width, 0f, t);
                    if (t >= 1f) laser.state = ELaserState.Dead;
                    break;
                case ELaserState.Dead:
                    ReleaseLaser(laser);
                    continue;

            }
            DrawLaser(laser, propBlock);
        }
    }
    // =========================================================
    // 命中 & 擦弹（点对线段）
    // =========================================================
    private void DoHitAndGraze(ELaserData laser, Vector2 playerPos, float dt)
    {
        float hitboxWidth = laser.width * 0.6f;
        laser.grazeCooldown -= dt;
        float tHit = laser.Distance(playerPos, playerHitboxRadius + hitboxWidth * 0.5f);
        if (tHit >= 0f)
        {
            if (player != null && player.Missable())
            {
                player.PlayerMiss();
                missNum++;
                if (missUI != null) missUI.SetMiss(missNum);

            }
            if (destroyVFX != null)
            {
                Vector3 hp = laser.position + (Vector3)(laser.Dir2 * tHit);
                Instantiate(destroyVFX, hp, Quaternion.identity);
            }

            float gap = playerHitboxRadius * 2.5f + hitboxWidth;
            SplitLaser(laser, tHit, gap);
            return;
        }

        if (laser.grazeCooldown <= 0f)
        {
            float tGr = laser.Distance(playerPos, playerGrazeRadius + hitboxWidth * 0.5f);
            if (tGr >= 0f)
            {
                grazeNum++;
                if (grazeUI != null) grazeUI.SetGraze(grazeNum);
                laser.grazeCooldown = GRAZE_COOLDOWN;
                if (grazeVFX != null)
                {
                    Vector3 gp = laser.position + (Vector3)(laser.Dir2 * tGr);
                    Instantiate(grazeVFX, gp, Quaternion.identity);
                }
            }
        }
    }

    // =========================================================
    // 切分：命中处把激光切成两段
    // =========================================================
    private void SplitLaser(ELaserData laser, float tHit, float gap)
    {
        float halfGap = gap * 0.5f;
        float leftLen = tHit - halfGap;
        float rightLen = laser.currentLength - tHit - halfGap;

        if (leftLen >= MIN_SPLIT_LENGTH)
        {
            switch (laser.type)
            {
                case ELaserType.Instant:
                    var a = GetLaser();
                    if (a == null) return;
                    a.ActiveInstant(laser.styleName, laser.position, laser.speed, laser.direction,
                             leftLen, laser.width, laser.color);
                    a.state = ELaserState.Normal;
                    a.currentLength = leftLen;
                    a.grazeCooldown = 0.3f;

                    laser.state = ELaserState.Dead;
                    break;
                case ELaserType.Warning:
                    laser.currentLength = leftLen;
                    break;
            }
        }

        if (rightLen >= MIN_SPLIT_LENGTH)
        {
            Vector3 newPos = laser.position + (Vector3)(laser.Dir2 * (tHit + halfGap));
            var b = GetLaser();
            if (b == null) return;
            b.ActiveInstant(laser.styleName, newPos, laser.speed, laser.direction,
                     rightLen, laser.width, laser.color);
            b.state = ELaserState.Normal;
            b.currentLength = rightLen;
            b.grazeCooldown = 0.3f;
        }
    }
    /*
    */
    // =========================================================
    // 渲染：Quad(中心锚点) + TRS(length, width, 1)
    // =========================================================
    private void DrawLaser(ELaserData laser, MaterialPropertyBlock prop)
    {
        if (!laserStyleDict.TryGetValue(laser.styleName, out var style)) return;
        if (style == null || style.quadMesh == null || style.laserMaterial == null) return;
        if (laser.currentLength <= 0f || laser.width <= 0f) return;

        Vector2 d = laser.Dir2;
        Vector3 center = laser.position + (Vector3)(d * (laser.currentLength * 0.5f));
        center.z = style.zDepth;
        Matrix4x4 m = Matrix4x4.TRS(
            center,
            Quaternion.Euler(0f, 0f, laser.direction),
            new Vector3(laser.currentLength, laser.width, 1f)
        );
        Vector4 color = (Vector4)laser.color / 255.0f;
        color.w = 1f;
        prop.SetVector("_Color", color);
        Graphics.DrawMesh(style.quadMesh, m, style.laserMaterial, 0, null, 0, prop);
    }
}
