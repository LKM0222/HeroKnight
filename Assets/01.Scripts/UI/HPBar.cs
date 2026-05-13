using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] Image fillImg;
    [SerializeField] TMP_Text hpText;

    public void SetHPBar(float hp, float maxHP)
    {
        float hpRatio = hp / maxHP;
        fillImg.fillAmount = hpRatio;
        hpText.text = $"{Math.Clamp(hp, 0, maxHP):0}/{maxHP:0}";
    }
}
