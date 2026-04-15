using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    [SerializeField, Range(0, 128)] private int power = 0;
    private const int POWER_MAX = 128;

    // 档位：0~5（0=初始，5=满火）
    private int shotLevel = 0;

    private int EvaluateShotLevel(int p)
    {
        if (p >= 128) return 5;
        if (p >= 80) return 4;
        if (p >= 48) return 3;
        if (p >= 24) return 2;
        if (p >= 8) return 1;
        return 0;
    }

    public void AddPower(int amount)
    {
        int oldLevel = shotLevel;
        power = Mathf.Clamp(power + amount, 0, POWER_MAX);
        shotLevel = EvaluateShotLevel(power);

        if (shotLevel != oldLevel)
        {
            OnShotLevelUp(shotLevel); // 播特效/音效/UI提示
        }
    }

    private void Shoot()
    {
        // 核心改动：不再 Instantiate，而是从对象池 Get
        PlayerBullet[] bullets = new PlayerBullet[4];
        Vector3[] pos = new Vector3[4];

        pos[0] = firePoint.position + new Vector3(0.2f, 0.1f, 0);
        pos[1] = firePoint.position + new Vector3(-0.2f, 0.1f, 0);
        pos[2] = firePoint.position + new Vector3(0.4f, 0, 0);
        pos[3] = firePoint.position + new Vector3(-0.4f, 0, 0);

        for (int i = 0; i < 4; i++)
        {
            bullets[i] = PBulletManager.Instance.GetBullet();
            if (bullets[i] != null)
            {
                bullets[i].transform.position = pos[i];
                bullets[i].transform.rotation = Quaternion.identity;
                // 如果需要改变方向或速度，可以在这里设置
            }
        }
    }

    private void OnShotLevelUp(int level)
    {

    }
}

