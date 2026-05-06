using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ELaserType : byte { Instant, Warning }

public enum ELaserState : byte { Spawning, Normal, Dying, Dead }

public class ELaserData : EBullet<ELaserData>
{
    public ELaserType type;
    public bool isActive;

    public string styleName;
    public Vector3 color;

    public ELaserState state;

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


    public ELaserData SetAppearance(string styleName, Vector3 color)
    {
        this.styleName = styleName;
        this.color = color;
        return this;
    }
    public ELaserData SetInstant() { type = ELaserType.Instant; return this; }
    public ELaserData SetWarning() { type = ELaserType.Warning; return this; }
    public ELaserData SetDuration(float time) { duration = time; return this; }
    public ELaserData SetArea(float length, float width)
    {
        if (type == ELaserType.Instant)
        {
            this.width = width;
        }
        else if (type == ELaserType.Warning)
        {
            this.fullWidth = width;
        }

        this.length = length;
        return this;
    }

    public void Active()
    {
        this.currentLength = 0f;
        state = ELaserState.Spawning;
        timer = 0f;
        grazeCooldown = 0f;
        isActive = true;
    }

    /*
    public void ActiveInstant(string styleName, Vector3 pos, float speed, float angle,
                       float length, float width, Vector3 color)
    {

        this.type = ELaserType.Instant;
        this.styleName = styleName;
        this.speed = speed;
        this.position = pos;
        this.direction = angle;
        this.rotation = angle;
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
        this.rotation = angle;
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
    */

    public void Clear()
    {
        ClearMoveParam();
        state = ELaserState.Dead;
        timer = 0f;

        currentLength = 0f;
        width = 0f;
        fullWidth = 0f;
        duration = 0f;

        grazeCooldown = 0f;

        styleName = null;
        isActive = false;
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

