using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spore_Mind : MonoBehaviour {

    public GameObject spore_Big;
    public GameObject spore_Normal;
    public GameObject spore_Small;
    public Spore_Zone spore_Zone;

    public float spore_Zone_MediumSize = 1.5f;
    public float spore_Zone_BigSize = 2f;
    public float spore_Zone_SpawningTime = 3f;
    public float spore_Zone_lifetime = 10f;

    [Header("Plague Control")]
    public float hierarchyRatio = 4f;
    [Space]
    public int totalSpore_Zones;
    public int totalSpore_Small;
    public int totalSpore_Normal;
    public int totalSpore_Big;

    public int TotalSpore_Small { get => totalSpore_Small; set { totalSpore_Small = value; CheckSporeFusion(); } }
    public int TotalSpore_Normal { get => totalSpore_Normal; set { totalSpore_Normal = value; CheckSporeFusion(); } }
    public int TotalSpore_Big { get => totalSpore_Big; set { totalSpore_Big = value; CheckSporeFusion(); } }

    public int TotalSpores { get => TotalSpore_Small + TotalSpore_Normal + TotalSpore_Big + totalSpore_Zones; }

    public static Spore_Mind instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void OnEnable() {
        Events.OnEnemyDeath += SporeDeath;
    }

    private void OnDisable() {
        Events.OnEnemyDeath -= SporeDeath;
    }

    /// <summary>
    /// Checks if spores can fuse
    /// </summary>
    public void CheckSporeFusion() {
        if (TotalSpore_Normal * hierarchyRatio >= TotalSpore_Small) {
            //TODO: Small fusion => normal

        }
        if (TotalSpore_Big * hierarchyRatio >= TotalSpore_Normal) {
            //TODO: Normal fusion => Big
        }

        //Fusionar si TotalSpore_Big * hierarchyRatio^2 >= TotalSpore_Small no tendría sentido, porque eso significa que los medianos ya se habrían fusionado para hcer uno grande.
        if (/*Already fusing  */TotalSpore_Big * hierarchyRatio * hierarchyRatio >= TotalSpore_Normal) {
            //TODO: Small fusion => Big

        }
    }

    /// <summary>
    /// Spawns a spore
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    public void SpawnSpore(GameObject prefab, Vector3 position) {

        if (TotalSpores >= 100) { return; }
        position = (Vector2)position + RandomPosition.Circle_2D(3, true);
        Instantiate(prefab, position, Quaternion.identity, EnemyContainer.instance.transform);
    }

    /// <summary>
    /// Spawns a spore_zone
    /// </summary>
    /// <param name="position"></param>
    /// <param name="spore_ZoneSize"></param>
    public void Spore_Zone(Vector3 position, Spore_ZoneSize spore_ZoneSize) {

        Spore_Zone newSpore_zone = Instantiate(spore_Zone, position, Quaternion.identity, EnemyContainer.instance.transform);
        newSpore_zone.Size = spore_ZoneSize;
    }

    /// <summary>
    /// Dpenes on the spore that died, something can happen, tpically spawn more spores
    /// </summary>
    /// <param name="spore"></param>
    public void SporeDeath(Enemy spore) {

        Enum.TryParse(spore.GetType().ToString(), out SporeType sporeType);

        switch (sporeType) {
            case SporeType.none:
                break;
            case SporeType.Spore_Small:
                break;
            case SporeType.Spore_Normal:
                SpawnSpore(spore_Small, spore.transform.position);
                Spore_Zone(spore.transform.position, Spore_ZoneSize.small);
                break;
            case SporeType.Spore_Big:
                SpawnSpore(spore_Normal, spore.transform.position);
                SpawnSpore(spore_Normal, spore.transform.position);
                Spore_Zone(spore.transform.position, Spore_ZoneSize.big);
                break;
            default:
                break;
        }
    }
}

public enum SporeType {
    none,
    Spore_Small,
    Spore_Normal,
    Spore_Big,
}

