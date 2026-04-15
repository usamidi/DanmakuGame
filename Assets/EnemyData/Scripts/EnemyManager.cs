using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("===Prefabs===")]
    [SerializeField] private Enemy[] enemyPrefabs;

    [Header("===Refs===")]
    [SerializeField] private Player player;

    private readonly List<IObjectPool<Enemy>> pools = new();
    private readonly HashSet<Enemy> aliveEnemies = new();

    public int AliveCount
    {
        get => aliveEnemies.Count;

    }

    private Coroutine waveCoroutine;

    void Awake()
    {
        Instance = this;
        BuildPools();
    }

    private void BuildPools()
    {
        foreach (var e in enemyPrefabs)
        {
            pools.Add(CreatePool(e, 32, 256));

        }
    }

    private IObjectPool<Enemy> CreatePool(Enemy prefab, int cap, int max)
    {
        return new ObjectPool<Enemy>(
            () => Instantiate(prefab).SetPlayer(player),
            e => e.gameObject.SetActive(true),
            e => e.gameObject.SetActive(false),
            e => Destroy(e.gameObject),
            true, cap, max
        );
    }

    public Enemy SpawnEnemy(int type, Vector3 pos, EnemyMotion motion, EnemyBulletSpawner spawner)
    {
        Enemy e = pools[type].Get();
        e.transform.position = pos;
        e.SetMotion(motion).SetSpawner(spawner).OnSpawned();
        //e.BindManager(this, type);   // 让 Enemy 知道回收去哪里
        //e.OnSpawned();               // 重置HP、启动移动/射击
        aliveEnemies.Add(e);
        return e;
    }

    public void DespawnEnemy(Enemy enemy, int type)
    {
        if (enemy == null) return;
        enemy.StopMove();
        if (aliveEnemies.Remove(enemy))
        {
            //enemy.OnDespawned();     // 停止协程、清状态
            pools[type].Release(enemy);
        }
    }

    public void ClearAllEnemies()
    {
        // 拷贝列表避免遍历中修改
        var temp = new List<Enemy>(aliveEnemies);
        //foreach (var e in temp) e.ForceDieAndDespawn();
    }

    /*
    public void StartWave(List<EnemySpawnRequest> wave)
    {
        if (waveCoroutine != null) StopCoroutine(waveCoroutine);
        waveCoroutine = StartCoroutine(SpawnWaveRoutine(wave));
    }

    private IEnumerator SpawnWaveRoutine(List<EnemySpawnRequest> wave)
    {
        foreach (var req in wave)
        {
            if (req.delay > 0) yield return new WaitForSeconds(req.delay);
            SpawnEnemy(req.type, req.position);
        }
        waveCoroutine = null;
    }
    */
}
