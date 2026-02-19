using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuWithSelectableItems : Menu {

    public List<SelectableItemForController> items;
    public List<GroupForController> groups;
    public SelectableItemForController hoveredItem;

    void OnEnable() {
        InputManager.OnInterfaceNavigation += OnArrow;
        InputManager.OnInterfaceInteraction += OnUse;
        InputManager.OnInterfaceSwitchBranch += OnChangeBranch;
    }

    void OnDisable() {
        InputManager.OnInterfaceNavigation -= OnArrow;
        InputManager.OnInterfaceInteraction -= OnUse;
        InputManager.OnInterfaceSwitchBranch -= OnChangeBranch;
    }

    public void HoverDefault() {
        items[0].Hover();
    }

    public override void UseItem() {
        bool found = false;
        for (int i = 0; i < groups.Count; i++) {
            for (int j = 0; j < groups[i].items.Count; j++) {
                if (groups[i].items[j] == hoveredItem) {
                    groups[i].lastUsedItem = hoveredItem;
                    found = true;
                    break;
                }
            }
            if (found) {
                break;
            }
        }

        if (hoveredItem != null) {

            hoveredItem.Use();
        }
    }

    public virtual void OnUse(InputAction.CallbackContext context) {
        if (context.started) {
            UseItem();
        }
    }

    public virtual void OnArrow(InputAction.CallbackContext context) {
        Vector2 direction = context.ReadValue<Vector2>();
        if (direction.x > 0) {
            MoveOutline(OutlineDirection.Right);
        } else if (direction.x < 0) {
            MoveOutline(OutlineDirection.Left);
        } else if (direction.y > 0) {
            MoveOutline(OutlineDirection.Up);
        } else if (direction.y < 0) {
            MoveOutline(OutlineDirection.Down);
        }
    }

    public void MoveOutline(OutlineDirection direction) {

        SelectableItemForController nextSelectableItem = Controller.GetCloseSelectableItem(items, hoveredItem, direction);

        if (hoveredItem == null) {
            hoveredItem = items[0];
            return;
        }

        if (hoveredItem == nextSelectableItem) { return; }

        GroupForController selectedItemGroup = null;
        for (int i = 0; i < groups.Count; i++) {
            for (int j = 0; j < groups[i].items.Count; j++) {
                if (groups[i].items[j] == hoveredItem) {
                    selectedItemGroup = groups[i];
                    break;
                }
            }
            if (selectedItemGroup != null) {
                break;
            }
        }

        GroupForController nextSelectableItemGroup = null;
        for (int i = 0; i < groups.Count; i++) {
            for (int j = 0; j < groups[i].items.Count; j++) {
                if (groups[i].items[j] == nextSelectableItem) {
                    nextSelectableItemGroup = groups[i];
                    break;
                }
            }
            if (nextSelectableItemGroup != null) {
                break;
            }
        }

        if (selectedItemGroup != nextSelectableItemGroup) {
            nextSelectableItemGroup.HoverPreferedItem(this, nextSelectableItem);
        } else {
            nextSelectableItem.Hover(); //This will do hoveredItem = nextSelectableItem
        }
    }


    private void OnChangeBranch(InputAction.CallbackContext context) {

        float input = context.ReadValue<float>();

        if (context.started) {
            if (input > 0) {
                OnDeckMenuChangeBranchRight();
            } else if (input < 0) {
                OnDeckMenuChangeBranchLeft();
            }
        }
    }

    public void OnDeckMenuChangeBranchLeft() {
        //deckBranches[selectedBranch].OnPointerExit(null);

        //selectedBranch--;
        //if (selectedBranch < 0) {
        //    selectedBranch = 0;
        //}

        //deckBranches[selectedBranch].OnPointerEnter(null);
        //deckBranches[selectedBranch].Use();
    }

    public void OnDeckMenuChangeBranchRight() {
        //deckBranches[selectedBranch].OnPointerExit(null);

        //selectedBranch++;
        //if (selectedBranch >= deckBranches.Count) {
        //    selectedBranch = deckBranches.Count - 1;
        //}

        //deckBranches[selectedBranch].OnPointerEnter(null);
        //deckBranches[selectedBranch].Use();
    }
}
