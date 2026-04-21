using UnityEngine;

public enum ItemType { Score, SmallPower, BigPower, ClearBullet, Life, Bomb }

public struct Item
{
    public Vector3 position;
    public Vector3 velocity;
    public ItemType type;
    public bool isActive;

    public Item(Vector3 pos, Vector3 velocity, ItemType type)
    {
        position = pos;
        this.velocity = velocity;
        this.type = type;
        this.isActive = true;
    }
}
