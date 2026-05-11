using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Unit
{
    [Header("Refrence")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] AnimatorController_Player anim;


    [Header("Base Data")]
    [SerializeField] Vector2 inputVec2;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isRoll;
    [SerializeField] bool isBlocked;
    [SerializeField] Enemy target;
    [SerializeField] Vector2 attackOffset; // 공격 레이 쏘는 오프셋
    [SerializeField] private float maxMana;
    [SerializeField] private float mana;
    [SerializeField] private float recoveryManaAmount;

    public bool IsGrounded => isGrounded;
    public float AirSpeedY => rb.linearVelocityY;
    public bool IsRoll { get { return isRoll; } set { isRoll = value; } }
    public float MaxMana => maxMana;
    public float Mana => mana;

    protected override void Start()
    {
        base.Start();
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
        if (isBlocked) return;
        transform.Translate(moveDirection * Time.deltaTime * moveSpeed);
    }

    public void Hit(float atk)
    {
        if (isBlocked)
        {
            anim.SetBlcokSuccessAnim();
            floatingText.SpawnText(TextType.Nomal, "Defenced");
            return;
        }
        hp -= atk;
        floatingText.SpawnText(TextType.Hit, atk.ToString());
        anim.SetAnim(AnimType_Player.Hurt);
    }

    void OnMove(InputValue value)
    {
        inputVec2 = value.Get<Vector2>();

        if (inputVec2 != null)
        {
            moveDirection = new Vector3(inputVec2.x, 0, inputVec2.y);

            anim.SetRunAnim((int)value.Get<Vector2>().x, isGrounded);
        }
    }

    void OnJump(InputValue value)
    {
        if (!isGrounded || isBlocked) return;

        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        isGrounded = false;

        // 점프중엔, IsTrigger 체크해서, 충돌 판정 안받게 -> 나중에 점프중엔 피격 안되는거 아닌가??
        boxCollider.isTrigger = true;

        if (!isRoll)
        {
            anim.SetAnim(AnimType_Player.Jump);
        }
    }

    void OnAttack(InputValue value)
    {
        if (isRoll || isBlocked) return;

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
            target.Hit(atk);
            Debug.Log($"Hit {target.name}");
            target = null;
        }

        anim.SetAnim(AnimType_Player.Attack);
        GameManager.Instance.SetComboUI();
    }

    void OnRoll(InputValue value)
    {
        if (isRoll || isBlocked) return;

        isRoll = true;
        anim.SetAnim(AnimType_Player.Roll);
    }

    void OnBlock(InputValue value)
    {
        if (isRoll) return;

        isBlocked = value.isPressed;
        anim.SetBlockAnim(isBlocked);
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
                    Debug.Log($"Grounded : {hit.collider.gameObject.name}");
                    isGrounded = true;
                    boxCollider.isTrigger = false;
                }
            }
        }
    }

    public void RecoveryMana()
    {
        mana = Mathf.Clamp(mana + recoveryManaAmount, 0f, maxMana);
    }
}