using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour {

    public RectTransform rectTransform;
    public Image icon;
    public TMP_Text description;
    public UpgradeAsset upgradeAsset;

    public void SetUpUpgradeCard(UpgradeAsset upgradeAsset) {
        this.description.text = upgradeAsset.description;
        this.icon.sprite = upgradeAsset.icon;
        this.upgradeAsset = upgradeAsset;
    }
}
