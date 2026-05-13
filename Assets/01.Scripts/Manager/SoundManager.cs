using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public enum SoundType{ Attack, Heal, ButtonClick, DownSmash, Hit, Death, WizardArrack, GoblinAttack, RushAttack, BGM, Block, Jump }

[Serializable]
public class ClipInfo
{
    public SoundType type;
    public AudioSource source;
}


public class SoundManager : MonoSingleton<SoundManager>
{
    [Header("Data")]
    [SerializeField] List<ClipInfo> clipList; // 사운드 리스트


    public void PlaySound(SoundType type)
    {
        switch (type)
        {
            case SoundType.Attack:
            case SoundType.Hit:
                {
                    PlayerRandomSound(type);
                }
                break;

            case SoundType.BGM:
            case SoundType.Heal:
            case SoundType.ButtonClick:
            case SoundType.DownSmash:
            case SoundType.Death:
            case SoundType.WizardArrack:
            case SoundType.GoblinAttack:
            case SoundType.RushAttack:
            case SoundType.Block:
            case SoundType.Jump:
                {
                    clipList.Find(x => x.type.Equals(type)).source.Play();
                }
                break;
        }
    }

    void PlayerRandomSound(SoundType type)
    {
        var list = clipList.FindAll(x => x.type.Equals(type));
        list[UnityEngine.Random.Range(0, list.Count)].source.Play();
    }
}
