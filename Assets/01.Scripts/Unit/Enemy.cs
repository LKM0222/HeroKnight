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


    [Header("Refrence")]
    [SerializeField] CircleCollider2D findRangeCollider;

    [Header("Time")]
    [SerializeField] protected float waitTime;
    [SerializeField] protected float findTime;
    [SerializeField] protected float atkCooltime;

    protected Coroutine nowStateCoroutine;

    protected Vector2 toTarget => target == null ? Vector2.zero : (Vector2)(target.transform.position - transform.position);
    protected float dist => Mathf.Abs(toTarget.x);


    // 간단하게 Idle -> Chase -> Attack 이렇게만 만들면 됨.
    protected abstract void StateManagement(EnemyState state);

    protected virtual IEnumerator IdelCoroutine() { yield return null; }
    protected virtual IEnumerator FindCoroutine() { yield return null; }
    protected virtual IEnumerator ChaseCoroutine() { yield return null; }
    protected virtual IEnumerator AttackCoroutine() { yield return null; }
    protected virtual IEnumerator DeathCoroutine() { yield return null; }

    public virtual void Hit(float dmg) // 이건 외부에서 공격할 때 호출해야되니깐 public으로 설정
    {
        hp -= dmg;
        float hpRatio = hp / maxHP;
        hpBar.SetHPBar(hpRatio);
    }

    protected override void Start()
    {
        base.Start();

        float hpRatio = hp / maxHP;
        hpBar.SetHPBar(hpRatio);

        findRangeCollider.radius = findRange;
        findRangeCollider.isTrigger = true;

        gameObject.layer = LayerMask.NameToLayer("Enemy");
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(7))
        {
            var player = collision.GetComponent<Player>();
            if (player != null)
            {
                target = player;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(7))
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
}
