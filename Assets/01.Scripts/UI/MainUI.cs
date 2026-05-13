using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [Header("Slider")]
    [SerializeField] Slider hpSlider;
    [SerializeField] TMP_Text hpText;
    [SerializeField] Slider mpSlider;
    [SerializeField] TMP_Text mpText;

    [Header("Key Guid")]
    [SerializeField] List<KeyGuid> keyGuidList = new List<KeyGuid>();

    [Header("Combo")]
    [SerializeField] Image comboImg;
    [SerializeField] TMP_Text comboText;
    [SerializeField] TMP_Text comboTitle;
    [SerializeField] float visibleImgTime;
    [SerializeField] float uiVanishTime;
    [SerializeField] float time;

    [Header("Remain Enemy")]
    [SerializeField] TMP_Text remainEnemyText;

    [Header("Intro")]
    [SerializeField] GameObject introUI;
    [SerializeField] TMP_Text highComboText;
    [SerializeField] TMP_Text highStageText;

    Coroutine comboCoroutine = null;

    bool isInit;

    public void Init()
    {
        comboImg.gameObject.SetActive(false);
        comboText.gameObject.SetActive(false);
        comboTitle.gameObject.SetActive(false);
        //
        keyGuidList.ForEach(x => x.Init());
        //
        isInit = true;
    }

    void Update()
    {
        if (!isInit) return;
        SetHPSlider();
        SetManaSlider();
        SetRemainEnemyText();
        SetIntroUI();
    }

    void SetHPSlider()
    {
        hpSlider.maxValue = GameManager.Instance.player.MaxHP;
        hpSlider.value = GameManager.Instance.player.HP;

        hpText.text = $"{GameManager.Instance.player.HP:0}/{GameManager.Instance.player.MaxHP:0}";
    }

    void SetManaSlider()
    {
        mpSlider.maxValue = GameManager.Instance.player.MaxMana;
        mpSlider.value = GameManager.Instance.player.Mana;

        mpText.text = $"{GameManager.Instance.player.Mana:0}/{GameManager.Instance.player.MaxMana:0}";
    }

    void SetRemainEnemyText()
    {
        if (StageManager.Instance == null) return;
        remainEnemyText.text = $"X {StageManager.Instance.RemainEnemyCount}";
    }

    void SetIntroUI()
    {
        highComboText.text = $"High Combo:{PlayerPrefs.GetInt("MaxCombo", 0)}";
        highStageText.text = $"High Stage:{PlayerPrefs.GetInt("MaxStage", 0)}";
    }

    public void SetComboUI()
    {
        if (comboCoroutine != null) // 아직 코루틴이 실행중임 -> 기존 코루틴 반복
        {
            time = 0; // 쿨타임 초기화
            GameManager.Instance.combo++;
        }
        else
        {
            GameManager.Instance.combo = 1;
            comboCoroutine = StartCoroutine(ComboCoroutine());
        }

        comboText.text = GameManager.Instance.combo.ToString();
    }

    public void GameOverUI()
    {
        introUI.SetActive(true);
    }

    IEnumerator ComboCoroutine()
    {
        yield return null;
        time = 0f;

        while (time < (visibleImgTime + uiVanishTime))
        {
            comboImg.gameObject.SetActive(true);
            comboText.gameObject.SetActive(true);
            comboTitle.gameObject.SetActive(true);


            while (time < visibleImgTime)
            {
                var deltaTime = Time.deltaTime;
                time += deltaTime;

                comboImg.color = new Color(1f, 1f, 1f, 1f);
                comboText.color = new Color(1f, 1f, 1f, 1f);
                comboTitle.color = new Color(1f, 0.6057305f, 0f, 1f);

                yield return null;
            }

            while (time - visibleImgTime < uiVanishTime)
            {
                if (time < visibleImgTime) break;

                var deltaTime = Time.deltaTime;
                time += deltaTime;

                // 시간 비율에 맞춰서 투명화 진행
                float i = 1f - Mathf.Clamp01((time - visibleImgTime) / uiVanishTime);
                comboImg.color = new Color(1f, 1f, 1f, i);
                comboText.color = new Color(1f, 1f, 1f, i);
                comboTitle.color = new Color(1f, 0.6057305f, 0f, i);

                yield return null;
            }
        }

        // 다 끝나면 코루틴 null
        comboImg.gameObject.SetActive(false);
        comboText.gameObject.SetActive(false);
        comboTitle.gameObject.SetActive(false);
        comboCoroutine = null;
    }


    public void OnClick_GameStart()
    {
        introUI.SetActive(false);
        StageManager.Instance.GaemStart();
        GameManager.Instance.player.GaemStart();
    }

    public void OnClick_GameReset()
    {
        PlayerPrefs.SetInt("MaxCombo", 0);
        PlayerPrefs.SetInt("MaxStage", 0);
    }
}
