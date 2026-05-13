using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class KeyGuid : MonoBehaviour // 나중에 스킬을 연동시킬 수 있도록 할거임
{
    [Header("Key")]
    [SerializeField] SkillType skillType;
    [SerializeField] KeyCode keyType;
    [SerializeField] TMP_Text keyGuidText;
    [SerializeField] Image keyIcon;

    [Header("Timer")]
    [SerializeField] TMP_Text timerText;
    [SerializeField] Image timerImg;

    [SerializeField] SkillInfo info;

    bool isInit;

    private void Update()
    {
        if (info == null || !isInit) return;
        SetUI();
    }


    public void Init()
    {
        this.info = GameManager.Instance.player.skillController.FindSkill(skillType);

        keyGuidText.text = keyType.ToString().ToLower();
        //
        isInit = true;
    }

    private void SetUI()
    {
        timerText.gameObject.SetActive(!info.canUse);
        timerText.text = $"{info.time:0}s";
        timerImg.fillAmount = Mathf.Clamp01(info.time / info.coolTime);
    }
    
}
