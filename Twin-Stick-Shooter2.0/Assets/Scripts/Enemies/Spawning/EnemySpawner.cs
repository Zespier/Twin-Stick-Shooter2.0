using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public float timeBetweenWaves = 1;
    public List<Wave> waves1 = new List<Wave>();
    public List<Wave> waves2 = new List<Wave>();
    public List<Wave> waves3 = new List<Wave>();
    public float circunferenceRadius = 15f;
    public List<Transform> spawnPoints = new List<Transform>();
    public GameObject firstLevelArrows;
    public GameObject secondLevelArrows;
    public GameObject firstLevelRock;
    public GameObject secondLevelRock;

    [SerializeField] public Wave _currentWave;
    private Transform _playerBase;
    private bool _startingWave = false;
    private float _nextWaveSaveCounter; //To avoid going trhought 2 waves instantly
    public List<Wave> _currentWaveList;

    public static EnemySpawner instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void OnEnable() {
        GameEvents.OnEnemyDeath += NextWaveIfFinished;
    }
    private void OnDisable() {
        GameEvents.OnEnemyDeath -= NextWaveIfFinished;

        ResetWaves(waves1);
        ResetWaves(waves2);
        ResetWaves(waves3);
    }

    private void Update() {
        _nextWaveSaveCounter -= Time.deltaTime;
    }

    private async void Start() {

        InitializeWaves(waves1);
        await Task.Delay(1000);
        SetNewWaveList(0);
    }

    private void OnDrawGizmos() {
        if (_playerBase != null) {
            Gizmos.DrawWireSphere(_playerBase.position, circunferenceRadius);
        }
    }

    public void SetNewWaveList(int index) {
        switch (index) {
            case 0:
                _currentWaveList = waves1;
                firstLevelArrows.SetActive(false);
                firstLevelRock.SetActive(true);
                secondLevelRock.SetActive(true);
                StartWave();
                break;

            case 1:
                _currentWaveList = waves2;
                _currentWave = _currentWaveList[0];
                StartWave();
                break;

            case 2:
                _currentWaveList = waves3;
                _currentWave = _currentWaveList[0];
                StartWave();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Reset all Scriptable Object variables 
    /// </summary>
    private void ResetWaves(List<Wave> waveList) {
        for (int i = 0; i < waveList.Count; i++) {
            for (int j = 0; j < waveList[i].initialEnemies.Count; j++) {
                waveList[i].initialEnemies[j].ResetVariables();
            }
            for (int j = 0; j < waveList[i].enemiesAdditional.Count; j++) {
                waveList[i].enemiesAdditional[j].ResetVariables();
            }
        }
    }

    /// <summary>
    /// Initializes all waves
    /// </summary>
    private void InitializeWaves(List<Wave> waveList) {

        _currentWave = waveList[0];
    }

    public void StartWave() {
        StartCoroutine(C_WaitToStartWave());
    }

    private IEnumerator C_WaitToStartWave() {
        yield return null;

        _startingWave = true;

        List<int> spawnPoints = new();

        for (int i = 0; i < _currentWave.initialSpawnPointList.Count; i++) {
            if (!spawnPoints.Exists(sp => sp == _currentWave.initialSpawnPointList[i])) {
                spawnPoints.Add(_currentWave.initialSpawnPointList[i]);
            }
        }

        for (int i = 0; i < _currentWave.additionalSpawnPointList.Count; i++) {
            if (!spawnPoints.Exists(sp => sp == _currentWave.additionalSpawnPointList[i])) {
                spawnPoints.Add(_currentWave.additionalSpawnPointList[i]);
            }
        }

        SpawnWaveInitialEnemies(_currentWave);

        _startingWave = false;
    }

    /// <summary>
    /// Spawns initial enemies of a wave
    /// </summary>
    /// <param name="wave"></param>
    public void SpawnWaveInitialEnemies(Wave wave) {
        for (int i = 0; i < wave.initialEnemies.Count; i++) {
            SpawnEnemy(wave.initialEnemies[i], wave.initialSpawnPointList, i);
        }
    }

    /// <summary>
    /// Spawn Every enemy on a wave
    /// </summary>
    public void SpawnAllEnemies() {

        for (int i = 0; i < _currentWave.initialEnemies.Count; i++) {
            SpawnEnemy(_currentWave.initialEnemies[i], _currentWave.initialSpawnPointList, i);
        }

        for (int i = 0; i < _currentWave.enemiesAdditional.Count; i++) {
            SpawnEnemy(_currentWave.enemiesAdditional[i], _currentWave.additionalSpawnPointList, i);
        }
    }

    public void SpawnAllDeadConditionEnemies() {
        for (int i = 0; i < _currentWave.conditionsToSpawn.Count; i++) {
            if (_currentWave.conditionsToSpawn[i].condition == ConditionType.AllDead) {
                SpawnEnemy(_currentWave.enemiesAdditional[i], _currentWave.additionalSpawnPointList, i);
            }
        }
    }
    /// <summary>
    /// Spawn an enemy
    /// </summary>
    public void SpawnEnemy(WaveEnemy enemy, List<int> spawnPointList, int index) {

        if (enemy.spawned) { return; }

        Vector3 spawnPoint = SpawnPointRandom(spawnPoints[spawnPointList[index]].position);
        Enemy newEnemy = EnemyContainer.instance.GetEnemyFromPool(enemy.type);
        newEnemy.transform.position = spawnPoint;

        float increasedHpInThisWave = newEnemy.stats.baseHP * (_currentWave.hpIncreasePercentage / 100f);
        if (increasedHpInThisWave != 0) { newEnemy.stats.ManageAddUpgrade(Buff.BaseHp, increasedHpInThisWave); }

        float increasedAttackInThisWave = newEnemy.stats.baseAtk * (_currentWave.atkIncreasePercentage / 100f);
        if (increasedAttackInThisWave != 0) { newEnemy.stats.ManageAddUpgrade(Buff.BaseDamage, increasedAttackInThisWave); }

        newEnemy.InitializeHP();

        if (newEnemy.gameObject.activeSelf) {
            newEnemy.AssetReference = enemy;
            newEnemy.AssetReference.spawned = true;
        }
    }

    /// <summary>
    /// returns a random spawn point that is not near the player
    /// </summary>
    /// <returns></returns>
    private Vector3 SpawnPointRandom(Vector3 originPoint) {

        float newPositionX = Random.Range(-circunferenceRadius, circunferenceRadius);
        float newPositionZ = Mathf.Sqrt(circunferenceRadius * circunferenceRadius - newPositionX * newPositionX);

        float randomSign = Random.Range(0, 2);
        if (randomSign == 0) {
            newPositionZ *= -1;
        }

        return new Vector3(originPoint.x + newPositionX, originPoint.y, originPoint.z + newPositionZ);
    }

    /// <summary>
    /// Judges whether it should spawn an enemy or not
    /// </summary>
    /// <param name="waveEnemy"></param>
    /// <param name="healthRemaining"></param>
    public void EnemyTookDamage(WaveEnemy waveEnemy, float healthRemaining) {

        waveEnemy.healthRemaining = healthRemaining;

        List<int> dependantEnemiesIndexes = new List<int>();

        for (int i = 0; i < _currentWave.conditionsToSpawn.Count; i++) {

            if (_currentWave.conditionsToSpawn[i].dependantOfThisEnemy == waveEnemy || _currentWave.conditionsToSpawn[i].condition == ConditionType.AllDead) {

                dependantEnemiesIndexes.Add(i);
            }
        }

        for (int i = 0; i < dependantEnemiesIndexes.Count; i++) {

            ConditionToSpawn conditionToSpawn = _currentWave.conditionsToSpawn[dependantEnemiesIndexes[i]];

            switch (conditionToSpawn.condition) {

                case ConditionType.AllDead:
                    //TODO: This will not work if the last to die is not the referenced one
                    if (AreAllEnemiesDead()) { SpawnAllDeadConditionEnemies(); }
                    break;

                case ConditionType.EnemyDead:

                    if (healthRemaining <= 0) { SpawnEnemy(_currentWave.enemiesAdditional[dependantEnemiesIndexes[i]], _currentWave.additionalSpawnPointList, i); }
                    break;

                case ConditionType.Damaged:

                    if (healthRemaining <= conditionToSpawn.healthRemaining) { SpawnEnemy(_currentWave.enemiesAdditional[dependantEnemiesIndexes[i]], _currentWave.additionalSpawnPointList, i); }
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Checks if all enemies are dead
    /// </summary>
    /// <returns></returns>
    private bool AreAllEnemiesDead() {

        bool allDead = true;
        for (int j = 0; j < _currentWave.initialEnemies.Count; j++) {
            if (_currentWave.initialEnemies[j].spawned && _currentWave.initialEnemies[j].healthRemaining > 0) {
                allDead = false;
            }
        }

        for (int n = 0; n < _currentWave.enemiesAdditional.Count; n++) {
            if (_currentWave.enemiesAdditional[n].spawned && _currentWave.enemiesAdditional[n].healthRemaining > 0) {
                allDead = false;
            }
        }

        return allDead;
    }

    private void NextWaveIfFinished(Enemy enemy) {
        StartCoroutine(C_NextWaveIfFinished());
    }

    private IEnumerator C_NextWaveIfFinished() {
        yield return null;
        if (_nextWaveSaveCounter < 0 && FinishedWave()) {

            NextWave(_currentWaveList);
            StartWave();
        }
    }

    public bool FinishedWave() {

        bool finished = true;

        for (int i = 0; i < _currentWave.initialEnemies.Count; i++) {
            if (!_currentWave.initialEnemies[i].spawned || _currentWave.initialEnemies[i].healthRemaining > 0) {
                finished = false;
            }
        }

        for (int n = 0; n < _currentWave.enemiesAdditional.Count; n++) {
            if (!_currentWave.enemiesAdditional[n].spawned || _currentWave.enemiesAdditional[n].healthRemaining > 0) {
                finished = false;
            }
        }

        return finished;
    }

    public void NextWave(List<Wave> waveList) {

        _nextWaveSaveCounter = 0.1f;

        Debug.Log("La lista existe" + waveList != null);
        Debug.Log("_currentWave existe" + _currentWave != null);

        int newWaveIndex = waveList.FindIndex(w => w == _currentWave) + 1;

        if (newWaveIndex < waveList.Count) {
            _currentWave = waveList[newWaveIndex];
        } else {
            if (_currentWaveList == waves3) {
                YouWin.instance.ShowYouWinPanel();

            } else if (_currentWaveList == waves1) {
                firstLevelArrows.SetActive(true);
                firstLevelRock.SetActive(false);
            } else if (_currentWaveList == waves2) {
                secondLevelArrows.SetActive(true);
                secondLevelRock.SetActive(false);
            }
            Debug.Log("FINISHED GAME");
        }
    }

    #region Calculating Current Wave Index

    private bool _calculatedCurrentWaveIndex;
    private int _lastCurrentWaveIndex;

    public int GetCurrentWaveIndex(List<Wave> waveList) {
        if (_currentWave == null) { return 0; }
        if (_calculatedCurrentWaveIndex) { return _lastCurrentWaveIndex; }

        int waveIndex = 0;

        StartCoroutine(C_CalculatedWaveIndex());

        string waveNumberString = "" + _currentWave.name[^1];
        string waveDecenaString = "" + _currentWave.name[_currentWave.name.Length - 2];

        if (int.TryParse(waveNumberString, out int waveNumber)) {
            int.TryParse(waveDecenaString, out int waveDecena);//Realmente no importa si la decena falla, se queda en 0

            waveIndex = waveDecena * 10 + waveNumber;

        } else {
            waveIndex = waveList.FindIndex(w => w == _currentWave);
        }

        _lastCurrentWaveIndex = waveIndex;
        return waveIndex;
    }

    private IEnumerator C_CalculatedWaveIndex() {
        _calculatedCurrentWaveIndex = true;
        yield return null;
        _calculatedCurrentWaveIndex = false;
    }

    #endregion
}
