using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Asset", menuName = "Upgrade Asset")]
public class UpgradeAsset : ScriptableObject {

    public Sprite icon;
    public string description;

    public List<UpgradeType> upgradeTypes = new List<UpgradeType>();
    public List<int> upgradeAmounts = new List<int>();
}
