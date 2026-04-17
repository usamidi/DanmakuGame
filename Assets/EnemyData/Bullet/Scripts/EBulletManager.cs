using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;


public enum EBulletState : byte { Spawning, Normal, ReachBound, Dying, Dead }

public enum EBBoundType : byte
{
    None,
    Top,
    Bottom,
    Left,
    Right,
}

public class EBulletData
{
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
    /*
  {
      get => velocity.magnitude;
      set
      {
          if (velocity.sqrMagnitude > 0f)
              velocity = velocity.normalized * value;
      }
  }
  */


    public float rotation;
    public float spawnDuration = 0.3f;
    public float dieDuration = 0.3f;

    public EBulletState state;
    public EBBoundType boundType = EBBoundType.None;
    public float timer = 0f;

    public bool isGrazed = false;

    public int reflectTimes = 0;

    public void Active(Vector3 startPos, float speed, float degree, int reflects = 0)
    {
        this.speed = speed;
        direction = degree;
        //float angleRad = degree * Mathf.Deg2Rad;
        //Vector3 dir = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0);
        //velocity = dir * speed;

        position = startPos;
        rotation = degree;

        state = EBulletState.Spawning;

        reflectTimes = reflects;
    }

    public void SetDirection(float degree)
    {
        direction = degree;
        rotation = degree;
    }

    public void Clear()
    {
        state = EBulletState.Dead;
        boundType = EBBoundType.None;
        timer = 0f;
        spawnDuration = 0.3f;
        dieDuration = 0.3f;
        isGrazed = false;
        reflectTimes = 0;
    }


}



public struct EBulletAppearance
{
    public int style;
    public Vector3 color;

    public EBulletAppearance(int s, Vector3 clr)
    {
        style = s;
        color = clr;
    }
}


public class EBulletBatch
{
    // 计时器
    public List<EBulletData> bulletBatch;

    public EBulletBatch()
    {
        bulletBatch = new List<EBulletData>();
    }

    public int BulletNum()
    {
        return bulletBatch.Count;
    }

    public EBulletBatch AddBullet(Vector3 startPos, float speed, float degree, int reflects = 0)
    {
        EBulletData bullet = EBulletManager.Instance.GetBullet();
        bullet.Active(startPos, speed, degree, reflects);
        bulletBatch.Add(bullet);
        return this;
    }

    public EBulletRenderBatch Packed(int styleIndex, Vector3 color)
    {
        if (BulletNum() == 0) return null;
        if (BulletNum() > 1023) return null;

        EBulletRenderBatch renderBatch = EBulletManager.Instance.GetRenderBatch();

        renderBatch.bullets = bulletBatch;
        renderBatch.appearance.color = color;
        renderBatch.appearance.style = styleIndex;

        return renderBatch;
    }

}

public partial class EBulletManager : MonoBehaviour
{
    public static EBulletManager Instance;


    [Header("=== 注册所有子弹类型 ===")]
    [Tooltip("将你创建的所有 BulletStyle ScriptableObject 拖到这个数组里")]
    public EBulletStyle[] registeredStyles;

    [Header("=== 擦弹计数器 ===")]
    public uint grazeNum = 0;
    [SerializeField] private GrazeUI grazeUI;

    [Header("=== 被弹计数器 ===")]
    public uint missNum = 0;
    [SerializeField] private MissUI missUI;

    [Header("=== 全局与碰撞设置 ===")]
    public Player player;
    public Transform playerTransform;
    public float playerHitboxRadius = 0.05f;
    public float playerGrazeRadius = 0.50f;

    public Vector2 boundsMin = new Vector2(-15, -15);
    public Vector2 boundsMax = new Vector2(15, 15);

    [Header("=== 粒子效果 ===")]
    public ParticleSystem destroyVFX;
    public ParticleSystem grazeVFX;



    public void SpawnBullet(EBulletRenderBatch batch)
    {
        //if (bulletBatch.BulletNum() == 0) return false;
        //if (bulletBatch.BulletNum() > 1023) return false;
        batch.isActive = true;

        //EBulletRenderBatch renderBatch = pool.Get();

        //renderBatch.bullets = bulletBatch.bulletBatch;
        //renderBatch.appearance.color = color;
        //renderBatch.appearance.style = styleIndex;
        // renderBatch.onUpdate = callBack;

        //renderBatches.Add(renderBatch);
    }

    public EBulletStyle GetBulletStyle(int index)
    {
        return registeredStyles[index];

    }

    void Awake()
    {
        Instance = this;
        initObjectPool();
    }

    // Start is called before the first frame update
    void Start()
    {
        //EBulletBatch batch1 = new EBulletBatch(new Vector3(255f, 10f, 10f));
        EBulletBatch batch1 = new EBulletBatch();
        batch1.AddBullet(new Vector3(0f, 0f, 0f), 1f, 90f);
        SpawnBullet(batch1.Packed(0, new Vector3(255, 255, 0)));

        // UI init
        grazeUI.SetGraze(grazeNum);
    }

    // Update is called once per frame
    void Update()
    {
        renderBullet();

        // 测试输入
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float angle1 = Random.Range(0f, 360f);
            float angle2 = Random.Range(0f, 360f);
            //EBulletBatch batch2 = new EBulletBatch(new Vector3(255f, 10f, 10f));
            EBulletBatch batch2 = new EBulletBatch();
            batch2.AddBullet(
                new Vector3(-1f, 0f, 0f), 1f, angle1
            ).AddBullet(
                new Vector3(1f, 0f, 0f), 1f, angle2
            );

            SpawnBullet(batch2.Packed(0, new Vector3(255, 0, 255)));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            //Debug.Log(renderBatches);
            Debug.Log($"renderBatches.Count: {renderBatches.Count}");
            for (int i = 0; i < renderBatches.Count; i++)
            {
                //Debug.Log($"renderBatches[{i}].bullets.Count: {renderBatches[i].bullets.Count}");

            }
        }
    }

    public static float GetAngleToPlayer(Vector3 position)
    {
        Player player = EBulletManager.Instance.player;
        if (player == null) return 270f; // 如果玩家死了或不存在，默认向下射击 (270度)

        Vector2 dir = player.transform.position - position;
        // Mathf.Atan2 返回的是弧度，需要乘以 Mathf.Rad2Deg 转换为度数
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle;
    }

    public static float GetAngleToPosition(Vector3 position, Vector3 destination)
    {
        Vector2 dir = destination - position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle;
    }

}
