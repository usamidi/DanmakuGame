using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct ESContext
{
    public Transform self;
    public Transform player;
}

[System.Serializable]
public abstract class EnemyBulletSpawner : ScriptableObject
{
    public static Func<EBulletData> GetBullet;
    public static Func<ELaserData> GetLaser;

    public Func<IEnumerator, Coroutine> StartCoroutine;
    public int moveSteps { get; private set; } = 0;
    public abstract List<Func<ESContext, IEnumerator>> SpawnerList();
    public virtual void OnDespawn()
    {
        return;
    }

    public void NextMoveStep()
    {
        moveSteps++;
    }

    public void ClearMoveStep()
    {
        moveSteps = 0;
    }

    public static float GetAngleToPlayer(Vector3 position)
    {
        Player player = EBulletManager.Instance.player;
        if (player == null) return 270f; // 如果玩家死了或不存在，默认向下射击 (270度)

        Vector2 dir = player.transform.position - position;
        // Mathf.Atan2 返回的是弧度，需要乘以 Mathf.Rad2Deg 转换为度数
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle;
    }

    public static float GetAngleToPosition(Vector3 position, Vector3 destination)
    {
        Vector2 dir = destination - position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle;
    }
}



