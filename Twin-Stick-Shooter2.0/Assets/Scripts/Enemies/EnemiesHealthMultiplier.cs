using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesHealthMultiplier : MonoBehaviour {

    [SerializeField] private float _healthMultiplier = 1f;

    /// <summary>
    /// Multiplies the health of the enemies, to balance the game easier
    /// </summary>
    public float HealthMultiplier { get => _healthMultiplier; }

    public static EnemiesHealthMultiplier instance;

    private void Awake() {
        if (!instance) { instance = this; }
    }
}
