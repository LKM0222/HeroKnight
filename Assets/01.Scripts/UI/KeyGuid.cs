using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class KeyGuid : MonoBehaviour // 나중에 스킬을 연동시킬 수 있도록 할거임
{
    [Header("Key")]
    [SerializeField] KeyCode keyType;
    [SerializeField] TMP_Text keyGuidText;
    [SerializeField] Image keyIcon;

    [Header("Timer")]
    [SerializeField] TMP_Text timerText;
    [SerializeField] Image timerImg;
    [SerializeField] float time;
    Coroutine timerCoroutine = null;

    void OnEnable()
    {
        Init();
    }

    void Init()
    {
        keyGuidText.text = keyType.ToString().ToLower();
        timerText.gameObject.SetActive(false);
        timerImg.gameObject.SetActive(false);
    }

    public void SetTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    IEnumerator TimerCoroutine()
    {
        float curTime = 0;

        timerImg.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);

        while (curTime < time)
        {
            timerImg.fillAmount = 1 - (curTime / time);
            timerText.text = $"{time - curTime:D2}s";

            var deltaTime = Time.deltaTime;
            curTime += deltaTime;
            yield return new WaitForSeconds(deltaTime);
        }

        // 스킬 사용 가능 상태로 변경 후, UI 닫기
        timerImg.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
    }
}
