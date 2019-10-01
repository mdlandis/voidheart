using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HealthBarDisplay : MonoBehaviour {
    public SimpleHealthBar healthBar = null;
    public TMPro.TextMeshProUGUI textMeshPro = null;

    public int maxHealth;
    public int currHealth;
    public float displayHealth;
    private Tween tweenInProgress;

    public void Init(int max) {
        maxHealth = max;
        currHealth = max;
        displayHealth = max;

        healthBar.UpdateBar(currHealth, maxHealth);
        textMeshPro.text = currHealth + "/" + maxHealth;
    }

    public void Attach(Transform t) {
        transform.SetParent(t);
        transform.localPosition = new Vector2(0.0f, -0.4f);
    }

    public void SetValue(int value) {
        if (tweenInProgress != null && tweenInProgress.active) {
            tweenInProgress.Kill();
        }

        currHealth = value;
        tweenInProgress = DOTween.To(() => displayHealth, (x) => {
            displayHealth = x;
            healthBar.UpdateBar(displayHealth, maxHealth);
        }, currHealth, 1.0f);

        textMeshPro.text = currHealth + "/" + maxHealth;
    }
}