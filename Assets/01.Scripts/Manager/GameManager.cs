using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public Player player;

    public int combo;
    public int maxCombo
    {
        get { return PlayerPrefs.GetInt("MaxCombo", 0); }
        set { combo = value; PlayerPrefs.SetInt("MaxCombo", value); }
    }

    public MainUI mainUI;

    protected override void OnAwakeRoutine()
    {
        mainUI.Init();
    }
    
    public void SetComboUI()
    {
        mainUI.SetComboUI();
    }
}
