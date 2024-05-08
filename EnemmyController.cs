using System.Collections;
using UnityEngine;
using UnityEngine.AI;  // ★変更1

public class EnemyController : MonoBehaviour
{

    [SerializeField]
    Animator animator = null;
    [SerializeField]
    NavMeshAgent navmeshAgent = null;   // ★変更1
    [SerializeField]
    Transform target = null;   // ★変更1
    [SerializeField]
    CapsuleCollider capsuleCollider = null;
    [SerializeField, Min(0)]
    int maxHp = 3;
    [SerializeField]
    float deadWaitTime = 3;

    // ★変更２
    [SerializeField]
    float chaseDistance = 5;
    [SerializeField]
    Collider attackCollider = null;
    [SerializeField]
    int attackPower = 10;
    [SerializeField]
    float attackTime = 0.5f;
    [SerializeField]
    float attackInterval = 2;
    [SerializeField]
    float attackDistance = 2;

    // アニメーターのパラメーターのIDを取得（高速化のため）
    readonly int SpeedHash = Animator.StringToHash("Speed");
    readonly int AttackHash = Animator.StringToHash("Attack");
    readonly int DeadHash = Animator.StringToHash("Dead");

    bool isDead = false;
    int hp = 0;
    Transform thisTransform;
    // ★変更２
    bool isAttacking = false;
    Transform player;
    Transform defaultTarget;
    WaitForSeconds attackWait;
    WaitForSeconds attackIntervalWait;

    // ★変更4
    GameManager gameManager;

    public int Hp
    {
        set
        {
            hp = Mathf.Clamp(value, 0, maxHp);
        }
        get
        {
            return hp;
        }
    }

    // ★変更3
    public Transform Target
    {
        set
        {
            target = value;
        }
        get
        {
            return target;
        }
    }

    // ★変更4
    GameManager GameManager
    {
        get
        {
            if (gameManager == null)
            {
                gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            }
            return gameManager;
        }
    }

    void Start()
    {
        target = GameObject.Find("Cube").transform; //ここの名前変えてターゲット変更
        thisTransform = transform;  // transformをキャッシュ（高速化）

        // ★変更2
        defaultTarget = target;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // ★変更2。WaitForSecondsをキャッシュして高速化
        attackWait = new WaitForSeconds(attackTime);
        attackIntervalWait = new WaitForSeconds(attackInterval);

        InitEnemy();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        CheckDistance();  // ★変更2
        Move(); // ★変更1
        UpdateAnimator();
    }

    void InitEnemy()
    {
        Hp = maxHp;
    }

    // 被ダメージ処理
    public void Damage(int value)
    {
        if (value <= 0)
        {
            return;
        }

        Hp -= value;

        if (Hp <= 0)
        {
            Dead();
        }
    }

    // 死亡時の処理
    void Dead()
    {
        isDead = true;
        capsuleCollider.enabled = false;
        animator.SetBool(DeadHash, true);

        // ★変更2
        StopAttack();
        navmeshAgent.isStopped = true;

        // ★変更4
        GameManager.Count++;

        StartCoroutine(nameof(DeadTimer));
    }

    // 死亡してから数秒間待つ処理
    IEnumerator DeadTimer()
    {
        yield return new WaitForSeconds(deadWaitTime);

        Destroy(gameObject);
    }

    // ★変更1
    void Move()
    {
        navmeshAgent.SetDestination(target.position);
    }

    // アニメーターのアップデート処理
    void UpdateAnimator()
    {
        // ★変更1
        animator.SetFloat(SpeedHash, navmeshAgent.desiredVelocity.magnitude);
    }

    // ★変更2 以下を追加

    void CheckDistance()
    {
        // プレイヤーまでの距離（二乗された値）を取得
        // sqrMagnitudeは平方根の計算を行わないので高速。距離を比較するだけならそちらを使った方が良い
        float diff = (player.position - thisTransform.position).sqrMagnitude;
        // 距離を比較。比較対象も二乗するのを忘れずに
        if (diff < attackDistance * attackDistance)
        {
            if (!isAttacking)
            {
                StartCoroutine(nameof(Attack));
            }
        }
        else if (diff < chaseDistance * chaseDistance)
        {
            target = player;
        }
        else
        {
            target = defaultTarget;
        }
    }
    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger(AttackHash);
        attackCollider.enabled = true;
        yield return attackWait;
        attackCollider.enabled = false;
        yield return attackIntervalWait;
        isAttacking = false;
    }
    void StopAttack()
    {
        StopCoroutine(nameof(Attack));
        attackCollider.enabled = false;
        isAttacking = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FpsGunControler gun = other.gameObject.GetComponent<FpsGunControler>();
            if (gun != null)
            {
                gun.CurrentAmmo -= attackPower;
            }
        }
    }

}