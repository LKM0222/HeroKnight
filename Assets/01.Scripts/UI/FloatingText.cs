using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable] public enum TextType { Nomal, Hit, Heal }

[Serializable]
public class TextColor
{
    public TextType textType;
    public Color color;
}

public class FloatingText : MonoBehaviour
{
    [SerializeField] List<TextColor> textColor = new List<TextColor>();
    [SerializeField] List<TextObj> floatingTextObj;

    [SerializeField] TextObj textObjPrefab;

    public float upPos;

    void Start()
    {
        floatingTextObj.ForEach(x => x.gameObject.SetActive(false));
        floatingTextObj.ForEach(x => x.transform.localPosition = Vector3.zero);
    }

    public void SpawnText(TextType type, string message)
    {
        var unActiveObj = floatingTextObj.Find(x => x.gameObject.activeSelf.Equals(false));

        if (unActiveObj == null)
        {
            unActiveObj = Instantiate(textObjPrefab, this.transform);
            floatingTextObj.Add(unActiveObj);
        }

        unActiveObj.Init(this, FindColor(type), message);
    }

    Color FindColor(TextType type)
    {
        return textColor.Find(x => x.textType.Equals(type)).color;
    }
}
