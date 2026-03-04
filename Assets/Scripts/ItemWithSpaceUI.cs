using UnityEngine;

public class ItemWithSpaceUI : MonoBehaviour {

    public bool scaleInX = true;
    public RectTransform rectTransform;
    public Weapon weapon;
    public RectTransform totalWeaponSpace;

    private void OnValidate() {
        if (weapon == null || !weapon.equipped) { return; }

        if (scaleInX) {
            ScaleInX();

        } else {
            ScaleInY();
        }
    }

    private void Update() {
        if (weapon == null || !weapon.equipped) { return; }

        if (scaleInX) {
            ScaleInX();

        } else {
            ScaleInY();
        }
    }

    public void ScaleInX() {
        float cellWidth = totalWeaponSpace.sizeDelta.x / 128f;

        rectTransform.anchoredPosition = new Vector2(weapon.initialIndex * cellWidth, 0);

        rectTransform.sizeDelta = new Vector2(weapon.sizee * cellWidth - 5, totalWeaponSpace.sizeDelta.y - 5);
    }

    public void ScaleInY() {
        float cellWidth = totalWeaponSpace.sizeDelta.x / 128f;

        rectTransform.anchoredPosition = new Vector2(0, weapon.initialIndex * cellWidth);

        rectTransform.sizeDelta = new Vector2(totalWeaponSpace.sizeDelta.x - 5, weapon.sizee * cellWidth - 5);
    }
}
