using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveTimer : MonoBehaviour {

    public TMP_Text timer;
    public TMP_Text waveCounter;

    public int CurrentWaveIndex => EnemySpawner.instance.CurrentWaveIndex;

    private void Update() {

        if (EnemySpawner.instance._currentWave != null) {

            string waveIndex = CurrentWaveIndex < 9 ? $"0{CurrentWaveIndex}" : CurrentWaveIndex.ToString();
            waveCounter.text = $"Oleada {waveIndex}";

        } else {
            waveCounter.text = $"Oleada {0}";
        }
    }

    public void SetTimer(float time) {
        timer.text = "00:" + Mathf.CeilToInt(time).ToString("D2");

        if (time <= 0) {
            timer.text = "Defiende";
        }
    }
}
