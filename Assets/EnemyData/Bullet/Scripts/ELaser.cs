using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ELaserType : byte { Instant, Warning }

public enum ELaserState : byte { Spawning, Normal, Dying, Dead }

public class ELaserData
{
    public ELaserType type;
    public bool isAcive;

    public string styleName;
    public Vector3 color;

    public ELaserState state;

    public Vector3 position;
    public Vector3 velocity
    {
        get
        {
            Vector3 dir = new Vector3(Mathf.Cos(direction * Mathf.Deg2Rad), Mathf.Sin(direction * Mathf.Deg2Rad), 0);
            return dir * speed;
        }

        set
        {
            speed = value.magnitude;
            direction = Mathf.Atan2(value.y, value.x) * Mathf.Rad2Deg;
        }

    }


    public float speed;
    public float direction;

    public float length;
    public float currentLength;
    public float width;

    // 预警线用
    public float duration;
    public float fullWidth;


    public float grazeCooldown;
    public float timer;


    public Vector2 Dir2 => new Vector2(
        Mathf.Cos(direction * Mathf.Deg2Rad),
        Mathf.Sin(direction * Mathf.Deg2Rad));

    public void ActiveInstant(string styleName, Vector3 pos, float speed, float angle,
                       float length, float width, Vector3 color)
    {

        this.type = ELaserType.Instant;
        this.styleName = styleName;
        this.speed = speed;
        this.position = pos;
        this.direction = angle;
        this.length = length;
        this.width = width;
        this.color = color;


        this.currentLength = 0f;

        state = ELaserState.Spawning;
        timer = 0f;
        grazeCooldown = 0f;
        isAcive = true;
    }

    public void ActiveWarning(string styleName, Vector3 pos, float duration, float angle,
        float length, float width, Vector3 color)
    {
        this.type = ELaserType.Warning;
        this.styleName = styleName;
        this.speed = EBulletManager.Instance.warningSpeed;
        this.duration = duration;
        this.position = pos;
        this.direction = angle;
        this.length = length;
        this.fullWidth = width;
        this.width = 0f;
        this.color = color;

        this.currentLength = 0f;

        state = ELaserState.Spawning;
        timer = 0f;
        grazeCooldown = 0f;
        isAcive = true;
    }


    public void Move(float dt)
    {
        position += velocity * dt;
    }

    public void Clear()
    {
        state = ELaserState.Dead;
        timer = 0f;

        currentLength = 0f;
        width = 0f;
        fullWidth = 0f;
        speed = 0f;
        duration = 0f;

        grazeCooldown = 0f;

        styleName = null;
        isAcive = false;
    }

    public float Distance(Vector2 pos, float r)
    {
        Vector2 location = new Vector2(position.x, position.y);
        Vector2 d = Dir2;
        Vector2 rel = pos - location;
        float t = Vector2.Dot(rel, d);
        if (t < currentLength * 0.1f || t > currentLength * 0.9f) return -1f;
        Vector2 closest = location + d * t;
        float sqrDist = (pos - closest).sqrMagnitude;
        return sqrDist <= r * r ? t : -1f;

    }
}

