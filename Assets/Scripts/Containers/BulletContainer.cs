using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletContainer : MonoBehaviour {

    public static BulletContainer instance;
    private void Awake() {
        if (!instance) {
            instance = this;
        }
    }
}
