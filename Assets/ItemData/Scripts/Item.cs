using UnityEngine;

public enum ItemType { Score, SmallPower, BigPower, ClearBullet, Life, Bomb }

public struct Item
{
    public Vector3 position;
    public Vector3 velocity;
    public bool isActive;
    public bool isAttract;

    public Item(Vector3 pos)
    {
        position = pos;
        velocity = new Vector3(0f, 3f, 0f);
        isActive = false;
        isAttract = false;
    }

    public Item SetPosition(Vector3 pos)
    {
        position = pos;
        return this;
    }

    public void SetAttract()
    {
        isAttract = true;
    }

    public void Move(float dt, Vector3 gravity, Vector3 playerPos)
    {
        if (isAttract)
        {
            position = Vector3.MoveTowards(position, playerPos, 9f * dt);
        }
        else
        {
            position += velocity * dt;
            if (velocity.y >= -3f)
            {
                velocity += gravity * dt;
            }
        }
    }

    public Item Active()
    {
        isActive = true;
        return this;
    }

    public void Deactive()
    {
        isActive = false;
        isAttract = false;
        velocity = new Vector3(0f, 3f, 0f);
    }
}
