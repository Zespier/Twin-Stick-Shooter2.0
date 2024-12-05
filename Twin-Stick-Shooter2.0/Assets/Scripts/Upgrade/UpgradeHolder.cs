using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHolder : MonoBehaviour {

    public Upgrade upgradePrefab;
    public List<Upgrade> upgrades = new List<Upgrade>();

    public Upgrade TempUpgrade { get; set; }

    /// <summary>
    /// Adds an upgrade
    /// </summary>
    /// <param name="upgrade"></param>
    public void AddUpgrade(Buff buffType, float amount, Stats statsToBuff) {

        bool found = false;

        for (int i = 0; i < upgrades.Count; i++) {

            if (!upgrades[i].enabled && upgrades[i].upgradeType == buffType && upgrades[i].amount == amount) {
                upgrades[i].targetStats = statsToBuff;
                upgrades[i].enabled = true;
                found = true;
                break;
            }
        }

        if (!found) {

            TempUpgrade = UpgradeContainer.instance.GetUpgrade(); //Get an existing one
            if (TempUpgrade == null) {
                TempUpgrade = Instantiate(upgradePrefab, transform); //If not generate new
            }

            TempUpgrade.upgradeType = buffType;
            TempUpgrade.amount = amount;
            TempUpgrade.targetStats = statsToBuff;
            if (!TempUpgrade.enabled) { TempUpgrade.enabled = true; }

            upgrades.Add(TempUpgrade);
        }
    }

    /// <summary>
    /// removes an upgrade
    /// </summary>
    /// <param name="upgrade"></param>
    public void RemoveUpgrade(Buff buffType, float amount) {

        for (int i = 0; i < upgrades.Count; i++) {

            if (upgrades[i].upgradeType == buffType && upgrades[i].amount == amount && upgrades[i] != null) {
                upgrades[i].enabled = false;
                break;
            }
        }
    }

}
