using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePickable : MonoBehaviour {

    public List<Upgrade> upgrades;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            for (int i = 0; i < upgrades.Count; i++) {

                PlayerStats.instance.AddBuff(upgrades[i].upgradeType, upgrades[i].amount);
            }

            Destroy(gameObject);
        }
    }
}
