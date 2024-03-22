using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public List<Transform> spawnPoints = new List<Transform>();
    public List<Enemy> enemies = new List<Enemy>();
    public List<int> maxEnemies = new List<int>();

    private List<Enemy> _generatedEnemies = new List<Enemy>();

    private int _currentEnemy;

    public int CurrentEnemy { get => _currentEnemy; set => _currentEnemy = Mathf.Clamp(value, 0, enemies.Count - 1); }

    private bool _won;

    public static EnemySpawner instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void Update() {
        if (CurrentEnemy >= enemies.Count - 1) {
            if (!_won && _generatedEnemies.Count == 0) {
                YouWin.instance.ShowYouWinPanel();
            }
            return;
        }

        if (_generatedEnemies.Count < maxEnemies[CurrentEnemy]) {
            SpawnNextEnemy();
        }
    }

    public void SpawnAllEnemies() {
        for (int i = 0; i < maxEnemies.Count; i++) {
            maxEnemies[i] = 1000;
        }
    }

    public void RemoveEnemyFromGenerated(Enemy enemy) {
        _generatedEnemies.Remove(enemy);
    }

    public void SpawnNextEnemy() {
        Transform spawnPoint = SpawnPointRandom();

        _generatedEnemies.Add(Instantiate(enemies[CurrentEnemy], spawnPoint.position, Quaternion.identity, EnemyContainer.instance.transform));

        CurrentEnemy++;
    }

    private Transform SpawnPointRandom() {

        Transform result = spawnPoints[Random.Range(0, spawnPoints.Count)];

        while (Vector3.Distance(PlayerController.instance.transform.position, result.position) < 2f) {
            result = spawnPoints[Random.Range(0, spawnPoints.Count)];
        }

        return result;
    }

}
