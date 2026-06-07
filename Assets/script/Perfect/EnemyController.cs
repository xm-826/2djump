using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static PlayerController;

public class EnemyController : MonoBehaviour
{

    private enum EnemyState
    {
        Idle,
        Move,
        Run,
        Die,
        Turn
    }

    [SerializeField] private EnemyState currentState;
    private EnemyState lastState;
    private bool dieFlag = false;
    [SerializeField] private bool canMove = false;

   
    [Header("射线检测设置")]
    private bool canSee = false;
    [SerializeField] private LayerMask rayerCastLayer;
    [SerializeField] private float hitLineSee = 5f;
    [SerializeField] private float hitLineMove = 15f;

    [Header("敌人属性设置")]
    [SerializeField] private float dieTime = 0.2f;
    [SerializeField] private float danmage = 0.2f;
    [SerializeField] private float boundForce = 8f;
    [SerializeField] private float enemySpeed = 1.5f;
    [SerializeField] private float enemyFastSpeed = 5f;
    private float temp=0;
    private float exitTurnTime = -99f;

    [Header("敌人位移设置")]
    [SerializeField] private Transform p1;
    [SerializeField] private Transform p2; 
    [SerializeField] private float moveDir = 1f;
    
    [SerializeField] private Animator ani;

    [Header("道具配置")]
    [SerializeField] private GameObject[] itemPrefab;


    private BoxCollider2D bc;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();

        currentState = EnemyState.Move;
        UpdateAnimatorByState();

        lastState = currentState;
    }


    private void Update()
    {
        UpdateState();
        ExecuteStateBehavior();

    }


    private void UpdateState()
    {
        if (currentState != EnemyState.Die && dieFlag == true)
        {
            SwitchState(EnemyState.Die);
            return;
        }

        if(currentState == EnemyState.Die)
            return;

        if (currentState == EnemyState.Turn||currentState == EnemyState.Idle)
        {
            return;
        }

        float dist1 = Mathf.Abs(rb.position.x - p1.position.x);
        float dist2 = Mathf.Abs(rb.position.x - p2.position.x);
        if (currentState == EnemyState.Move && (dist1 < 0.1f || dist2 < 0.1f)
            && Time.time - exitTurnTime > 0.2f)
        {
            SwitchState(EnemyState.Idle);
            return;
        }


        if (currentState != EnemyState.Run && canSee)
        {
            SwitchState(EnemyState.Run);
            return;

        }

        if(canMove)
        {
            SwitchState(EnemyState.Move);
            return;
        }
           


        SwitchState(EnemyState.Idle);
        
         


    }

    private void SwitchState(EnemyState state)
    {
        if (currentState == state) return;

        lastState = currentState;

        OnExitState(currentState);
        OnEnterState(state);


        currentState = state;
        UpdateAnimatorByState();
    }

  
    private void UpdateAnimatorByState()
    {
        ani.SetBool("Move", currentState == EnemyState.Move);
        ani.SetBool("Idle", currentState == EnemyState.Idle);



    }

    private void OnExitState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Die:
                
                break;
            case EnemyState.Move:
                rb.velocity = new Vector2(0, rb.velocity.y);
                break;
            case EnemyState.Run:
                enemySpeed = temp;
                break;
            case EnemyState.Turn:
                break;

        }
    }


    private void OnEnterState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Die:
                Vector2 p1 = transform.position;
                GameObject picked = PickByWeight(itemPrefab);
                Instantiate(picked,p1,transform.rotation);
                break;
            case EnemyState.Move:
                break;
            case EnemyState.Run:
                temp = enemySpeed;
                enemySpeed = enemyFastSpeed;
                break;
            case EnemyState.Idle:
                rb.velocity = new Vector2(0, rb.velocity.y);
                StartCoroutine(IdleToTurn());
                break;
            case EnemyState.Turn:
                moveDir = -moveDir;
                sp.flipX = !sp.flipX;
                rb.velocity = Vector2.zero;
                StartCoroutine(TurnToMove());
                break;
        }
    }

    private void ExecuteStateBehavior()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdle();
                break;
            case EnemyState.Move:
                HandleMove();
                break;
            case EnemyState.Run:
                HandleRun();
                break;
            case EnemyState.Turn:
                HandleTurn();
                break;
            case EnemyState.Die:
                HandleDie();
                break;
        }
    }

    private void HandleTurn()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    IEnumerator IdleToTurn()
    {
        yield return new WaitForSeconds(0.5f);
        SwitchState(EnemyState.Turn);
    }

    IEnumerator TurnToMove()
    {
        yield return new WaitForSeconds(0.3f);
        exitTurnTime = Time.time;
        SwitchState(EnemyState.Move);
    }

    private void HandleDie()
    {
        GetComponent<Collider2D>().enabled = false;  
        
        Destroy(gameObject, dieTime);
    }

    private void HandleRun()
    {
        IsHit();
        
        rb.velocity = new Vector2(enemySpeed * moveDir, rb.velocity.y);
    }

    private void HandleMove()
    {
        IsHit();
        //Debug.Log("moveDir:"+moveDir);
        rb.velocity = new Vector2(enemySpeed * moveDir, rb.velocity.y);
    }

    private void HandleIdle()
    {
        IsHit();
    }

   
    private void IsHit()
    {
        Vector2 orginPlace = bc.bounds.center;
        float dir;
        if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            dir = Mathf.Sign(rb.velocity.x);
        }

        else
        {
            dir = sp.flipX ? 1 : -1;
        }
        RaycastHit2D hitSee = Physics2D.Raycast(orginPlace, new Vector2(dir, 0), hitLineSee, rayerCastLayer);
        canSee = hitSee.collider != null;
        Debug.DrawRay(orginPlace, new Vector2(dir, 0) * hitLineSee, canSee ? Color.green : Color.red);

        //RaycastHit2D hitMove = Physics2D.Raycast(orginPlace, new Vector2(dir, 0), hitLineMove, rayerCastLayer);
        //canMove = hitMove.collider != null;
       // Debug.DrawRay(orginPlace, new Vector2(dir, 0) * hitLineMove, canSee ? Color.green : Color.red);


    }

   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();


            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();


            //print(rb.velocity.y);
            bool isPlayerOnTop = CheckPlayerOnTop(collision);


            if (playerRb != null && isPlayerOnTop)
            {
                dieFlag = true;
                playerRb.velocity = new Vector2(playerRb.velocity.x, boundForce);
                return;
            }

            
            if (player != null)
            {
                
                player.Hurt(danmage, transform);
            }
        }
    }

    
    private bool CheckPlayerOnTop(Collision2D collision)
    {
       
        Collider2D playerCol = collision.collider;
       
        float playerBottom = playerCol.bounds.min.y;
   
        float enemyTop = bc.bounds.max.y;

        return playerBottom >= enemyTop - 0.1f;
    }

    
    //随机获取预制件
    private GameObject PickByWeight(GameObject[] prefabs)
    {
        float total = 0;
        foreach(var p in prefabs)
        {
            //获取预制件中的item
            var item = p.GetComponent<Item>();
            if(item!=null&&item.itemSos !=null)
                total += item.itemSos.dropWeight;
        }

        //Roll点
        float roll = UnityEngine.Random.Range(0,total);

        //查看处于什么区间
        float accumulate = 0;
        foreach (var p in prefabs)
        {
            var item = p.GetComponent<Item>();
            if(item!=null&&item.itemSos !=null)
            {
                accumulate += item.itemSos.dropWeight;
                if(roll <= accumulate)
                {
                    return p;
                }
            }
        }
    
        return prefabs[0];
    }
}
