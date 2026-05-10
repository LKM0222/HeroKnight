using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public enum AnimType_Player { Idle, Attack, Block, Hurt, Death, Jump, Roll, Run }
[Serializable] enum AnimKey_Player{AnimState, AttackType, Attack, Block, IdleBlock, Hurt, Death, AirSpeedY, Grounded, Jump, Roll}


public class AnimatorController_Player : AnimatorController
{
    [SerializeField] Player player;

    Dictionary<AnimKey_Player, int> animDict = new Dictionary<AnimKey_Player, int>();

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
        animDict.Clear();
        foreach (AnimKey_Player key in Enum.GetValues(typeof(AnimKey_Player)))
        {
            animDict[key] = Animator.StringToHash(key.ToString());
        }
    }

    public void SetAnim(AnimType_Player type)
    {
        switch (type)
        {
            case AnimType_Player.Attack:
                {
                    attackType = UnityEngine.Random.Range(0, 3) * 0.5f;
                    animator.SetFloat(animDict[AnimKey_Player.AttackType], attackType);
                    animator.SetTrigger(animDict[AnimKey_Player.Attack]);
                }
                break;

            case AnimType_Player.Hurt:
                {
                    animator.SetTrigger(animDict[AnimKey_Player.Hurt]);
                }
                break;

            case AnimType_Player.Death:
                {
                    animator.SetTrigger(animDict[AnimKey_Player.Death]);
                }
                break;

            case AnimType_Player.Jump:
                {
                    animator.SetTrigger(animDict[AnimKey_Player.Jump]);
                }
                break;
            case AnimType_Player.Roll:
                {
                    animator.SetTrigger(animDict[AnimKey_Player.Roll]);
                }
                break;
        }
    }

    public void SetRunAnim(int value, bool isGrounded)
    {
        if(value != 0) sr.flipX = value < 0;
        animator.SetInteger(animDict[AnimKey_Player.AnimState], Math.Abs(value));
        animator.SetBool(animDict[AnimKey_Player.Grounded], isGrounded);
    }

    public void SetBlockAnim(bool isBlocked) // 피격도중, block상태라면 여기로 넘어와야됨.
    {
        animator.SetBool(animDict[AnimKey_Player.IdleBlock], isBlocked);

        if (isBlocked)
        {
            animator.SetTrigger(animDict[AnimKey_Player.Block]);
        }
    }

    public void CheckGrounded(float airSpeedY, bool isGrounded)
    {
        animator.SetBool(animDict[AnimKey_Player.Grounded], isGrounded);
        animator.SetFloat(animDict[AnimKey_Player.AirSpeedY], airSpeedY);
    }

    public void AnimEvent_RollEnd()
    {
        player.IsRoll = false;
    }


}
