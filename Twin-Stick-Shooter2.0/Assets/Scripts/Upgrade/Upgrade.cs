using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour {

    public Stats targetStats;
    public Buff upgradeType;
    public float amount = 1f;

    private UpgradeHolder upgradeHolder;

    public UpgradeHolder UpgradeHolder { get => upgradeHolder; set => upgradeHolder = value; }

    private void OnEnable() {

        if (targetStats != null) {

            targetStats.AddBuff(upgradeType, amount);

        } else {
            Debug.LogWarning("No stats to upgrade");
        }
    }

    private void OnDisable() {

        if (targetStats != null) {

            targetStats.RemoveBuff(upgradeType, amount);

        } else {
            Debug.LogWarning("No stats to upgrade");
        }
    }

}
