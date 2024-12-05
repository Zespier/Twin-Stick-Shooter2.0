using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeCardManager : MonoBehaviour {

    public GameObject canvas;
    public List<UpgradeCard> upgradeCards;
    public List<UpgradeAsset> allUpgradeAssets;
    public RectTransform outline;

    public static UpgradeCardManager instance;
    private void Awake() {
        if (!instance) { instance = this; }

        canvas.gameObject.SetActive(false);
    }

    public void Open() {

        List<UpgradeAsset> randomUpgrades = new List<UpgradeAsset>(capacity: 3);

        for (int i = 0; i < 3; i++) {
            randomUpgrades.Add(allUpgradeAssets[Random.Range(0, allUpgradeAssets.Count)]);
        }

        for (int i = 0; i < randomUpgrades.Count; i++) {
            upgradeCards[i].SetUpUpgradeCard(randomUpgrades[i]);
        }
        canvas.SetActive(true);
    }

    public void ChooseMode(InputAction.CallbackContext context) {
        if (canvas.activeSelf) {
            ChooseMode();
            canvas.gameObject.SetActive(false);
        }
    }

    public void ChooseMode() {
        int currentImage = GetCurrentImageOutlined();

        for (int i = 0; i < upgradeCards[currentImage].upgradeAsset.upgradeTypes.Count; i++) {
            PlayerStats.instance.AddBuff(upgradeCards[currentImage].upgradeAsset.upgradeTypes[i], upgradeCards[currentImage].upgradeAsset.upgradeAmounts[i]);
        }
    }

    public void OnArrowMoveSelectionHorizontally(InputAction.CallbackContext context) {

        if (!canvas.gameObject.activeSelf) { return; }

        Vector2 direction = context.ReadValue<Vector2>();
        if (direction.x > 0) {
            MoveOutlineRight();
        } else if (direction.x < 0) {
            MoveOutlineLeft();
        } else if (direction.y > 0) {
            //MoveOutlineUp();
        } else if (direction.y < 0) {
            //MoveOutlineDown();
        }
    }

    public void MoveOutlineRight() {
        int currentUpgradeCardIndex = GetCurrentImageOutlined() - 1;
        if (currentUpgradeCardIndex < 0) { currentUpgradeCardIndex = upgradeCards.Count - 1; }

        outline.position = upgradeCards[currentUpgradeCardIndex].rectTransform.position;
    }

    public void MoveOutlineLeft() {
        int currentUpgradeCardIndex = (GetCurrentImageOutlined() + 1) % upgradeCards.Count;

        outline.position = upgradeCards[currentUpgradeCardIndex].rectTransform.position;
    }

    public int GetCurrentImageOutlined() {

        for (int i = 0; i < upgradeCards.Count; i++) {
            if (Vector3.Distance(upgradeCards[i].rectTransform.position, outline.position) < 10) {
                return i;
            }
        }

        Debug.LogError("OU shiet");
        return 0;
    }
}
