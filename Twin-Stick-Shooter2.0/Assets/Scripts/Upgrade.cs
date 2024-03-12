using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour {

    public UpgradeType upgradeType;
    public float amount = 1f;

    private UpgradeHolder upgradeHolder;

    public UpgradeHolder UpgradeHolder { get => upgradeHolder; set => upgradeHolder = value; }

    private void OnEnable() {
        PlayerStats.instance.AddBuff(upgradeType, amount);
    }

    private void OnDisable() {
        PlayerStats.instance.RemoveBuff(upgradeType, amount);
    }

}

public enum UpgradeType {
    baseDamage,
    damagePercentage,
    flatDamage,
}