using UnityEngine;

public class ItemWithSpaceUI : MonoBehaviour {

    public RectTransform rectTransform;
    public Weapon weapon;
    public RectTransform totalWeaponSpace;

    private void OnValidate() {
        if (weapon == null || !weapon.equipped) { return; }
        float cellWidth = totalWeaponSpace.sizeDelta.x / 128f;

        rectTransform.anchoredPosition = new Vector2(weapon.initialIndex * cellWidth, 0);

        rectTransform.sizeDelta = new Vector2(weapon.sizee * cellWidth - 5, totalWeaponSpace.sizeDelta.y - 5);
    }

    private void Update() {
        if (weapon == null || !weapon.equipped) { return; }
        float cellWidth = totalWeaponSpace.sizeDelta.x / 128f;

        rectTransform.anchoredPosition = new Vector2(weapon.initialIndex * cellWidth, 0);

        rectTransform.sizeDelta = new Vector2(weapon.sizee * cellWidth, totalWeaponSpace.sizeDelta.y - 5);
    }
}
