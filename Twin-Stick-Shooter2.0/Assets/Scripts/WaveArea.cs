using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveArea : MonoBehaviour {

    public int waveIndex = 1;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            EnemySpawner.instance.SetNewWaveList(waveIndex);
        }
    }
}
