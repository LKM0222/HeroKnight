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
    protected virtual IEnumerator DeathCoroutine() { yield return null; StageManager.Instance.EnemyDead(this); }

    public virtual void Hit(float dmg) // 이건 외부에서 공격할 때 호출해야되니깐 public으로 설정
    {
        hp -= dmg;
        float hpRatio = hp / maxHP;
        hpBar.SetHPBar(hpRatio);
        hitParticle.Emit(10);
    }

    protected override void Start()
    {
        base.Start();

        float hpRatio = hp / maxHP;
        hpBar.SetHPBar(hpRatio);

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

    IEnumerator KnockbackRoutine(Player attacker)
    {
        isKnockback = true;
        float sign = Mathf.Sign(transform.position.x - attacker.transform.position.x);
        if (Mathf.Approximately(sign, 0f)) sign = 1f;
        Vector3 start = transform.position;
        float elapsed = 0f;
        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / knockbackDuration);
            float eased = 1f - (1f - t) * (1f - t);
            transform.position = start + new Vector3(sign * knockbackDistance * eased, 0f, 0f);
            yield return null;
        }
        transform.position = start + new Vector3(sign * knockbackDistance, 0f, 0f);
        isKnockback = false;
        knockbackCoroutine = null;
    }
}
