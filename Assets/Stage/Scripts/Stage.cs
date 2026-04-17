using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StageEnemyConfig
{
    public int hp;
    public Vector3 position;
}

[Serializable]
public class StageSpawnCommand
{
    [Header("生成配置")]
    public int enemyPrefabIndex = 0;

    [Header("敌人配置")]
    public List<StageEnemyConfig> enemyConfigs = new();

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
                    SpawnOneWave(cmd);
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

    private void SpawnOneWave(StageSpawnCommand cmd)
    {
        if (EnemyManager.Instance == null) return;

        foreach (var conf in cmd.enemyConfigs)
        {
            EnemyManager.Instance.SpawnEnemy(cmd.enemyPrefabIndex, conf.hp, conf.position, cmd.motion, cmd.spawner);
        }
    }
}
