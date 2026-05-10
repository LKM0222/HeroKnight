using System;
using System.Collections;
using TMPro;
using UnityEngine;

[Serializable]
public class TextObj : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    FloatingText parent;

    Coroutine floatingTextCoroutine = null;

    public void Init(FloatingText _parent, Color color, string msg)
    {
        this.parent = _parent;
        text.color = color;
        text.text = msg;

        this.gameObject.SetActive(true);

        if (floatingTextCoroutine != null)
        {
            StopCoroutine(floatingTextCoroutine);
            floatingTextCoroutine = null;
        }

        floatingTextCoroutine = StartCoroutine(FloatingTextCoroutine());
    }


    IEnumerator FloatingTextCoroutine()
    {
        yield return null;
        this.transform.localPosition = Vector3.zero;

        for (float i = 0; i < parent.upPos; i += 0.01f)
        {
            transform.localPosition = Vector3.up * i;

            var c = text.color;
            c.a = 1f - i; 
            text.color = c;

            yield return new WaitForSeconds(0.01f);
        }

        this.gameObject.SetActive(false);
    }
}
