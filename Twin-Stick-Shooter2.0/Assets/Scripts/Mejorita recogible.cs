using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mejoritarecogible : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            UpgradeCardManager.instance.Open();
            Destroy(gameObject);
        }
    }
}
