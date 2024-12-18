using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour {

  
    /// <summary>
    /// Internal events of the game
    /// </summary>

    public static Action<SpawnArea> OnEnterSpawnArea; //TODO: Waves should stop even when you don't exit the zone, but in other cases likes going to the menu, etc...
    public static Action OnExitSpawnArea;
    public static Action<Wave> OnWaveStarted;
    public static Action<Wave> OnWaveEnded;

    public static Action<Vector3> OnTargetMove;
    //---------------Enemies---------------------
    public static Action<Enemy> OnEnemyDeath;

}
