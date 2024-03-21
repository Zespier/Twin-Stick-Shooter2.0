using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesHealthMultiplier : MonoBehaviour {

    [SerializeField] private float _healthMultiplier = 1f;

    public float HealthMultiplier { get => _healthMultiplier; }

    public static EnemiesHealthMultiplier instance;

    private void Awake() {
        if (!instance) { instance = this; }
    }
}
