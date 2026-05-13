using System;
using System.Collections;
using UnityEngine;

public class Goblin : Enemy
{
    [Header("Refresnce")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] AnimatorController_Goblin anim;
    Vector2 findRangeOffsetBase => findRangeCollider.offset;

    protected override void Start()
    {
        base.Start();

        // FSM 만들어야됨.
        StateManagement(EnemyState.Idle);
    }

    protected override void StateManagement(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle: ChangeCoroutine(StartCoroutine(IdelCoroutine())); break;
            case EnemyState.Find: ChangeCoroutine(StartCoroutine(FindCoroutine())); break;
            case EnemyState.Chase: ChangeCoroutine(StartCoroutine(ChaseCoroutine())); break;
            case EnemyState.Attack: ChangeCoroutine(StartCoroutine(AttackCoroutine())); break;
            case EnemyState.Death: ChangeCoroutine(StartCoroutine(DeathCoroutine())); break;
        }
    }

    protected override IEnumerator IdelCoroutine()
    {
        // IdelState -> n초 대기 후, 플레이어가 감지되는지 확인
        // 기다리는동안 플레이어 찾음 -> Chase로 변경
        // 기다리는동안 못찾음 -> Find로 변경 후 플레이어 찾아다니기
        yield return null;

        waitTime = UnityEngine.Random.Range(1f, 3f);
        moveDir = Vector2.zero;
        anim.SetRunAnim(moveDir.x);

        float time = 0;

        while (time < waitTime)
        {
            if (target != null)
            {
                StateManagement(EnemyState.Chase);
                yield break;
            }

            var deltaTime = Time.deltaTime;
            time += deltaTime;

            yield return new WaitForSeconds(deltaTime);
        }

        StateManagement(EnemyState.Find);
    }

    protected override IEnumerator FindCoroutine()
    {
        // FindState -> 플레이어가 발견될때까지 탐색 
        // findTime초 단위로 탐색, (이후 Idle 진입 후(n초 대기) 방향 전환?)
        // 도중 플레이어를 만났다면 Chase로 전환
        yield return null;

        float time = 0;

        moveDir = (UnityEngine.Random.Range(0, 2) == 0) ? Vector2.left : Vector2.right;

        while (time < findTime)
        {
            if (target != null) // 플레이어를 만났다면
            {
                StateManagement(EnemyState.Chase);
                yield break;
            }
            else // 플레이어를 못찾았다면
            {
                Move(); // 방향으로 움직임
            }

            var deltaTime = Time.deltaTime;
            time += deltaTime;
            yield return new WaitForSeconds(deltaTime); // 괜찮나..?
        }

        // 시간 내에 못찾았다면 Idle로 진입
        StateManagement(EnemyState.Idle);
    }

    protected override IEnumerator ChaseCoroutine()
    {
        // ChaseState -> 감지 범위에 있다면 타겟을 쫒아감
        // 타겟이 범위에서 벗어날때까지 쫒아감
        // 타겟이 공격 범위에 들어온다면, 공격 실행
        // 벗어난다면 다시 Idle로 진입 (여기서 Chase로 돌아가야될수도)
        yield return null;
        while (target != null)
        {
            if (CanAttack())
            {
                moveDir = Vector2.zero;
                StateManagement(EnemyState.Attack);
                yield break;
            }
            
            SetMoveDir();
            Move();
            yield return null;
        }
        StateManagement(EnemyState.Idle);
    }

    protected override IEnumerator AttackCoroutine()
    {
        // AttackState -> 플레이어를 공격
        // 공격 범위에 들어왔을 때, 공격 사거리까지 타겟에 다가감 (이 부분을 대쉬로?)
        // 공격 사거리 타겟에 들어왔을 때, 공격하고, 쿨타임 대기
        // 만약, 공격범위에서 나간다면 다시 idle로 전환
        yield return null;

        while (target != null)
        {
            anim.SetAttackAnim(UnityEngine.Random.Range(0, 2)); // 애니메이션 안에 있는 이벤트 실행

            float time = 0f;
            while (time < atkCooltime)
            {
                if (target == null) break; // 만약 target이 사라졌다면 쿨타임 종료

                var deltaTime = Time.deltaTime;
                time += deltaTime;
                yield return new WaitForSeconds(deltaTime);
            }
            yield return new WaitForSeconds(0.0001f);
        }

        StateManagement(EnemyState.Idle);
    }

    protected override IEnumerator DeathCoroutine()
    {
        yield return base.DeathCoroutine();
        yield return null;
        anim.SetAnim(AnimType_Goblin.Death);
    }

    public override void Hit(float atk)
    {
        base.Hit(atk);

        if (hp <= 0f) StateManagement(EnemyState.Death);
        else anim.SetAnim(AnimType_Goblin.Hit);

        floatingText.SpawnText(TextType.Hit, atk.ToString());
    }

    public override void Attack()
    {
        base.Attack();
    }

    protected override void Move()
    {
        base.Move();
        anim.SetRunAnim(moveDir.x);
        float dir = anim.LookingDir();
        findRangeCollider.offset = new Vector2(
            Mathf.Abs(findRangeOffsetBase.x) * dir,
            findRangeOffsetBase.y
        );
    }
}
