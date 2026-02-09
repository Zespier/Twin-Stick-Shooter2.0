using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveArea : MonoBehaviour {

    public int waveIndex = 1;

    private bool _entered;

    private void OnTriggerEnter(Collider other) {
        if (!_entered && other.CompareTag("Player")) {
            EnemySpawner.instance.SetNewWaveList(waveIndex);
            //StartCoroutine(C_WaitForEnemySpawnerToSpawnTheseWaves());
            Debug.Log("ENtroOOO");

            _entered = true;
        }
    }

    private IEnumerator C_WaitForEnemySpawnerToSpawnTheseWaves() {
        bool canStartNewWaveList = false;

        do {

            yield return null;

            bool noMoreWavesInList = false;

            int newWaveIndex = EnemySpawner.instance._currentWaveList.FindIndex(w => w == EnemySpawner.instance._currentWave) + 1;
            if (newWaveIndex < EnemySpawner.instance._currentWaveList.Count) {
            } else {
                noMoreWavesInList = true;
            }

            canStartNewWaveList = EnemySpawner.instance.FinishedWave() && noMoreWavesInList;

        } while (!canStartNewWaveList);

    }
}
