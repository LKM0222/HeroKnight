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
    [SerializeField] protected float atkRange;

    [Header("Refrence")]
    [SerializeField] CircleCollider2D findRangeCollider;

    [Header("Time")]
    [SerializeField] protected float waitTime;
    [SerializeField] protected float findTime;
    [SerializeField] protected float atkCooltime;

    protected Coroutine nowStateCoroutine;

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
    }

    void Start()
    {
        findRangeCollider.radius = findRange;
        findRangeCollider.isTrigger = true;
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
}
