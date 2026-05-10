using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable] public enum AnimType_EvilWizard { Idle, Attack, Run, Hit, Death }
[Serializable] public enum AnimKey_EvilWizard { AttackType, Attack, Hit, Death, MoveDir }

public class AnimatorController_EvilWizard : AnimatorController_Enemy
{
    [SerializeField] EvilWizard owner;

    Dictionary<AnimKey_EvilWizard, int> animDict = new Dictionary<AnimKey_EvilWizard, int>();
    void Start()
    {
        InitAnimDict();
    }

    void InitAnimDict()
    {
        animDict.Clear();
        foreach (AnimKey_EvilWizard key in Enum.GetValues(typeof(AnimKey_EvilWizard)))
        {
            animDict[key] = Animator.StringToHash(key.ToString());
        }
    }

    public void SetAnim(AnimKey_EvilWizard type)
    {
        switch (type)
        {
            case AnimKey_EvilWizard.Hit:
                {
                    if (isAttack) return;
                    animator.SetTrigger(animDict[AnimKey_EvilWizard.Hit]);
                }
                break;

            case AnimKey_EvilWizard.Death:
                {
                    animator.SetTrigger(animDict[AnimKey_EvilWizard.Death]);
                }
                break;
        }
    }

    public void SetRunAnim(float value)
    {
        if (value != 0) sr.flipX = value < 0;
        animator.SetInteger(animDict[AnimKey_EvilWizard.MoveDir], (int)Math.Abs(value));
    }

    public void SetAttackAnim(float attackType)
    {
        SetRunAnim(0);
        isAttack = true;
        animator.SetTrigger(animDict[AnimKey_EvilWizard.Attack]);
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
