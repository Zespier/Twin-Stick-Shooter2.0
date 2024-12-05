using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Waves/Wave")]
public class Wave : ScriptableObject {

    public float hpIncreasePercentage;
    public float atkIncreasePercentage;

    public List<WaveEnemy> initialEnemies;
    public List<WaveEnemy> enemiesAdditional;
    public List<ConditionToSpawn> conditionsToSpawn;
    public List<int> initialSpawnPointList;
    public List<int> additionalSpawnPointList;

    public string dialogueTriggerName;
}

[System.Serializable]
public struct ConditionToSpawn {

    public ConditionType condition;
    public WaveEnemy dependantOfThisEnemy;
    public float healthRemaining;

}

[System.Serializable]
public enum ConditionType {
    AllDead,
    EnemyDead,
    Damaged
}