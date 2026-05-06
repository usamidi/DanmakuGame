using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct EMContext
{
    public Transform self;
    public EnemyBulletSpawner spawner;
}


[System.Serializable]
public abstract class EnemyMotion : ScriptableObject
{
    public abstract IEnumerator Move(EMContext context);
}


