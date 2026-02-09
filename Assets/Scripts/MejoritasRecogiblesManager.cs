using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MejoritasRecogiblesManager : MonoBehaviour {

    public int enemiesPerUpgrade = 6;
    public Mejoritarecogible prefab;

    private int _currentEnemies;

    public static MejoritasRecogiblesManager instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    public void SpawnMejoritaRecogible(Vector3 position) {
        _currentEnemies++;
        if (_currentEnemies >= enemiesPerUpgrade) {
            _currentEnemies = 0;
            Instantiate(prefab, position, Quaternion.identity);
        }
    }
}
