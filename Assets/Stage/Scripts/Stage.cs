using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageSpawnCommand
{
    [Header("生成配置")]
    public int enemyPrefabIndex = 0;
    public Vector3 spawnPosition;
    public float delayFromPrevious = 0.15f;

    [Header("移动模式")]
    public EnemyMotion motion;

    [Header("生成数量")]
    public int number;
    [Header("生成间隔")]
    public float interval;

    [Header("发射弹幕样式")]
    [SerializeReference] public EnemyBulletSpawner spawner;
}

[Serializable]
public class StageWave
{
    public string waveName = "Wave";
    public float preDelay = 0f;
    public StageSpawnCommand[] spawns = Array.Empty<StageSpawnCommand>();
    [Tooltip("该波刷完后是否等待清场")]
    public bool waitUntilNoEnemy = false;
    public float postDelay = 0f;
}


[CreateAssetMenu(fileName = "Stage", menuName = "STG/Stage")]
public class Stage : ScriptableObject
{
    [Header("关卡信息")]
    [SerializeField] private string stageId = "Stage-n";
    [Header("流程配置")]
    [SerializeField] private float introDelay = 0.8f;
    [SerializeField] private float clearDelay = 1.2f;
    [SerializeField] private StageWave[] waves = Array.Empty<StageWave>();

    public string StageId => stageId;

    public IEnumerator Run()
    {
        if (introDelay > 0f) yield return new WaitForSeconds(introDelay);
        foreach (var wave in waves)
        {
            if (wave.preDelay > 0f) yield return new WaitForSeconds(wave.preDelay);
            for (int i = 0; i < wave.spawns.Length; i++)
            {
                StageSpawnCommand cmd = wave.spawns[i];
                if (cmd.delayFromPrevious > 0f)
                    yield return new WaitForSeconds(cmd.delayFromPrevious);
                for (int j = 0; j < cmd.number; j++)
                {
                    SpawnOne(cmd);
                    yield return new WaitForSeconds(cmd.interval);
                }
            }
            if (wave.waitUntilNoEnemy)
            {
                yield return new WaitWhile(() => (EnemyManager.Instance != null && EnemyManager.Instance.AliveCount > 0));
            }
            if (wave.postDelay > 0f) yield return new WaitForSeconds(wave.postDelay);
        }
        if (clearDelay > 0f) yield return new WaitForSeconds(clearDelay);
    }

    private void SpawnOne(StageSpawnCommand cmd)
    {
        if (EnemyManager.Instance == null) return;
        Enemy enemy = EnemyManager.Instance.SpawnEnemy(cmd.enemyPrefabIndex, cmd.spawnPosition, cmd.motion, cmd.spawner);
        if (enemy == null) return;
    }
}
