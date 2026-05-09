using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Unit
{
    [Header("Refrence")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D boxCollider;
    

    [Header("Base Data")]
    [SerializeField] Vector2 inputVec2;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] bool isGrounded;

    public bool IsGrounded => isGrounded;
    public float AirSpeedY => rb.linearVelocityY;

    void Update()
    {
        transform.Translate(moveDirection * Time.deltaTime * moveSpeed);
    }

    void FixedUpdate()
    {
        CheckGround();
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
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isGrounded = false;

            // 점프중엔, IsTrigger 체크해서, 충돌 판정 안받게 -> 나중에 점프중엔 피격 안되는거 아닌가??
            boxCollider.isTrigger = true;

            anim.SetAnim(AnimType.Jump);
        }
    }

    void OnAttack(InputValue value)
    {
        anim.SetAnim(AnimType.Attack);
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
}
