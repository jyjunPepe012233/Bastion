using System;
using System.Collections;
using Bastion.EnemyAIStates;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GoToSupply, ChasePlayer, AttackPlayer, AttackSupply }

public abstract class Enemy : MonoBehaviour {
    
    [SerializeField] private int dieTime;
    [SerializeField] private float hitCoolDownTime;
    [SerializeField] private float attackCoolDownTime;
    [Header("Status")]
    [SerializeField] private int hp;
    [SerializeField] private GameObject attackCollider;
    [SerializeField] private int credit;
    [Space(5)]
    [SerializeField] private float detectDistance;
    [SerializeField] private float attackDistance;
    [Header("Info")]
    [SerializeField] private int curHp;
    [SerializeField] private bool noAi;


    private EnemyStates? currentState = null;
    private EnemyStates previousState;
    private EnemyState[] states;
    
    private Vector3 targetLastPos;
    private float hitCool;
    private bool isNotFirstSpawn;
    private bool isInAttack;

    private Animator animator;
    private NavMeshAgent agent;
    

    public NavMeshAgent Agent { get => agent; }
    public float DetectDistance { get => detectDistance; }
    public float AttackDistance { get => attackDistance; }
    public bool IsInAttack { get => isInAttack; }
    
    
    
    void Awake() {
        Create();
    }



    private void Create() { // 첫 생성
        isNotFirstSpawn = true;
        
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        
        states = new EnemyState[4];
        states[(int)EnemyStates.GoToSupply] = new GoToSupply();
        states[(int)EnemyStates.ChasePlayer] = new ChasePlayer();
        states[(int)EnemyStates.AttackPlayer] = new AttackPlayer();
        states[(int)EnemyStates.AttackSupply] = new AttackSupply();

        
        OnCreate();
    }



    public void Spawn() {
        if (isNotFirstSpawn) Create();
        
        curHp = hp;
        noAi = false;
        agent.isStopped = false;
        
        agent.enabled = false;
            // NavMesh의 고질적 버그 때문에 Agent를 비활성화해둠
            // 최초 업데이트에서 상태 설정과 함께 활성화함
    }
    
    
    
    private void Update() {
        if (noAi) return;

        if (!agent.enabled) {
            agent.enabled = true;
            StartCoroutine(ChangeState(EnemyStates.GoToSupply));
        } // 최초 업데이트에서 비활성화해둔 NavMesh Agent를 활성화함
        
        

        // 애니메이션 쿨타임
        if (hitCool > 0) hitCool -= Time.deltaTime;
        
        if (hitCool < 0) {
            hitCool = 0;
            agent.isStopped = false;
                // hitCool이 0 미만으로 내려가면 한번 작동
        }

        if (currentState != null) {
            
            states[(int)currentState].Execute(this);
        }
        
        
        
        OnUpdate();
    } 
    
    
    
    public void GetHit(float damage) {
        if (noAi) return;
        
        // 데미지
        curHp -= (int)damage;

        if (curHp <= 0) {
            curHp = 0; // Clamp
            StartCoroutine(Die());
            return;
        }


        // 애니메이션
        if (hitCool == 0) {
            agent.isStopped = true;
                // 피격 시 멈췄다가, hitCool이 0이 되면 다시 움직임
            
            animator.SetTrigger("GetHitTrigger");
            hitCool = hitCoolDownTime;
                // 재생된 피격 애니메이션의 길이만큼 기다림
        }
        
    }



    public IEnumerator Attack() {
        if (isInAttack) yield break;

        agent.isStopped = true;
        isInAttack = true;
        animator.SetTrigger("AttackTrigger");
        
        OnAttack();

        yield return new WaitForSeconds(attackCoolDownTime);
            // 재생된 공격 애니메이션의 길이만큼 기다림

        agent.isStopped = false;
        isInAttack = false;
    }


    public IEnumerator ActiveDamageCollision() {

        attackCollider.SetActive(true);
        yield return null;
        attackCollider.SetActive(false);
        
    }




    private IEnumerator Die() {
        animator.SetTrigger("DieTrigger");
        KillAI();
        
        yield return new WaitForSeconds(dieTime);
        
        OnDie();
        gameObject.SetActive(false);
        GameManager.Instance.GetCredit(credit);
    }



    IEnumerator ChangeState(EnemyStates newState) {
        
        if (currentState != null)
            previousState = (EnemyStates)currentState;
        
        currentState = null;
            // Execute 등등을 작동할 수 없게 함

        if (previousState != null)
            yield return states[(int)previousState].Exit(this);   
            // 이전 상태를 종료할때까지 기다림
            
        yield return states[(int)newState].Enter(this);
            // 새로운 상태로 들어갈때까지 기다림

        currentState = newState;
            // 상태 진입이 완료되면 Execute가 작동할 수 있음
    }


    public void StartChangeState(EnemyStates newState) {
        StartCoroutine(ChangeState(newState));
    }

    public void RevertToPreviousState() {
        StartCoroutine(ChangeState(previousState));
    }
    
    
    
    
    
    protected abstract void OnCreate();

    protected abstract void OnUpdate();

    protected abstract void OnAttack();
    
    protected abstract void OnDie();



    private void KillAI() {

        agent.isStopped = true;
        noAi = true;
    }


    private void OnDrawGizmos() {

        if (agent != null) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, agent.destination);
            
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
        }

        if (GameManager.Instance.IsGaming) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDistance);
            Gizmos.DrawSphere(GameManager.Instance.PlayerPosition, 0.3f);
        }

        if (currentState != null) {
            for (int i = 0; i < (int)currentState; i++) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(transform.position + Vector3.up * i, 0.3f);
            }
        }

    }
}
