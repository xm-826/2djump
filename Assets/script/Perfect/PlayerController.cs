using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    
    public enum PlayerState
    {
        Idle,  //等待
        Move,  //移动
        Jump,  //跳跃
        Fall,  //下落
        Land,  //着地
        Hurt,  //受伤
        Respawn,//复活
        Die    //死亡
    }

    [Header("玩家状态")]
    public PlayerState currentState; //现在状态
    private PlayerState lastState; //上一个状态
    [SerializeField] private Transform p1;

    [Header("玩家血量相关设置")]
    [SerializeField] public float blood = 1f;
    [SerializeField] private Slider slider;//血条
    private bool canBeHurt = true;
    [SerializeField] private float timecold = 5f;
    [SerializeField] public bool isRespwan = false;
    [SerializeField] private float hurtForce = 4.5f;

    [Header("玩家速度")]
    [SerializeField] public float moveSpeed = 7f;
    private bool moveDir = false;


    [Header("玩家跳跃设置")]
    [SerializeField] private float jumpForce = 9f;
    [SerializeField] private float jumpUp = 1f;
    [SerializeField] private float jumpDown = 2.2f;
    [SerializeField] private int jumpCount = 0;
    [SerializeField] private int jumpmax = 1;

    [Header("落地状态设置")]
    public float landDelay = 0.05f;
    private bool isLanding = false;
    private bool aniLand = false;

    [Header("射线检测设置")]
    [SerializeField] private float hitLine = 0.25f;
    [SerializeField] private LayerMask rayerCastLayer;
    private bool isGround = false;

    [SerializeField] private SaveController saveController;
    
    //水平方向记录
    private float input_x;

    
    private Rigidbody2D rb;

   
    private SpriteRenderer sr;

    
    private BoxCollider2D bc;

    
    private Animator ani;

    private void Start()
    {
        ani = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        currentState = PlayerState.Idle;
        lastState = currentState;
        
    }


    private void Update()
    {
        
        UpdateState();
        ExecuteStateBehavior();
    }

 
   
    private void UpdateState()
    {
        IsGround();

        if(currentState == PlayerState.Die)
        {
            if(isRespwan)
            {
                SwitchState(PlayerState.Respawn);
            }
            return;
        }

        if(currentState == PlayerState.Respawn )
        {
            return;
        }

        if (!canBeHurt && currentState == PlayerState.Hurt)
        {
            return;
        }

        if (currentState == PlayerState.Land && isLanding)
        {
            return;
        }

        if (blood<=0 && currentState != PlayerState.Die)
        {
            SwitchState(PlayerState.Die);
            return;
        }

       
       

        if (!isGround)
        {
            if(rb.velocity.y>0.1f)
            {
                SwitchState(PlayerState.Jump);
                return;
            }

            else if(rb.velocity.y<0)
            {
                SwitchState(PlayerState.Fall);
                return;
            }
        }

        else
        {
             if (currentState != PlayerState.Land && input_x == 0 && isGround&&(lastState == PlayerState.Jump ||lastState == PlayerState.Fall))
            {
                Debug.Log("input_x = "+input_x);
                SwitchState(PlayerState.Land);
                return;
            }


            if(Math.Abs(input_x)>0)
            {
                SwitchState(PlayerState.Move);
                return;

            }
            else
            {
                SwitchState(PlayerState.Idle);
                return;
            }
        }
        


    }

    //
    private void ExecuteStateBehavior()
    {
        switch (currentState)
        {
            case PlayerState.Die:
                HandleDie();
                break;
            case PlayerState.Hurt:
                HandleHurt();
                break;
            case PlayerState.Respawn:
                HandleRespawn();
                break;
            case PlayerState.Jump:
                HandleJump();
                break;
            case PlayerState.Fall:
                HandleFall();
                break;
            case PlayerState.Move:
                HandelMove();
                break;
            case PlayerState.Land:
                HandelLand();
                break;
            case PlayerState.Idle:
                HandleIdel();
                break;

        }      
          
    }



  
    private void SwitchState(PlayerState newPlayerState)
    {
        
        if (currentState == newPlayerState) return;
        
        OnStateExit(currentState);
        OnStateEnter(newPlayerState);

        
        lastState = currentState;
        currentState = newPlayerState;

        UpdateAnimatorByState();

    }

   
    private void OnStateEnter(PlayerState state)
    {
        switch(state)
        {
            case PlayerState.Die:
                Die();
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                bc.enabled = false;
                input_x = 0;
                break;

            case PlayerState.Jump:
                aniLand = false;
                if (isGround) jumpCount++;
                break;

            case PlayerState.Hurt:
                ani.ResetTrigger("Hurt");
                sr.color = new Color(1, 0.5f, 0.5f);
                break;

            case PlayerState.Land:
                Debug.Log("进入落地状态");
                rb.velocity = new Vector2(rb.velocity.x, 0);
                jumpCount = 0;
                isLanding = true;
                Invoke(nameof(EndLand), landDelay);
                break;

            case PlayerState.Respawn:               
                rb.isKinematic = false;                
                bc.enabled = true;
                rb.velocity = Vector2.zero;
                jumpCount = 0;
                
                break;

            case PlayerState.Idle:
                
                rb.velocity = Vector2.zero;
                break;

            
        }
    }

    //״̬
    private void OnStateExit(PlayerState state)
    {
        switch(state)
        {
            case PlayerState.Die:
                break;

            case PlayerState.Hurt:
                sr.color = Color.white;
                break;

            case PlayerState.Move:
                break;

            case PlayerState.Respawn:
                isRespwan = false;
                break;

            case PlayerState.Land:
                break;

            case PlayerState.Idle:
                aniLand = true;
                break;

        }


    }

   //动画的
    private void UpdateAnimatorByState()
    {
        ani.SetBool("isMove", currentState == PlayerState.Move);

       
        ani.SetBool("jump", currentState == PlayerState.Jump || currentState == PlayerState.Fall);

       
        if (currentState == PlayerState.Land )
        {
            //Debug.Log("进入落地状态");
            ani.SetTrigger("land");
        }

        if (currentState == PlayerState.Hurt && lastState != PlayerState.Hurt)
        {
            ani.SetTrigger("Hurt");
        } 

        if (currentState == PlayerState.Die && lastState != PlayerState.Die)
            ani.SetTrigger("Die");
    }


    private void HandleIdel()
    { 
        GetInput();
        IsGround();
        sr.flipX = moveDir;
    }

  
    private void HandelMove()
    {
        GetInput();
        IsGround();
        rb.velocity = new Vector2(input_x * moveSpeed, rb.velocity.y);
        if(input_x!=0) moveDir = input_x > 0;
        sr.flipX = input_x >= 0;
    }

    
    private void HandelLand()
    {
        GetInput();
        IsGround();
        if (input_x != 0) moveDir = input_x > 0;
        sr.flipX = moveDir;

    }

   private void EndLand()
   {
        isLanding = false;
        
        SwitchState(Mathf.Abs(input_x) > 0 ? PlayerState.Move : PlayerState.Idle);
   }

    
    private void HandleJump()
    {
        GetInput();
        IsGround();

        rb.velocity = new Vector2(input_x * moveSpeed, rb.velocity.y);
        if (input_x != 0) moveDir = input_x > 0;
        sr.flipX = moveDir;

        if (Input.GetKey(KeyCode.Space))
        {
            rb.gravityScale = jumpUp;
        }
        else
        {
            rb.gravityScale = jumpDown;
        }
    }

    private void HandleFall()
    {
        GetInput();
        IsGround();
  
        rb.velocity = new Vector2(input_x * moveSpeed, rb.velocity.y);
        if (input_x != 0) moveDir = input_x > 0;
        sr.flipX = moveDir;

        rb.gravityScale = jumpDown;
    }

   
    private void HandleHurt()
    {
        input_x = 0;
    }
  
    private void HandleDie()
    {
        
    }
    
    private void HandleRespawn()
    {
        Respawn();

    }

    //===========函数实现===========
  
    private void GetInput()
    {
        
        if (currentState == PlayerState.Hurt || !canBeHurt)
        {
            input_x = 0;
            return;
        }

        input_x = Input.GetAxisRaw("Horizontal");
        
        if (isGround && Input.GetKeyDown(KeyCode.Space) && jumpCount < jumpmax)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    
    private void IsGround()
{
    // 方案：发射3条射线（左、中、右），任意一条命中即判定为地面
    Vector2 center = (Vector2)bc.bounds.center - new Vector2(0, bc.bounds.extents.y);
    Vector2 left = center - new Vector2(bc.bounds.extents.x - 0.1f, 0);
    Vector2 right = center + new Vector2(bc.bounds.extents.x - 0.1f, 0);

    RaycastHit2D hitCenter = Physics2D.Raycast(center, Vector2.down, hitLine, rayerCastLayer);
    RaycastHit2D hitLeft = Physics2D.Raycast(left, Vector2.down, hitLine, rayerCastLayer);
    RaycastHit2D hitRight = Physics2D.Raycast(right, Vector2.down, hitLine, rayerCastLayer);

    isGround = hitCenter.collider != null || hitLeft.collider != null || hitRight.collider != null;
    
    // 绘制调试射线
    Debug.DrawRay(center, Vector2.down * hitLine, isGround ? Color.green : Color.red);
    Debug.DrawRay(left, Vector2.down * hitLine, Color.blue);
    Debug.DrawRay(right, Vector2.down * hitLine, Color.yellow);

    if (isGround) jumpCount = 0;
}
    
  
    public void Hurt(float damage , Transform attack)
    {
        if (!canBeHurt  || currentState == PlayerState.Die) return;

        setBlood(-damage);
        
        float dir = transform.position.x > attack.position.x ? 1 : -1;

        Vector2 forceHurtDir = new Vector2(dir * hurtForce, 1f);
        input_x = 0;

        canBeHurt = false;
        rb.AddForce(forceHurtDir , ForceMode2D.Impulse);
  
        SwitchState(PlayerState.Hurt);
        //受击冷却
        StartCoroutine(TimeCold());

    }

  
    

    
    public void Die()
    {
        if (UIController.instance != null)
        {
            UIController.instance.DieYes();
        }
        else
        {
            Debug.Log("未找到uIController");
        }
    }

  
    /// <summary>
    /// 复活：重置血量 → 从存档读取位置并传送 → 切回 Idle 状态。
    /// 由 HandleRespawn() 调用，触发链：死亡 UI "Try Again" → IsRespawn() → UpdateState → Respawn()
    /// </summary>
    public void Respawn()
    {
        // 1. 重置血量
        blood = 1f;
        if (slider != null) slider.value = blood;

        // 2. 读取最近的存档点，传送到指定出生位置
        if (saveController != null)
        {
            saveController.Load();
        }
        else
        {
            Debug.LogWarning("[Respawn] 场景中没有找到 SaveController，玩家位置传送到初始位置");
        }

        // 3. 恢复可受伤状态，回到 Idle
        canBeHurt = true;
        isRespwan = false;
        SwitchState(PlayerState.Idle);
    }
   
    public void IsRespawn()
    {
        Debug.Log("复活");
        isRespwan = true;
        
    }


    private IEnumerator TimeCold()
    {
        yield return new WaitForSeconds(timecold);
        canBeHurt = true;
        // 只恢复可受伤标志，不强制切状态——下一帧 UpdateState 会根据当前情况自行决定
    }

    //血量变化
    public void setBlood(float value)
    {
        blood = Mathf.Clamp(blood + value, 0, 1);
        if (slider != null)
            slider.value = blood;
    }

    //速度变化
    public void setSpeed(float multiplier,float timecoldSpeed)
    {
        Debug.Log("已进入加速，倍率：" + multiplier);
        moveSpeed *= multiplier;
        StartCoroutine(TimeSpeedCOld(multiplier,timecoldSpeed));
    }
    //速度变化时间
    private IEnumerator TimeSpeedCOld(float multiplier,float timecoldSpeed)
    {
        yield return new WaitForSeconds(timecoldSpeed);
        moveSpeed /= multiplier;
        Debug.Log("已退出加速");
    }


}


    