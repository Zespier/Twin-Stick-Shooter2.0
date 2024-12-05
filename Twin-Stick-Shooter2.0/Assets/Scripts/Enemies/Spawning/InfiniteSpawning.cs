using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteSpawning : MonoBehaviour {

    public EnemySpawner spawner;
    public Wave tankWave;
    public List<Wave> randomWaves;
    public List<Wave> initialWaves;
    public List<(int waveIndex, float difficultyMultiplier)> graphic = new List<(int, float)>() {
        (2,800),
        (5,900),
        (10,1100),
        (15,1300),
        (20,1500),
        (25,1700),
        (30,1800),
        (35,1900),
        (40,2100),
        (45,2300),
        (50,2500),
        (55,2700),
        (60,2900),
        (65,3100),
        (70,3300),
    };

    private const float incomeToDps = 0.4f;
    public List<Wave> totalPlayedWaves = new List<Wave>(capacity: 16); //Es para el recuento, da igual lo que lleve dentro

    public int TotalWaves => spawner.waves.Count;
    public int CurrentWaveIndex => spawner.CurrentWaveIndex;

    public void ResetWaves() {
        if (tankWave != null && tankWave.hpIncreasePercentage != 0) {

            for (int i = 0; i < tankWave.initialEnemies.Count; i++) {
                tankWave.initialEnemies[i].ResetVariables();
            }
            tankWave.hpIncreasePercentage = 0;
        }

        for (int i = 0; i < randomWaves.Count; i++) {
            if (randomWaves[i].hpIncreasePercentage != 0) {
                for (int j = 0; j < randomWaves[i].initialEnemies.Count; j++) {
                    randomWaves[i].initialEnemies[j].ResetVariables();
                }
                randomWaves[i].hpIncreasePercentage = 0;
            }
        }

        ResetInitialWaves();
    }

    public void ResetInitialWaves() {

        for (int i = 0; i < initialWaves.Count; i++) {
            if (initialWaves[i].hpIncreasePercentage != 0) {

                for (int n = 0; n < initialWaves[i].initialEnemies.Count; n++) {
                    initialWaves[i].initialEnemies[n].ResetVariables();
                }
                initialWaves[i].hpIncreasePercentage = 0;
            }
        }
    }

}