using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spore_Egg : Generator, IDamageable {

    public Vector3Int generationPerLevel = new Vector3Int(1, 2, 4);
    public float hp = 1000f;

    public override GameObject SporePrefab => GetNextSporePrefab();

    public override int NextAmountToGenerate => GetNextAmountOfSpores();

    protected override void Update() {
        if (Spore_Mind.instance.TotalSpores >= 100) { return; }

        if (_spawnTimer >= (Spore_Mind.instance.spore_Zone_SpawningTime/* - ReduceSpawnTimeBasedOnSpore_Zones()) / _sizeRatio*/)) {
            Spawn();
            CheckDeath();
            NextLevel();
        }

        _spawnTimer += Time.deltaTime;
    }

    /// <summary>
    /// Reaches next level 
    /// </summary>
    private void NextLevel() {
        if ((int)Size >= 2) { return; }
        Size = (Spore_ZoneSize)((int)Size + 1);
    }

    /// <summary>
    /// Gets the next enemy that is going to spawn
    /// </summary>
    /// <returns></returns>
    private GameObject GetNextSporePrefab() {

        return Size switch {
            Spore_ZoneSize.small => Spore_Mind.instance.spore_Small,
            Spore_ZoneSize.medium => Spore_Mind.instance.spore_Normal,
            Spore_ZoneSize.big => Spore_Mind.instance.spore_Big,

            _ => Spore_Mind.instance.spore_Small,
        };
    }

    /// <summary>
    /// Gets the next amount of enemies to spawn
    /// </summary>
    /// <returns></returns>
    private int GetNextAmountOfSpores() {

        return Size switch {
            Spore_ZoneSize.small => generationPerLevel.z,
            Spore_ZoneSize.medium => generationPerLevel.y,
            Spore_ZoneSize.big => generationPerLevel.x,

            _ => 1,
        };
    }

    /// <summary>
    /// Takes damage
    /// </summary>
    /// <param name="position"></param>
    /// <param name="damage"></param>
    /// <param name="crit"></param>
    /// <param name="damageType"></param>
    public void TakeDamage(Vector3 position, float damage, bool crit, string damageType) {
        FeedbackController.instance.DamageFeedBack(position, damage, crit, damageType);
        hp -= damage;
        CheckDeath();
    }

    /// <summary>
    /// Checks if it died
    /// </summary>
    private void CheckDeath() {
        if (hp < 0 || (int)Size >= 2) {
            Deactivate();
        }
    }

    /// <summary>
    /// Deactivate the enemy
    /// </summary>
    public void Deactivate() {
        gameObject.SetActive(false);
    }
}
