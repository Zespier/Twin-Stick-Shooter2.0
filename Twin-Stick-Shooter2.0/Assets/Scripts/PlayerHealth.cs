using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public RectTransform health;
    public RectTransform damageBar;
    public float lerpSpeed = 0.2f;

    private Vector3 _localScale;
    private float _width;

    private void Awake() {
        _width = health.rect.width;
    }

    private void Update() {
        damageBar.sizeDelta = Vector3.Lerp(damageBar.sizeDelta, health.sizeDelta, Time.deltaTime / lerpSpeed);
    }

    public void ReduceHealthBar(float healthLeft, float maxHealth) {
        _localScale = health.sizeDelta;
        _localScale.x = Mathf.Lerp(0, _width, healthLeft / maxHealth);
        health.sizeDelta = _localScale;
    }

}
