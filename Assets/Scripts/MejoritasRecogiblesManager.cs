using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MejoritasRecogiblesManager : MonoBehaviour {

    //La logica es que no quiero que los clientes tengan esto siquiera, en plan pa que.

    public int enemiesPerUpgrade = 6;
    public Mejoritarecogible prefab;

    private int _currentEnemies;

    public static MejoritasRecogiblesManager instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    public void SpawnMejoritaRecogible(Vector3 position) {
        //_currentEnemies++;
        //if (_currentEnemies >= enemiesPerUpgrade) {
        //    _currentEnemies = 0;
        //    Instantiate(prefab, position, Quaternion.identity).GetComponent<NetworkObject>().Spawn();
        //}
    }
}
