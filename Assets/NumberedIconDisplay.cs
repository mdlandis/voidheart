using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NumberedIconDisplay : MonoBehaviour {
    public SpriteRenderer iconRenderer;
    public TMPro.TextMeshPro text;

    public void Init(Sprite sprite) {
        iconRenderer.sprite = sprite;
    }

    public void Appear() {
        iconRenderer.gameObject.SetActive(true);
        iconRenderer.transform.localPosition += new Vector3(0.0f, 0.1f, 0.0f);
        iconRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        iconRenderer.DOFade(1.0f, 0.1f);
        iconRenderer.transform.DOLocalMove(Vector3.zero, 0.1f);
    }

    public void SetValue(object newValue) {
        SetValue(newValue.ToString());
    }

    public void SetValue(string newValue) {
        text.gameObject.SetActive(true);
        text.text = newValue;

        text.transform.localPosition += new Vector3(0.0f, 0.1f, 0.0f);
        text.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        text.DOFade(1.0f, 0.1f);
        text.transform.DOLocalMove(Vector3.zero, 0.1f);
    }

    public void Break() {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}