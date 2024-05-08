using System.Collections;
using UnityEngine;
using UnityEngine.AI;  // ���ύX1

public class EnemyController : MonoBehaviour
{

    [SerializeField]
    Animator animator = null;
    [SerializeField]
    NavMeshAgent navmeshAgent = null;   // ���ύX1
    [SerializeField]
    Transform target = null;   // ���ύX1
    [SerializeField]
    CapsuleCollider capsuleCollider = null;
    [SerializeField, Min(0)]
    int maxHp = 3;
    [SerializeField]
    float deadWaitTime = 3;

    // ���ύX�Q
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

    // �A�j���[�^�[�̃p�����[�^�[��ID���擾�i�������̂��߁j
    readonly int SpeedHash = Animator.StringToHash("Speed");
    readonly int AttackHash = Animator.StringToHash("Attack");
    readonly int DeadHash = Animator.StringToHash("Dead");

    bool isDead = false;
    int hp = 0;
    Transform thisTransform;
    // ���ύX�Q
    bool isAttacking = false;
    Transform player;
    Transform defaultTarget;
    WaitForSeconds attackWait;
    WaitForSeconds attackIntervalWait;

    // ���ύX4
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

    // ���ύX3
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

    // ���ύX4
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
        target = GameObject.Find("Cube").transform; //�����̖��O�ς��ă^�[�Q�b�g�ύX
        thisTransform = transform;  // transform���L���b�V���i�������j

        // ���ύX2
        defaultTarget = target;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // ���ύX2�BWaitForSeconds���L���b�V�����č�����
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
        CheckDistance();  // ���ύX2
        Move(); // ���ύX1
        UpdateAnimator();
    }

    void InitEnemy()
    {
        Hp = maxHp;
    }

    // ��_���[�W����
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

    // ���S���̏���
    void Dead()
    {
        isDead = true;
        capsuleCollider.enabled = false;
        animator.SetBool(DeadHash, true);

        // ���ύX2
        StopAttack();
        navmeshAgent.isStopped = true;

        // ���ύX4
        GameManager.Count++;

        StartCoroutine(nameof(DeadTimer));
    }

    // ���S���Ă��琔�b�ԑ҂���
    IEnumerator DeadTimer()
    {
        yield return new WaitForSeconds(deadWaitTime);

        Destroy(gameObject);
    }

    // ���ύX1
    void Move()
    {
        navmeshAgent.SetDestination(target.position);
    }

    // �A�j���[�^�[�̃A�b�v�f�[�g����
    void UpdateAnimator()
    {
        // ���ύX1
        animator.SetFloat(SpeedHash, navmeshAgent.desiredVelocity.magnitude);
    }

    // ���ύX2 �ȉ���ǉ�

    void CheckDistance()
    {
        // �v���C���[�܂ł̋����i��悳�ꂽ�l�j���擾
        // sqrMagnitude�͕������̌v�Z���s��Ȃ��̂ō����B�������r���邾���Ȃ炻������g���������ǂ�
        float diff = (player.position - thisTransform.position).sqrMagnitude;
        // �������r�B��r�Ώۂ���悷��̂�Y�ꂸ��
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