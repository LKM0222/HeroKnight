using System;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public Player player;

    public int combo;
    public int maxCombo
    {
        get => PlayerPrefs.GetInt("MaxCombo", 0);
        set
        {
            int prev = PlayerPrefs.GetInt("MaxCombo", 0);
            int next = Math.Max(prev, value);
            PlayerPrefs.SetInt("MaxCombo", next);
        }
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
