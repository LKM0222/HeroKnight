using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public enum AnimType_Goblin { Idle, Attack, Run, Hit, Death }
[Serializable] enum AnimKey_Goblin{ AttackType, Attack, Hit, Death, MoveDir }
public class AnimatorController_Goblin : AnimatorController
{
    bool isAttack = false;

    [SerializeField] Goblin owner;
    Dictionary<AnimKey_Goblin, int> animDict = new Dictionary<AnimKey_Goblin, int>();


    void Start()
    {
        InitAnimDict();
    }

    void InitAnimDict()
    {
        animDict.Clear();
        foreach (AnimKey_Goblin key in Enum.GetValues(typeof(AnimKey_Goblin)))
        {
            animDict[key] = Animator.StringToHash(key.ToString());
        }
    }

    public void SetAnim(AnimType_Goblin type)
    {
        switch (type)
        {
            case AnimType_Goblin.Hit:
                {
                    if (isAttack) return;
                    animator.SetTrigger(animDict[AnimKey_Goblin.Hit]);
                }
                break;

            case AnimType_Goblin.Death:
                {
                    animator.SetTrigger(animDict[AnimKey_Goblin.Death]);
                }
                break;
        }
    }

    public void SetRunAnim(float value)
    {
        if (value != 0) sr.flipX = value < 0;
        animator.SetInteger(animDict[AnimKey_Goblin.MoveDir], (int)Math.Abs(value));
    }

    public void SetAttackAnim(float attackType)
    {
        SetRunAnim(0);
        isAttack = true;
        animator.SetFloat(animDict[AnimKey_Goblin.AttackType], attackType);
        animator.SetTrigger(animDict[AnimKey_Goblin.Attack]);
    }

    public void AnimEvent_AttackEnd()
    {
        isAttack = false;
    }

    public void AnimEvent_Attack()
    {
        owner.Attack();
    }
}
