using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer : MonoBehaviour {

    public static EnemyContainer instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }
}
