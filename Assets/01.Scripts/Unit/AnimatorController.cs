using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public enum AnimType { Idle, Attack, Block, Hurt, Death, Jump, Roll, Run }
[Serializable] public enum AnimKey{AnimState, AttackType, Attack, Block, IdleBlock, Hurt, Death, AirSpeedY, Grounded, Jump, Roll}


public class AnimatorController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Player player;
    [SerializeField] SpriteRenderer playerSprite;

    Dictionary<AnimKey, int> animKeyDict = new Dictionary<AnimKey, int>();

    [Header("Attack")]
    [SerializeField] float attackType;

    void Start()
    {
        InitAnimDict();
    }

    void Update()
    {
        CheckGrounded(player.AirSpeedY, player.IsGrounded);
    }

    void InitAnimDict()
    {
        animKeyDict.Clear();
        foreach (AnimKey key in Enum.GetValues(typeof(AnimKey)))
        {
            animKeyDict[key] = Animator.StringToHash(key.ToString());
        }
    }

    public void SetAnim(AnimType type)
    {
        switch (type)
        {
            case AnimType.Attack:
                {
                    attackType = UnityEngine.Random.Range(0, 3) * 0.5f;
                    animator.SetFloat(animKeyDict[AnimKey.AttackType], attackType);
                    animator.SetTrigger(animKeyDict[AnimKey.Attack]);
                }
                break;

            case AnimType.Hurt:
                {
                    animator.SetTrigger(animKeyDict[AnimKey.Hurt]);
                }
                break;

            case AnimType.Death:
                {
                    animator.SetTrigger(animKeyDict[AnimKey.Death]);
                }
                break;

            case AnimType.Jump:
                {
                    animator.SetTrigger(animKeyDict[AnimKey.Jump]);
                }
                break;
        }
    }

    public void SetRunAnim(int value, bool isGrounded)
    {
        playerSprite.flipX = value < 0;
        animator.SetInteger(animKeyDict[AnimKey.AnimState], Math.Abs(value));
        animator.SetBool(animKeyDict[AnimKey.Grounded], isGrounded);
    }

    public void SetBlockAnim(bool isBlocked) // 피격도중, block상태라면 여기로 넘어와야됨.
    {
        animator.SetTrigger(animKeyDict[AnimKey.Block]);
        animator.SetBool(animKeyDict[AnimKey.IdleBlock], isBlocked);
    }

    public void CheckGrounded(float airSpeedY, bool isGrounded)
    {
        animator.SetBool(animKeyDict[AnimKey.Grounded], isGrounded);
        animator.SetFloat(animKeyDict[AnimKey.AirSpeedY], airSpeedY);
    }
}
