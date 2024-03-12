using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SaveDataSystem : MonoBehaviour {
    public string savePath = "DataUser.dat";
    public static DataUser DataUser;

    public static SaveDataSystem instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }

        //  https://stackoverflow.com/questions/2507808/how-to-check-whether-a-file-is-empty-or-not
        //   To fix if the file is blank

        DataUser = IOData.Load(savePath) as DataUser;
        if (DataUser == null) {
            InitializeData();
        }
    }

    [ContextMenu("Reset Data")]
    public void ResetData() {
        InitializeData();
    }

    public void Save() {
        IOData.Save(savePath, DataUser);
    }

    /// <summary>
    /// Maybe in the future we need to separate them
    /// </summary>
    public void InitializeData() {
        DataUser = new() {
            test = 0,
            upgradesSaved = new List<UpgradeSaved>(),
        };

        IOData.Save(savePath, DataUser);
    }

}

[System.Serializable]
public class DataUser {
    public int test;
    public List<UpgradeSaved> upgradesSaved;


    public void SaveUpgrade(Upgrade upgrade) {
        UpgradeSaved newSave = new UpgradeSaved((int)upgrade.upgradeType, upgrade.amount);
        upgradesSaved.Add(newSave);
    }

    public void RemoveUpgrade(Upgrade upgrade) {
        for (int i = 0; i < upgradesSaved.Count; i++) {
            if (upgradesSaved[i].type == (int)upgrade.upgradeType && upgradesSaved[i].amount == upgrade.amount) {
                upgradesSaved.RemoveAt(i);
                break;
            }
        }
    }

    public void AreUpgradesSavedCorrectly(List<Upgrade> upgrades) {
        List<UpgradeSaved> temp = new List<UpgradeSaved>();
        for (int i = 0; i < upgradesSaved.Count; i++) {
            temp.Add(upgradesSaved[i]);
        }

        for (int i = 0; i < upgrades.Count; i++) {
            bool thisUpgradeWasSaved = false;

            for (int n = 0; n < temp.Count; n++) {
                if (temp[n].type == (int)upgrades[i].upgradeType && temp[n].amount == upgrades[i].amount) {
                    thisUpgradeWasSaved = true;
                    temp.RemoveAt(n);
                    break;
                }
            }

            if (!thisUpgradeWasSaved) {
                OverwriteUpgradesSaved(upgrades);
                break;
            }
        }
    }

    private void OverwriteUpgradesSaved(List<Upgrade> upgrades) {
        upgradesSaved.Clear();

        for (int i = 0; i < upgrades.Count; i++) {
            UpgradeSaved newSave = new UpgradeSaved((int)upgrades[i].upgradeType, upgrades[i].amount);
            upgradesSaved.Add(newSave);
        }

        SaveDataSystem.instance.Save();
    }
}

public struct UpgradeSaved {
    public int type;
    public float amount;

    public UpgradeSaved(int type, float amount) {
        this.type = type;
        this.amount = amount;
    }
}
