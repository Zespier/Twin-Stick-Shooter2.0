using UnityEngine;

public class RamFusionVisualizer : MonoBehaviour {
    [Header("Triangle Sides")]
    public RectTransform ramA;
    public RectTransform ramB;
    public RectTransform ramC;

    [Header("Result RAM (Center)")]
    public RectTransform resultRam;

    [Header("Settings")]
    public float thickness = 20f;
    public float baseValue = 5f;          // Minimum RAM value
    public float maxValue = 10f;          // Maximum RAM value
    public float outwardMoveMultiplier = 40f; // How much movement per extra RAM

    private float valueA;
    private float valueB;
    private float valueC;

    public void SetRamValues(float a, float b, float c) {
        valueA = Mathf.Clamp(a, baseValue, maxValue);
        valueB = Mathf.Clamp(b, baseValue, maxValue);
        valueC = Mathf.Clamp(c, baseValue, maxValue);

        Build();
    }

    void Build() {
        float result = CalculateFusion(valueA, valueB, valueC);

        // Set center width
        resultRam.sizeDelta = new Vector2(result * 20f, thickness);

        // Calculate how much bigger than minimum
        float expansionAmount = result - (baseValue + 1f);

        MoveTriangleOutward(expansionAmount);
    }

    float CalculateFusion(float a, float b, float c) {
        return ((a + b + c) / 3f) + 1f;
    }

    void MoveTriangleOutward(float expansionAmount) {
        float offset = expansionAmount * outwardMoveMultiplier;

        // Each side moves away from center along its perpendicular direction

        MoveSideOutward(ramA, offset);
        MoveSideOutward(ramB, offset);
        MoveSideOutward(ramC, offset);
    }

    void MoveSideOutward(RectTransform side, float offset) {
        // Get outward direction (perpendicular to the rectangle's right vector)
        Vector3 outward = Vector3.Cross(side.right, Vector3.forward).normalized;

        side.anchoredPosition = outward * offset;
    }
}

public class RAM : InventoryItem {
    public Tier tier;
    public int ramSpace = 5;
    public override Vector2 SizeInUI => base.SizeInUI;
    public override float SellValue => 1000 * (int)tier;
}
