using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Unit
{
    [Header("Refrence")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] AnimatorController_Player anim;
    [SerializeField] ParticleSystem healParticle;
    [SerializeField] DownSmashCollider downSmashCollider;
    [SerializeField] ParticleSystem downSmashParticle;
    public SkillController skillController;

    [Header("Base Data")]
    [SerializeField] Vector2 inputVec2;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] Enemy target;
    [SerializeField] Vector2 attackOffset; // 공격 레이 쏘는 오프셋
    [SerializeField] private float maxMana;
    [SerializeField] private float mana;
    [SerializeField] private float recoveryManaAmount;
    [SerializeField] int jumpCount = 0;
    [SerializeField] int maxJumpCount = 2;

    [Header("Skill Data/Rush")]
    [SerializeField] Vector3 rushAttackOffset;
    [SerializeField] float rushDistance;
    [SerializeField] float rushStep = 0.1f;
    [SerializeField] float rushAtkDmg;

    [Header("Skill Data/Heal")]
    [SerializeField] float healAmount;

    [Header("Skill Data/Down Smash")]
    [SerializeField] float downSmashAtkDamage;
    [SerializeField] float downSmashAtkArea;

    [Header("Flag")]
    [SerializeField] bool isGrounded;
    [SerializeField] bool isRoll;
    [SerializeField] bool isBlocked;
    [SerializeField] bool isDead = false;
    [SerializeField] bool isRush;
    [SerializeField] bool isDownSmash;

    [Header("Cam")]
    [SerializeField] CinemachineImpulseSource impulseSource;

    public bool IsGrounded => isGrounded;
    public float AirSpeedY => rb.linearVelocityY;
    public bool IsRoll { get { return isRoll; } set { isRoll = value; } }
    public float MaxMana => maxMana;
    public float Mana => mana;
    public bool IsGameStart => StageManager.Instance == null ? false : StageManager.Instance.isGameStart;

    public string skillResultText(int useResult) => useResult == -1 ? "Cooltime" : "No Enough Mana";


    Coroutine rushCoroutine = null;
    Coroutine downSmashCoroutine = null;


    protected override void Start()
    {
        GaemStart();
    }

    public void GaemStart()
    {
        base.Start();

        gameObject.layer = LayerMask.NameToLayer("Player");
        //
        isDead = false;
        mana = 0;
    
        //
        healParticle.gameObject.SetActive(true);
        downSmashCollider.Init(this, downSmashAtkArea);
        downSmashParticle.gameObject.SetActive(true);

        //
        this.transform.position = StageManager.Instance.playerStartPos.position;
    }

    void Update()
    {
        Move();
        RecoveryMana();
    }

    void FixedUpdate()
    {
        CheckGround();
    }

    void Move()
    {
        if (isBlocked || isDead || isRush || isDownSmash || !IsGameStart) return;
        transform.Translate(moveDirection * Time.deltaTime * moveSpeed);
    }

    public void Hit(float atk)
    {
        if (isRoll || !IsGameStart) return;

        if (isBlocked)
        {
            anim.SetBlcokSuccessAnim();
            floatingText.SpawnText(TextType.Nomal, "Defenced");
            SoundManager.Instance.PlaySound(SoundType.Block);
            return;
        }

        hp = Mathf.Clamp(hp - atk, 0f, maxHP);
        floatingText.SpawnText(TextType.Hit, atk.ToString());

        if (hp > 0f)
        {
            if (isRush || isDownSmash) return; // 애니메이션 출력만 막음

            anim.SetAnim(AnimType_Player.Hurt);
            SoundManager.Instance.PlaySound(SoundType.Hit);
        }
        else
        {
            DeathPlayer();
            anim.SetAnim(AnimType_Player.Death);
            SoundManager.Instance.PlaySound(SoundType.Death);
        }

        CamImpulse(atk);
    }

    void OnMove(InputValue value)
    {
        inputVec2 = value.Get<Vector2>();

        if (inputVec2 != null && !isDead)
        {
            moveDirection = new Vector3(inputVec2.x, 0, inputVec2.y);

            anim.SetRunAnim((int)value.Get<Vector2>().x, isGrounded);
        }
    }

    void OnJump(InputValue value)
    {

        if (isBlocked || isDead || isRush || isDownSmash || !IsGameStart) return;
        if (jumpCount < maxJumpCount)
        {
            jumpCount++;

            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            isGrounded = false;

            // 점프중엔 발판에 안맞게
            gameObject.layer = LayerMask.NameToLayer("PlayerJump");

            if (!isRoll)
            {
                anim.SetAnim(AnimType_Player.Jump);
            }
        }
    }

    void OnAttack(InputValue value)
    {
        if (isRoll || isBlocked || isDead || isRush || isDownSmash || !IsGameStart) return;

        // 공격을 만들건데
        // 공격의 종류에 따라서 공격이 다르게 적용되게 해야되긴해
        // 그래도 일단 공격을 만들어 보자
        // 버튼이 눌렸을 떄, 플레이어 앞으로 공격사거리만큼 Ray를 쏴서
        // Ray에 걸린 모든 오브젝트를 파악 (근데 Ray는 처음 부딛힌 하나만 감지할텐데)
        // 그떄, target에게 데미지를 입힘

        Vector2 origin = (Vector2)transform.position + attackOffset;
        Vector2 dir = Vector2.right * anim.LookingDir();
        int enemyMask = LayerMask.GetMask("Enemy");


        RaycastHit2D rayHit = Physics2D.Raycast(origin, dir, atkRange, enemyMask);
        Debug.DrawRay(origin, dir * atkRange, Color.blue, 0.5f);

        target = rayHit.collider?.GetComponent<Enemy>();

        if (target != null)
        {
            int atkValue = (int)UnityEngine.Random.Range(atk, maxAtk);
            target.Hit(atkValue);
            Debug.Log($"Hit {target.name}");
            target = null;
        }

        anim.SetAnim(AnimType_Player.Attack);
        SoundManager.Instance.PlaySound(SoundType.Attack);
    }

    void OnRoll(InputValue value)
    {
        if (isRoll || isBlocked || isDead || isRush || isDownSmash || !IsGameStart) return;
        int useResult = skillController.UseSkill(SkillType.Roll, ref mana);
        if (useResult > 0)
        {
            isRoll = true;
            anim.SetAnim(AnimType_Player.Roll);
        }
        else floatingText.SpawnText(TextType.Nomal, skillResultText(useResult));
    }

    void OnBlock(InputValue value)
    {
        if (isRoll || isDead || isRush || isDownSmash || !IsGameStart) return;

        isBlocked = value.isPressed;
        anim.SetBlockAnim(isBlocked);
    }

    void OnRushAttack(InputValue value)
    {
        if (isRush || isRoll || isDead || isDownSmash || !IsGameStart) return;

        int useResult = skillController.UseSkill(SkillType.RushAttack, ref mana);
        if (useResult > 0)
        {
            anim.SetAnim(AnimType_Player.RushAttack);
            SoundManager.Instance.PlaySound(SoundType.RushAttack);
        }
        else floatingText.SpawnText(TextType.Nomal, skillResultText(useResult));
    }

    void OnHeal(InputValue value)
    {
        if (isRush || isRoll || isDead || isDownSmash || !IsGameStart) return;

        int useResult = skillController.UseSkill(SkillType.Heal, ref mana);
        if (useResult > 0)
        {
            hp = Mathf.Clamp(hp + healAmount, 0f, maxHP);
            floatingText.SpawnText(TextType.Heal, healAmount.ToString());
            healParticle.Play();
            SoundManager.Instance.PlaySound(SoundType.Heal);
        }
        else floatingText.SpawnText(TextType.Nomal, skillResultText(useResult));
    }

    void OnDownSmash(InputValue value)
    {
        if (isRush || isRoll || isDead || isDownSmash || !IsGameStart) return;
        if (jumpCount < 2) return; // 점프를 하지 않으면 사용 불가

        int useResult = skillController.UseSkill(SkillType.DownSmash, ref mana);
        if (useResult > 0)
        {
            anim.SetAnim(AnimType_Player.DownSmash);
            DownSmash();
        }
        else floatingText.SpawnText(TextType.Nomal, skillResultText(useResult));
    }

    // Private Method
    void CheckGround()
    {
        // 점프 후, 아래로 내려가고 있을때만
        int mapMask = LayerMask.GetMask("Map");
        Vector2 origin = boxCollider.bounds.center;
        float castDistance = boxCollider.bounds.size.y / 2;

        if (rb.linearVelocityY < 0f)
        {
            RaycastHit2D hit = Physics2D.BoxCast(origin, boxCollider.bounds.size, 0f, Vector2.down, castDistance, mapMask);
            // RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, castDistance, mapMask);
            Debug.DrawRay(origin, Vector2.down * castDistance, Color.red);

            if (hit.collider != null)
            {
                if (hit.distance < boxCollider.bounds.size.y / 2) // Player의 중심에서 Ray를 쏘고 있기 떄문에, 플레이어 높이의 반 보다 작을때 = 땅에 있음
                {
                    isGrounded = true;
                    isRush = false;
                    jumpCount = 0;

                    gameObject.layer = LayerMask.NameToLayer("Player");
                }
            }
        }
    }

    public void RecoveryMana()
    {
        if (!IsGameStart) return;
        mana = Mathf.Clamp(mana + recoveryManaAmount, 0f, maxMana);
    }

    private void DeathPlayer()
    {
        // GameOver 로직 작성
        Debug.Log("GameOver");
        GameManager.Instance.mainUI.GameOverUI();
        isDead = true;
        gameObject.layer = LayerMask.NameToLayer("DeathPlayer");
    }

    public void RushAttackStart()
    {
        if (rushCoroutine != null)
            StopCoroutine(rushCoroutine);

        rushCoroutine = StartCoroutine(RushCoroutine());
    }

    public void RushAttackEnd()
    {
        isRush = false;
    }

    IEnumerator RushCoroutine()
    {
        isRush = true;

        int objMask = LayerMask.GetMask("Wall", "Enemy");
        float dir = anim.LookingDir();

        float moved = 0f; // 이동거리

        while (moved < rushDistance)
        {
            float remain = Mathf.Min(rushStep, rushDistance - moved);
            Vector2 delta = Vector2.right * dir * remain;

            Vector2 rayOrigin = rb.position + new Vector2(rushAttackOffset.x * dir, rushAttackOffset.y);
            RaycastHit2D obj = Physics2D.Raycast(rayOrigin, delta.normalized, delta.magnitude, objMask);
            // Debug.DrawRay(new Vector3(rayOrigin.x, rayOrigin.y, transform.position.z), new Vector3(delta.x, delta.y, 0f), Color.red, 0.5f);

            if (obj.collider != null)
            {
                Enemy enemy = obj.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Hit(rushAtkDmg);
                    CamImpulse(rushAtkDmg);
                }

                float d = Mathf.Max(0f, obj.distance - 0.02f);
                rb.MovePosition(rb.position + delta.normalized * d);
                transform.position = new Vector3(rb.position.x, rb.position.y, transform.position.z);
                break;
            }

            rb.MovePosition(rb.position + delta);
            transform.position = new Vector3(rb.position.x, rb.position.y, transform.position.z);
            moved += remain;
            yield return null;
        }

        rushCoroutine = null;
    }

    void DownSmash()
    {
        if (downSmashCoroutine != null)
        {
            StopCoroutine(downSmashCoroutine);
        }

        downSmashCoroutine = StartCoroutine(DownSmashCoroutine());
    }

    IEnumerator DownSmashCoroutine()
    {
        isDownSmash = true;

        downSmashCollider.gameObject.SetActive(true);

        yield return new WaitUntil(() => isGrounded.Equals(true));

        downSmashParticle.Emit(20);
        SoundManager.Instance.PlaySound(SoundType.DownSmash);

        // 콜라이더에 닿은 오브젝트 불러와서 모두 데미지 처리
        downSmashCollider.EnemyHit(downSmashAtkDamage);

        isDownSmash = false;
        downSmashCollider.gameObject.SetActive(false);
    }

    public void CamImpulse(float force = 0)
    {
        force = Mathf.Clamp(force * 0.05f, 0.25f, 2f); // 숫자는 튜닝
        impulseSource.GenerateImpulse(force);
    }
}