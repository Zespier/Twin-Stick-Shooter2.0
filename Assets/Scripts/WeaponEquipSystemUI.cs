using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponEquipSystemUI : Menu {

    public const byte TotalRam = 128;
    public List<ItemWithSpaceUI> verticalMovementItems;
    public List<ItemWithSpaceUI> horizontalMovementItems;

    private int _selectedIndex = 0;
    private byte _cursorPosition = 0;

    private Weapon HoveredWeapon => Ship.instance.weaponInventory[_selectedIndex];

    private void OnEnable() {
        InputManager.OnInterfaceNavigation += OnMoveHoveredItemVERTICALLY;
        InputManager.OnInterfaceSwitchBranch += OnMoveHoveredItemHorizontally;
    }

    private void OnDisable() {
        InputManager.OnInterfaceNavigation -= OnMoveHoveredItemVERTICALLY;
        InputManager.OnInterfaceSwitchBranch -= OnMoveHoveredItemHorizontally;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
            ActiveCanvasGroup(!IsOpen);
        }

        if (IsOpen) {
            PrepareEveryWeapon();
        }

        HandlePlacementInput();
    }

    public void PrepareEveryWeapon() {
        while (verticalMovementItems.Count < Ship.instance.weaponInventory.Count) {
            verticalMovementItems.Add(Instantiate(verticalMovementItems[0], verticalMovementItems[0].transform.parent));
        }

        while (horizontalMovementItems.Count < Ship.instance.weaponInventory.Count) {
            horizontalMovementItems.Add(Instantiate(horizontalMovementItems[0], horizontalMovementItems[0].transform.parent));
        }


        int count = Ship.instance.weaponInventory.Count;
        for (int i = 0; i < count; i++) {
            Weapon weapon = Ship.instance.weaponInventory[i];

            verticalMovementItems[i].weapon = weapon;
            horizontalMovementItems[i].weapon = weapon;

            if (weapon.equipped) {

            }
        }

        horizontalMovementItems[_cu]
    }

    public bool IsPreviewValid(Weapon weapon, byte previewStart) {
        if (!IsWithinBounds(previewStart, weapon.sizee)) {
            return false;
        }

        var overlaps = GetOverlappingWeapons(previewStart, weapon.sizee).Where(w => w != weapon).ToList();

        return overlaps.Count <= 1;
    }

    public bool IsWithinBounds(byte start, byte size) {
        return start + size <= TotalRam;
    }

    public List<Weapon> GetOverlappingWeapons(byte start, byte size) {
        byte end = (byte)(start + size - 1);

        return Ship.instance.weaponInventory.Where(w => w.equipped && !(end < w.initialIndex || start > w.EndIndex)).ToList();
    }

    public EquipResult TryPlaceWeapon(Weapon weapon, byte newStart) {
        if (!IsWithinBounds(newStart, weapon.sizee)) {
            return EquipResult.OutOfBounds;
        }

        var overlaps = GetOverlappingWeapons(newStart, weapon.sizee).Where(w => w != weapon).ToList();

        if (overlaps.Count > 1) {
            return EquipResult.OverlapMultiple;
        }

        if (overlaps.Count == 1) {
            var other = overlaps[0];
            byte oldStart = weapon.initialIndex;

            other.initialIndex = oldStart;
            weapon.initialIndex = newStart;

            weapon.equipped = true;
            return EquipResult.Swapped;
        }

        weapon.initialIndex = newStart;
        weapon.equipped = true;

        return EquipResult.Placed;
    }

    public void OptimizeLayout() {
        var equipped = Ship.instance.weaponInventory.Where(w => w.equipped).OrderBy(w => w.initialIndex).ToList();

        byte currentIndex = 0;

        foreach (var weapon in equipped) {
            weapon.initialIndex = currentIndex;
            currentIndex += weapon.sizee;
        }
    }

    void HandleVerticalInput() {

        //TODO: Qué hago con esto?
        if (Input.GetKeyDown(KeyCode.DownArrow) ||
            Input.GetKeyDown(KeyCode.UpArrow)) {
            if (HoveredWeapon.equipped)
                _cursorPosition = HoveredWeapon.initialIndex;
            else
                _cursorPosition = 0;
        }
    }

    void HandlePlacementInput() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            TryPlaceWeapon(HoveredWeapon, _cursorPosition);
        }

        if (Input.GetKeyDown(KeyCode.Backspace)) {
            HoveredWeapon.equipped = false;
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            OptimizeLayout();
        }
    }

    public void OnMoveHoveredItemVERTICALLY(InputAction.CallbackContext context) {

        Vector2 input = context.ReadValue<Vector2>();

        if (context.started) {
            if (input.y > 0) {
                //TODO: UP
                _selectedIndex--;
                if (_selectedIndex < 0) {
                    _selectedIndex = Ship.instance.weaponInventory.Count - 1;
                }

            } else if (input.y < 0) {
                //TODO: DOWN
                _selectedIndex = (_selectedIndex + 1) % Ship.instance.weaponInventory.Count;
            }
        }
    }

    public void OnMoveHoveredItemHorizontally(InputAction.CallbackContext context) {

        float input = context.ReadValue<float>();

        if (context.started) {
            if (input > 0) {
                //TODO: Move the hoveredItem one spot to the right
                if (_cursorPosition < TotalRam - 1) {
                    _cursorPosition++;
                }

            } else if (input < 0) {
                //TODO: Move the hoveredItem one spot to the left
                if (_cursorPosition > 0) {
                    _cursorPosition--;
                }
            }
        }
    }
}

public enum EquipResult {
    Placed,
    Swapped,
    OverlapMultiple,
    OutOfBounds
}