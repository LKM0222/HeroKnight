using UnityEngine;

[CreateAssetMenu(fileName = "ButtonClick", menuName = "Scriptable Objects/ButtonClick")]
public class ButtonClick : ScriptableObject
{
    public void PlayButtonClick()
    {
        SoundManager.Instance.PlaySound(SoundType.ButtonClick);
    }
}
