using System;
using System.Collections;
using UnityEngine;

[Serializable] public enum EnemyState {Idle, Find, Chase, Attack, Death}

public abstract class Enemy : Unit
{

    [Header("Data")]
    [SerializeField] protected Player target;
    [SerializeField] protected Vector2 moveDir;
    [SerializeField] protected float findRange;
    [SerializeField] public EnemyType enemyType;
    [SerializeField] float knockbackDistance = 1f;
    [SerializeField] float knockbackDuration = 0.25f;
    [SerializeField] bool isKnockback;

    [Header("Refrence")]
    [SerializeField] protected BoxCollider2D findRangeCollider;
    [SerializeField] BoxCollider2D objectCollider;
    [SerializeField] ParticleSystem hitParticle;

    [Header("Time")]
    [SerializeField] protected float waitTime;
    [SerializeField] protected float findTime;
    [SerializeField] protected float atkCooltime;

    protected Coroutine nowStateCoroutine;
    Coroutine knockbackCoroutine;

    protected Vector2 toTarget => target == null ? Vector2.zero : (Vector2)(target.transform.position - transform.position);
    protected float dist => Mathf.Abs(toTarget.x);


    // 간단하게 Idle -> Chase -> Attack 이렇게만 만들면 됨.
    protected abstract void StateManagement(EnemyState state);

    protected virtual IEnumerator IdelCoroutine() { yield return null; }
    protected virtual IEnumerator FindCoroutine() { yield return null; }
    protected virtual IEnumerator ChaseCoroutine() { yield return null; }
    protected virtual IEnumerator AttackCoroutine() { yield return null; }
    protected virtual IEnumerator DeathCoroutine() { yield return null; StageManager.Instance.EnemyDead(this); SoundManager.Instance.PlaySound(SoundType.Death); }

    public virtual void Hit(float dmg) // 이건 외부에서 공격할 때 호출해야되니깐 public으로 설정
    {
        hp -= dmg;
        hpBar.SetHPBar(hp, maxHP);
        hitParticle.Emit(10);
        GameManager.Instance.SetComboUI();
        SoundManager.Instance.PlaySound(SoundType.Hit);
    }

    protected override void Start()
    {
        base.Start();

        hpBar.SetHPBar(hp, maxHP);

        findRangeCollider.size = new Vector2(findRange, objectCollider.size.y * 2f);
        findRangeCollider.offset = new Vector2(
            findRange * 0.5f,
            objectCollider.offset.y + objectCollider.size.y * 0.5f
        );
        findRangeCollider.isTrigger = true;

        gameObject.layer = LayerMask.NameToLayer("Enemy");

        hitParticle.gameObject.SetActive(true);
    }


    protected void ChangeCoroutine(Coroutine stateCoroutine)
    {
        if (nowStateCoroutine != null)
        {
            StopCoroutine(nowStateCoroutine);
            nowStateCoroutine = null;
        }

        nowStateCoroutine = stateCoroutine;
    }

    protected virtual void Move()
    {
        if (isKnockback) return;
        transform.Translate(moveDir * Time.deltaTime * moveSpeed);
    }

    public virtual void Attack()
    {
        if (target == null) return;
        if (!target.gameObject.layer.Equals(LayerMask.NameToLayer("Player")))
        {
            target = null;
            return;
        }
        target.Hit(atk);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(7))
        {
            var player = collision.GetComponent<Player>();
            if (player != null)
            {
                target = player;
            }
            Debug.Log("Player");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(7) || collision.gameObject.layer.Equals(11) || collision.gameObject.layer.Equals(10))
        {
            var player = collision.GetComponent<Player>();
            if (target == player)
            {
                target = null;
            }
        }
    }

    // debug
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, atkRange);
    }

    protected bool CanAttack()
    {
        return dist <= atkRange;
    }

    protected void SetMoveDir()
    {
        moveDir = dist > 0.0001f ? new Vector2(Mathf.Sign(toTarget.x), 0f) : Vector2.zero;
    }

    public void EnemyKnockback(Player target)
    {
        if (target == null || isKnockback) return;
        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);
        knockbackCoroutine = StartCoroutine(KnockbackRoutine(target));
    }

    // IEnumerator KnockbackRoutine(Player attacker)
    // {
    //     isKnockback = true;
    //     float sign = Mathf.Sign(transform.position.x - attacker.transform.position.x);
    //     if (Mathf.Approximately(sign, 0f)) sign = 1f;
    //     Vector3 start = transform.position;
    //     float elapsed = 0f;
    //     while (elapsed < knockbackDuration)
    //     {
    //         elapsed += Time.deltaTime;
    //         float t = Mathf.Clamp01(elapsed / knockbackDuration);
    //         float eased = 1f - (1f - t) * (1f - t);
    //         transform.position = start + new Vector3(sign * knockbackDistance * eased, 0f, 0f);
    //         yield return null;
    //     }
    //     transform.position = start + new Vector3(sign * knockbackDistance, 0f, 0f);
    //     isKnockback = false;
    //     knockbackCoroutine = null;
    // }
    IEnumerator KnockbackRoutine(Player attacker)
    {
        isKnockback = true;
        float sign = Mathf.Sign(transform.position.x - attacker.transform.position.x);
        if (Mathf.Approximately(sign, 0f)) sign = 1f;
        int wallMask = LayerMask.GetMask("Wall");
        const float skin = 0.02f;
        Vector3 start = transform.position;
        float elapsed = 0f;
        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / knockbackDuration);
            float eased = 1f - (1f - t) * (1f - t);
            Vector3 desired = start + new Vector3(sign * knockbackDistance * eased, 0f, 0f);
            float moveX = desired.x - transform.position.x;
            float dist = Mathf.Abs(moveX);
            if (dist > 1e-5f)
            {
                Vector2 dir = new Vector2(Mathf.Sign(moveX), 0f);
                Vector2 size = objectCollider.bounds.size;
                float angle = objectCollider.transform.eulerAngles.z;
                RaycastHit2D hit = Physics2D.BoxCast(
                    objectCollider.bounds.center,
                    size,
                    angle,
                    dir,
                    dist,
                    wallMask);
                if (hit.collider != null)
                {
                    float allowed = Mathf.Max(0f, hit.distance - skin);
                    transform.position += new Vector3(dir.x * allowed, 0f, 0f);
                    break;
                }
            }
            transform.position = desired;
            yield return null;
        }
        // 루프에서 break 없이 끝났을 때만 최종 목표 한 번 더(이미 desired와 같을 수 있음)
        {
            Vector3 desired = start + new Vector3(sign * knockbackDistance, 0f, 0f);
            float moveX = desired.x - transform.position.x;
            float dist = Mathf.Abs(moveX);
            if (dist > 1e-5f)
            {
                Vector2 dir = new Vector2(Mathf.Sign(moveX), 0f);
                RaycastHit2D hit = Physics2D.BoxCast(
                    objectCollider.bounds.center,
                    objectCollider.bounds.size,
                    objectCollider.transform.eulerAngles.z,
                    dir,
                    dist,
                    wallMask);
                if (hit.collider != null)
                {
                    float allowed = Mathf.Max(0f, hit.distance - skin);
                    transform.position += new Vector3(dir.x * allowed, 0f, 0f);
                }
                else
                    transform.position = desired;
            }
        }
        isKnockback = false;
        knockbackCoroutine = null;
    }
}
