using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public Player player;

    public int combo;
    public int maxCombo;

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
