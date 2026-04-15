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
    public abstract IEnumerator BulletSpawn(ESContext context);
}



