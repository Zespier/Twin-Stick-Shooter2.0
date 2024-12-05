using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer : MonoBehaviour {

    public List<Enemy> activeEnemies = new List<Enemy>(capacity: 64);
    [Tooltip("Just for reference")]
    public EnemyType enemyTypesAvailableInOrder;
    public List<Enemy> enemyPrefabs = new();
    public List<int> initialPoolAmount = new() { };

    public List<Enemy> _enemyPool = new(capacity: 64);

    public static EnemyContainer instance;
    private void Awake() {
        if (!instance) { instance = this; }

        SetInitialPoolAmountByDefaultIfNotSet();

        InitializePool();
    }

    public void SetInitialPoolAmountByDefaultIfNotSet() {
        if (initialPoolAmount.Count != enemyPrefabs.Count) {
            initialPoolAmount.Clear();
            for (int i = 0; i < enemyPrefabs.Count; i++) {
                initialPoolAmount.Add(15);
            }
        }
    }

    public void InitializePool() {
        for (int i = 0; i < enemyPrefabs.Count; i++) {
            for (int j = 0; j < initialPoolAmount[i]; j++) {
                NewEnemy(enemyPrefabs[i].type);
            }
        }
    }

    public void NewEnemy(EnemyType enemyType) {
        for (int i = 0; i < enemyPrefabs.Count; i++) {
            if (enemyPrefabs[i].type == enemyType) {

                Enemy newEnemy = Instantiate(enemyPrefabs[i], transform);
                _enemyPool.Add(newEnemy);

                newEnemy.gameObject.SetActive(false);
                return;
            }
        }

        Debug.LogError("Enemy not defined");
    }

    public void StoreEnemyInPool(Enemy enemy) {
        _enemyPool.Add(enemy);
    }

    public Enemy GetEnemyFromPool(EnemyType enemyType) {

        for (int i = 0; i < _enemyPool.Count; i++) {
            if (_enemyPool[i].type == enemyType) {
                return RetrieveEnemyFromPool(i);
            }
        }

        NewEnemy(enemyType);

        return RetrieveEnemyFromPool(_enemyPool.Count - 1);
    }

    public Enemy RetrieveEnemyFromPool(int index) {
        Enemy desiredEnemy = _enemyPool[index];     //Saves the enemy
        _enemyPool[index] = _enemyPool[^1];         //Literally removes the enemy from the list
        _enemyPool.RemoveAt(_enemyPool.Count - 1);  //Remove the last item (that is duplicated now), to avoid swapping elements => efficient

        desiredEnemy.gameObject.SetActive(true);
        desiredEnemy.ResetSpecificVariables();
        return desiredEnemy;
    }

    #region Storing Enemies

    public void AddEnemy(Enemy enemy) {
        activeEnemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy) {
        activeEnemies.Remove(enemy);
    }

    #endregion
}


public enum EnemyType : byte {
    Minion = 0,
    MinionElite = 1,
    SporeSmall = 2,
    Wolf = 3,
}