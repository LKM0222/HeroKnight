using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public enum SkillType { Roll, Heal, RushAttack, DownSmash }

[Serializable]
public class SkillInfo
{
    public SkillType skillType;
    public float coolTime;
    public float time;
    public float consumeMana;
    public bool canUse;

    public Coroutine coolTimeCoroutine;
}

public class SkillController : MonoBehaviour
{
    [SerializeField] List<SkillInfo> skillInfo = new List<SkillInfo>();

    private void Start() {
        skillInfo.ForEach(x => x.canUse = true);
    }

    public int UseSkill(SkillType type, ref float mana)
    {
        var info = skillInfo.Find(x => x.skillType.Equals(type));

        // 쿨타임중이 아니고, 마나가 충분하다면
        if (info.coolTimeCoroutine == null && info.consumeMana <= mana)
        {
            mana -= info.consumeMana;
            info.coolTimeCoroutine = StartCoroutine(CoolTimeCoroutine(info));
            return 1;
        }
        else
        {
            if (info.coolTimeCoroutine != null) return -1; // 쿨타임중
            else return -2; // 마나 부족
        }
    }

    IEnumerator CoolTimeCoroutine(SkillInfo info)
    {
        info.canUse = false;
        //
        info.time = info.coolTime;
        while (info.time > 0f)
        {
            info.time -= Time.deltaTime;
            yield return null;
        }
        //
        info.coolTimeCoroutine = null;
        info.canUse = true;
    }

    public SkillInfo FindSkill(SkillType skillType)
    {
        return skillInfo.Find(x => x.skillType.Equals(skillType));
    }
}
