using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EBullet<TSelf> where TSelf : EBullet<TSelf>
{
    protected Vector2 _position;
    public Vector2 position
    {
        get
        {
            return new Vector2(Mathf.Cos(rotation * Mathf.Deg2Rad), Mathf.Sin(rotation * Mathf.Deg2Rad)) * radius + _position;
        }
    }
    public float depth { get; protected set; }

    public Vector2 velocity
    {
        get
        {
            Vector2 dir = new Vector2(Mathf.Cos(direction * Mathf.Deg2Rad), Mathf.Sin(direction * Mathf.Deg2Rad));
            return dir * speed;
        }

        protected set
        {
            speed = value.magnitude;
            if (speed != 0) direction = Mathf.Atan2(value.y, value.x) * Mathf.Rad2Deg;
        }

    }

    // 速度
    public float speed { get; protected set; }
    // 速度方向
    public float direction { get; protected set; }

    // 极坐标表示相对位置
    // 角度
    public float rotation { get; protected set; }
    // 半径
    protected float radius = 0f;

    // 加速度
    protected Vector2 accelarate = Vector2.zero;
    // 速度加速度
    protected float speedRate = 0f;
    // 飞行方向的角速度（让弹道转弯）
    protected float turnRate = 0f;
    // 转速
    protected float spinRate = 0f;
    // 速度上限
    protected Vector2 limit = new Vector2(0f, -1f);


    public virtual void Move(float dt)
    {
        direction += turnRate * dt;
        _position += velocity * dt;

        if ((speed < limit.y || limit.y < 0f) && speed > limit.x)
        {
            if (accelarate != Vector2.zero)
            {
                velocity += accelarate * dt;

            }

            if (speedRate != 0)
            {
                speed += speedRate * dt;
            }
        }


        if (spinRate <= 0f)
        {
            rotation = direction;
        }
        else
        {
            rotation += spinRate * dt;
        }
    }

    public void ClearMoveParam()
    {
        _position = Vector2.zero;
        depth = 0f;
        speed = 0f;
        direction = 0f;
        rotation = 0f;
        radius = 0f;

        accelarate = Vector2.zero;
        speedRate = 0f;
        turnRate = 0f;
        spinRate = 0f;
        limit = new Vector2(0f, -1f);
    }

    public Vector3 Position()
    {
        return new Vector3(position.x, position.y, depth);
    }

    public float Rotation() => rotation;


    public TSelf SetPosition(Vector2 pos, float depth = 0f) { _position = pos; this.depth = depth; return this as TSelf; }
    public TSelf SetSpeed(float s, float deg) { speed = s; direction = deg; return this as TSelf; }
    public TSelf SetSpeed(Vector2 velocity) { this.velocity = velocity; return this as TSelf; }
    public TSelf SetPolar(float rotation, float radius) { this.rotation = rotation; this.radius = radius; return this as TSelf; }
    public TSelf SetAccelarate(Vector2 a) { accelarate = a; return this as TSelf; }
    public TSelf SetSpeedRate(float a) { speedRate = a; return this as TSelf; }
    public TSelf SetLimit(Vector2 limit) { this.limit = limit; return this as TSelf; }
    public TSelf SetSpinRate(float s) { spinRate = s; return this as TSelf; }
    public TSelf SetTurnRate(float t) { turnRate = t; return this as TSelf; }
    public TSelf Rotate(float a) { direction += a; return this as TSelf; }
}

public class EBulletData : EBullet<EBulletData>
{
    public float spawnDuration = 0.3f;
    public float dieDuration = 0.3f;

    public EBulletState state;
    public EBBoundType boundType = EBBoundType.None;
    public float timer = 0f;

    public bool isGrazed = false;

    public int reflectTimes = 0;

    //public void Active(Vector3 startPos, float speed, float degree, int reflects = 0)
    public void Active()
    {
        //this.speed = speed;
        //direction = degree;
        //float angleRad = degree * Mathf.Deg2Rad;
        //Vector3 dir = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0);
        //velocity = dir * speed;
        //position = startPos;
        //rotation = degree;
        //reflectTimes = reflects;
        state = EBulletState.Spawning;
    }

    /*
    public void SetDirection(float degree)
    {
        direction = degree;
        rotation = degree;
    }
    */

    public void Clear()
    {
        ClearMoveParam();
        state = EBulletState.Dead;
        boundType = EBBoundType.None;
        timer = 0f;
        spawnDuration = 0.3f;
        dieDuration = 0.3f;
        isGrazed = false;
        reflectTimes = 0;
    }

    public float GetReflectAngle()
    {
        Vector2 normal;
        switch (boundType)
        {
            case EBBoundType.Left:
                normal = Vector2.right;
                break;
            case EBBoundType.Right:
                normal = Vector2.left;
                break;
            case EBBoundType.Top:
                normal = Vector2.down;
                break;
            case EBBoundType.Bottom:
                normal = Vector2.up;
                break;
            default:
                return 0f;
        }
        Vector2 reflectedDir = Vector2.Reflect(velocity, normal);

        // 得到新角度
        return Mathf.Atan2(reflectedDir.y, reflectedDir.x) * Mathf.Rad2Deg;
    }
}


public struct EBulletAppearance
{
    public string style;
    public Vector3 color;

    public EBulletAppearance(string s, Vector3 clr)
    {
        style = s;
        color = clr;
    }
}
