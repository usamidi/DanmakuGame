using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("测试新弹幕用")]
    [SerializeReference]
    public EnemyBulletSpawner spawner;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        ESContext context = new();
        context.player = player.transform;
        context.self = transform;
        foreach (var func in spawner.SpawnerList())
        {
            StartCoroutine(func(context));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
