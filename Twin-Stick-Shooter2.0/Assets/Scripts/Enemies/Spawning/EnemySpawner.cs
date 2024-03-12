using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour {

    public List<Wave> waves = new List<Wave>();
    public Transform enemyContainer;
    public Wolf wolf;
    public Shaman shaman;
    public Turret turret;

    private float _currentMoney;
    private float _waveTimer;
    private List<Coroutine> c_ActiveWaves = new List<Coroutine>();
    private List<Wave> _wavesAskingForMoney = new List<Wave>();
    private Coroutine c_waveSpawning;

    public SpawnArea SpawnArea { get; set; }
    public int CurrentWaveIndex { get => waves.IndexOf(CurrentWave); }
    public Wave CurrentWave { get; set; }

    private void OnEnable() {
        Events.OnEnterSpawnArea += StartWaveSpawning;
        Events.OnExitSpawnArea += FinishWaveSpawning;

        Events.OnWaveStarted += SetNextMoneyPeak;
        Events.OnWaveStarted += WaveStartsAskingForMoney;

        Events.OnWaveEnded += StopAskingForMoney;
    }

    private void OnDisable() {
        Events.OnEnterSpawnArea -= StartWaveSpawning;
        Events.OnExitSpawnArea -= FinishWaveSpawning;

        Events.OnWaveStarted -= SetNextMoneyPeak;
        Events.OnWaveStarted -= WaveStartsAskingForMoney;

        Events.OnWaveEnded -= StopAskingForMoney;
    }

    private void InitializeMoney() {
        _currentMoney = SpawnArea.initialMoney;
    }

    private void GenerateMoney() {
        _currentMoney += MoneyPerSecondThisWave() * Time.deltaTime;
    }

    private void ShareMoneyBetweenWaves() {
        if (_wavesAskingForMoney.Count > 0) {
            float moneyPerWave = _currentMoney / _wavesAskingForMoney.Count;
            for (int i = 0; i < _wavesAskingForMoney.Count; i++) {
                _wavesAskingForMoney[0].money = 1;
            }
            foreach (Wave wave in _wavesAskingForMoney) {

                wave.money += moneyPerWave;
                _currentMoney -= moneyPerWave;
            }
        }
    }

    private float MoneyPerSecondThisWave() {
        return Mathf.Lerp(SpawnArea.moneyPerSecond, SpawnArea.moneyPerSecond * 2, (float)CurrentWaveIndex / (waves.Count - 1));
    }

    /// <summary>
    /// Checks if the next wave can be spawned
    /// </summary>
    private void CheckForSpawnableWaves() {

        if (_waveTimer >= SpawnArea.timeBetweenWaves) {

            if (CurrentWaveIndex == 0) {

            } else if (CurrentWaveIndex + 1 < waves.Count) {
                CurrentWave = waves[CurrentWaveIndex + 1];
            } else {
                Debug.LogError("NO MORE WAVES");
            }

            switch (CurrentWave.startType) {
                case StartType.additive:
                    c_ActiveWaves.Add(StartCoroutine(C_SpawnWave(CurrentWave)));
                    break;
                case StartType.exclusive:
                    if (c_ActiveWaves != null && c_ActiveWaves.Count <= 0) {
                        c_ActiveWaves.Add(StartCoroutine(C_SpawnWave(CurrentWave)));
                    }
                    //TODO: Check if there is no more enemies from previous waves => Checking if there is any wave spawning on the c_ActiveWaves
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Spawns the specified wave
    /// The spawn method depends on the WaveType
    /// </summary>
    /// <param name="wave"></param>
    /// <returns></returns>
    private IEnumerator C_SpawnWave(Wave wave) {
        Events.OnWaveStarted?.Invoke(wave);

        switch (wave.waveType) {
            case WaveType.continuos:

                while (wave.amount > 0) {
                    if (wave.money >= wave.nextMoneyPeak) {
                        WaveConsumesMoney(wave);
                        SpawnEnemy(wave);
                    }
                    yield return null;
                }

                break;

            case WaveType.instantaneous:
                while (wave.money < wave.nextMoneyPeak) {
                    yield return null;
                }
                WaveConsumesMoney(wave);
                SpawnWaveInstantaneous(wave);
                break;

            default:
                WaveConsumesMoney(wave);
                SpawnWaveInstantaneous(wave);
                break;
        }
    }

    /// <summary>
    /// Instantaneously spawns every enemy on the wave
    /// Depending on the startType, it will wait for enough money or debt the wave
    /// </summary>
    /// <param name="wave"></param>
    private void SpawnWaveInstantaneous(Wave wave) {
        for (int i = 0; i < wave.amount; i++) {
            //TODO: SPAWN ALL ENEMIES TROUGH EVERY SPAWNER => POSITIONS WILL COME AFTER THAT
        }
    }

    private void WaveStartsAskingForMoney(Wave wave) {
        _wavesAskingForMoney.Add(wave);
    }

    private void StopAskingForMoney(Wave wave) {
        _wavesAskingForMoney.Remove(wave);
    }

    private void WaveConsumesMoney(Wave wave) {
        wave.money -= wave.nextMoneyPeak;
    }
    private void SetNextMoneyPeak(Wave wave) {
        if (wave.waveType == WaveType.continuos) {
            wave.nextMoneyPeak = 10;
        } else {
            wave.nextMoneyPeak = 1000;
        }
    }

    public void StartWaveSpawning(SpawnArea spawnArea) {
        AssignNewSpawnArea(spawnArea);
        AssignNewWaves(spawnArea);

        InitializeMoney();

        if (c_waveSpawning != null) {
            StopCoroutine(c_waveSpawning);
        }
        c_waveSpawning = StartCoroutine(C_WaveSpawning());
    }

    private IEnumerator C_WaveSpawning() {
        _waveTimer = SpawnArea.timeBetweenWaves;

        if (waves != null && waves.Count > 0) {
            CurrentWave = waves[0];
        } else {
            Debug.LogError("No waves right niaw");
        }

        //TODO: Until all the waves end
        while (true) {

            GenerateMoney();
            ShareMoneyBetweenWaves();

            CheckForSpawnableWaves();

            _waveTimer += Time.deltaTime;
            yield return null;
        }
    }

    private void FinishWaveSpawning() {
        RemoveSpawnArea();
        RemoveWaves();

        if (c_waveSpawning != null) {
            StopCoroutine(c_waveSpawning);
        }
    }

    private void AssignNewSpawnArea(SpawnArea spawnArea) {
        SpawnArea = spawnArea;
    }

    private void RemoveSpawnArea() {
        SpawnArea = null;
    }

    private void AssignNewWaves(SpawnArea spawnArea) {
        RemoveWaves();

        foreach (var wave in spawnArea.waves) {
            waves.Add(new Wave(wave.waveType, wave.startType, wave.enemyType, wave.amount, wave.money, wave.nextMoneyPeak));
        }
    }

    private void RemoveWaves() {
        waves.Clear();
    }

    private void SpawnEnemy(Wave wave) {

        switch (wave.enemyType) {
            case EnemyType.normal:
                Instantiate(shaman, GetNewSpawnPosition(), Quaternion.identity, enemyContainer);
                break;
            case EnemyType.fast:
                break;
            case EnemyType.heavy:
                break;
            case EnemyType.miniboss:
                break;
            case EnemyType.boss:
                break;
            default:
                break;
        }

        wave.amount--;
    }

    private Vector2 GetNewSpawnPosition() {
        Vector2 newPosition;

        int numberOfTries = 4;
        int counter = 0;
        do {
            float xPosition = Random.Range(SpawnArea.boxCollider2D.bounds.min.x, SpawnArea.boxCollider2D.bounds.max.x);
            float yPosition = Random.Range(SpawnArea.boxCollider2D.bounds.min.y, SpawnArea.boxCollider2D.bounds.max.y);
            newPosition = new Vector2(xPosition, yPosition);

            counter++;
        } while (Vector3.Distance(PlayerController.instance.transform.position, newPosition) > 2f && counter < numberOfTries);

        return newPosition;
    }
}

[System.Serializable]
public class Wave {
    public WaveType waveType;
    public StartType startType;
    public EnemyType enemyType;
    public float amount;
    public float money;
    public float nextMoneyPeak;

    public Wave(WaveType waveType, StartType startType, EnemyType enemyType, float amount, float money, float nextMoneyPeak) {
        this.waveType = waveType;
        this.startType = startType;
        this.enemyType = enemyType;
        this.amount = amount;
        this.money = money;
        this.nextMoneyPeak = nextMoneyPeak;
    }
}

public enum WaveType {
    continuos,
    instantaneous
}

public enum StartType {
    additive,
    exclusive
}

public enum EnemyType {
    normal,
    fast,
    heavy,
    miniboss,
    boss
}