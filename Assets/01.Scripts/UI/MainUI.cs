using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [Header("Slider")]
    [SerializeField] Slider hpSlider;
    [SerializeField] TMP_Text hpText;
    [SerializeField] Slider expSlider;
    [SerializeField] TMP_Text expText;

    [Header("Key Guid")]
    [SerializeField] List<KeyGuid> keyGuidList = new List<KeyGuid>();

    void Update()
    {
        SetHPSlider();
    }

    void SetHPSlider()
    {
        hpSlider.maxValue = GameManager.Instance.player.MaxHP;
        hpSlider.value = GameManager.Instance.player.HP;

        hpText.text = $"{GameManager.Instance.player.HP}/{GameManager.Instance.player.MaxHP}";
    }

    void SetEXPSlider()
    {

    }
}
