using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] Image fillImg;

    public void SetHPBar(float hpRatio)
    {
        fillImg.fillAmount = hpRatio;
    }
}
