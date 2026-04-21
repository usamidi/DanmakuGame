using System.Collections;
using System;
using UnityEngine;

public enum PlayerState
{
    Normal,
    Miss,
    Bomb,
    Invincible
}

public partial class Player : MonoBehaviour
{
    public static Player instance;

    [Header("移动设置")]
    [SerializeField] private float normalSpeed = 10f;
    [SerializeField] private float focusSpeed = 4f;
    [SerializeField] private Vector3 defaultPos;

    [Header("引用组件")]
    [SerializeField] private GameObject playerVisual; // 拖入刚才创建的子物体 Hitbox
    [SerializeField] private GameObject hitboxVisual; // 拖入刚才创建的子物体 Hitbox
    [SerializeField] private GameObject bulletPrefab; // 拖入你的子弹预制体
    [SerializeField] private Transform firePoint;     // 子弹发射位置

    [Header("射击设置")]
    [SerializeField] private float fireRate = 0.1f;    // 射击间隔
    [SerializeField] private float missTime = 0.1f;

    private float nextFireTime = 0f;

    private PlayerState state = PlayerState.Normal;
    private float invincibleTime;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    public ParticleSystem missVFX;

    //[Header("无敌闪烁设置")]
    //public  targetRenderer;
    //public float interval = 0.06f;
    private float invincibleDuration = 0.8f;
    private Coroutine missCoroutine = null;
    private Coroutine invincibleCoroutine = null;

    IEnumerator MissRoutine(float interval, float duration)
    {
        SpriteRenderer sr = playerVisual.GetComponent<SpriteRenderer>();

        if (missVFX != null)
        {
            ParticleSystem particle = Instantiate(missVFX, transform.position, Quaternion.identity);
        }

        float t = 0f;
        Color c = sr.color;
        while (t < duration)
        {
            c.a = 1f - (t / duration > 1f ? 1f : t / duration);
            sr.color = c;
            yield return new WaitForSeconds(interval);
            t += interval;
        }
        c.a = 1.0f;
        sr.color = c;
        invincibleDuration = 2.4f;
        state = PlayerState.Invincible;
        transform.position = defaultPos;
        missCoroutine = null;
    }

    IEnumerator InvincibleRoutine(float interval, float duration)
    {
        Renderer targetRenderer = playerVisual.GetComponent<Renderer>();
        float t = 0f;
        bool visible = true;
        while (t < duration)
        {
            visible = !visible;
            targetRenderer.enabled = visible;
            yield return new WaitForSeconds(interval);
            t += interval;
        }
        targetRenderer.enabled = true; // 恢复可见
        state = PlayerState.Normal;
        invincibleCoroutine = null;
    }

    public bool Missable()
    {
        return state == PlayerState.Normal;
    }

    public void PlayerMiss()
    {
        state = PlayerState.Miss;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        instance = this;
    }

    void Start()
    {
        CalculateCameraBounds();
    }

    void PlayerManager()
    {
        float dt = Time.deltaTime;

        switch (state)
        {
            case PlayerState.Normal:
                PlayerMove();
                break;
            case PlayerState.Miss:
                moveInput = Vector2.zero;
                if (missCoroutine == null)
                    missCoroutine = StartCoroutine(MissRoutine(0.01f, missTime));
                break;
            case PlayerState.Bomb:
                PlayerMove();
                break;
            case PlayerState.Invincible:
                PlayerMove();
                if (invincibleCoroutine == null)
                    invincibleCoroutine = StartCoroutine(InvincibleRoutine(0.06f, invincibleDuration));
                break;
        }
        PlayerFocus();
        PlayerShoot();
    }


    void PlayerMove()
    {
        // 1. 获取移动输入
        // 使用 GetAxisRaw 保证按键即走，松开即停，无惯性
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    void PlayerFocus()
    {
        // 2. 低速模式切换 (通常按住 LeftShift)
        bool isFocusing = Input.GetKey(KeyCode.LeftShift);
        hitboxVisual.SetActive(isFocusing); // 低速模式显示判定点
    }

    void PlayerShoot()
    {
        // 3. 射击逻辑 (通常按住 Z 或 Space)
        if (Input.GetKey(KeyCode.Z) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Update()
    {
        PlayerManager();

        // 4. 物理移动
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? focusSpeed : normalSpeed;
        Vector2 targetPosition = rb.position + moveInput * currentSpeed * Time.fixedDeltaTime;

        // 5. 边界限制 (防止飞机飞出屏幕)
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);

        rb.MovePosition(targetPosition);
    }

    void FixedUpdate()
    {
        // a
    }


    /// <summary>
    /// 计算相机视口边界，保证在任何分辨率下飞机都不会出屏
    /// </summary>
    void CalculateCameraBounds()
    {
        Camera mainCam = Camera.main;
        // 将屏幕左下角和右上角转换为世界坐标
        Vector3 bottomLeft = mainCam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = mainCam.ViewportToWorldPoint(new Vector3(1, 1, 0));

        // 这里的 0.5f 是预留的边距，防止飞机一半身体进墙
        float padding = 0.3f;
        minBounds = new Vector2(bottomLeft.x + padding, bottomLeft.y + padding);
        maxBounds = new Vector2(topRight.x - padding, topRight.y - padding);
    }
}
