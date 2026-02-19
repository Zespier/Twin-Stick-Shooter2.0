using System.Collections.Generic;
using UnityEngine;

public static class Controller {

    public const float marginForControllerOutline = 50;

    #region Controller Recognition

    public static bool IsControllerConnected() {
        string[] controllers = Input.GetJoystickNames();

        foreach (var controller in controllers) {
            if (!string.IsNullOrEmpty(controller)) {
                return true;
            }
        }

        return false;
    }

    public static ControllerType GetControllerType() {
        string[] controllers = Input.GetJoystickNames();

        foreach (var controller in controllers) {
            if (!string.IsNullOrEmpty(controller)) {
                if (controller.ToLower().Contains("xbox")) {
                    return ControllerType.xbox;
                }
                if (controller.ToLower().Contains("wireless") || controller.ToLower().Contains("pc")) {
                    return ControllerType.playstation;
                }

                return ControllerType.notDefined;
            }
        }

        // esto b�sicamente significa teclado y rat�n
        return ControllerType.noController;
    }

    #endregion

    #region Controller Movement Trought Screen


    public static SelectableItemForController GetPosibleSelectableItem(List<SelectableItemForController> items, SelectableItemForController outlinedItem, OutlineDirection direction) {


        float closestRowHeightDifferenceToTheRight = float.MaxValue;
        float closestRowHeightDifferenceToTheLeft = float.MaxValue;
        float closestColumnHeightDifferenceToTheDown = float.MaxValue;
        float closestColumnHeightDifferenceToTheUp = float.MaxValue;
        for (int i = 0; i < items.Count; i++) {
            if (items[i] == outlinedItem) { continue; }

            if (items[i].rectTransform.position.x > outlinedItem.rectTransform.position.x && Mathf.Abs(outlinedItem.rectTransform.position.y - items[i].rectTransform.position.y) < closestRowHeightDifferenceToTheRight) {
                closestRowHeightDifferenceToTheRight = Mathf.Abs(outlinedItem.rectTransform.position.y - items[i].rectTransform.position.y);
            }
            if (items[i].rectTransform.position.x < outlinedItem.rectTransform.position.x && Mathf.Abs(outlinedItem.rectTransform.position.y - items[i].rectTransform.position.y) < closestRowHeightDifferenceToTheLeft) {
                closestRowHeightDifferenceToTheLeft = Mathf.Abs(outlinedItem.rectTransform.position.y - items[i].rectTransform.position.y);
            }
            if (items[i].rectTransform.position.y < outlinedItem.rectTransform.position.y && Mathf.Abs(outlinedItem.rectTransform.position.x - items[i].rectTransform.position.x) < closestColumnHeightDifferenceToTheDown) {
                closestColumnHeightDifferenceToTheDown = Mathf.Abs(outlinedItem.rectTransform.position.x - items[i].rectTransform.position.x);
            }
            if (items[i].rectTransform.position.y > outlinedItem.rectTransform.position.y && Mathf.Abs(outlinedItem.rectTransform.position.x - items[i].rectTransform.position.x) < closestColumnHeightDifferenceToTheUp) {
                closestColumnHeightDifferenceToTheUp = Mathf.Abs(outlinedItem.rectTransform.position.x - items[i].rectTransform.position.x);
            }
        }

        SelectableItemForController leftSlot = outlinedItem;
        SelectableItemForController rightSlot = outlinedItem;
        SelectableItemForController downSlot = outlinedItem;
        SelectableItemForController upSlot = outlinedItem;

        for (int i = 0; i < items.Count; i++) {
            if (items[i] == outlinedItem) { continue; }

            float heightDifference = Mathf.Abs(items[i].rectTransform.position.y - outlinedItem.rectTransform.position.y);
            if (items[i].rectTransform.position.x < leftSlot.rectTransform.position.x && Mathf.Abs(heightDifference - closestRowHeightDifferenceToTheLeft) < marginForControllerOutline) {
                leftSlot = items[i];
            }
            if (items[i].rectTransform.position.x > rightSlot.rectTransform.position.x && Mathf.Abs(heightDifference - closestRowHeightDifferenceToTheRight) < marginForControllerOutline) {
                rightSlot = items[i];
            }

            float widthDifference = Mathf.Abs(items[i].rectTransform.position.x - outlinedItem.rectTransform.position.x);
            if (items[i].rectTransform.position.y < downSlot.rectTransform.position.y && Mathf.Abs(widthDifference - closestColumnHeightDifferenceToTheDown) < marginForControllerOutline) {
                downSlot = items[i];
            }
            if (items[i].rectTransform.position.y > upSlot.rectTransform.position.y && Mathf.Abs(widthDifference - closestColumnHeightDifferenceToTheUp) < marginForControllerOutline) {
                upSlot = items[i];
            }
        }

        SelectableItemForController closestLeftSlot = leftSlot;
        SelectableItemForController closestRightSlot = rightSlot;
        SelectableItemForController closestDownSlot = downSlot;
        SelectableItemForController closestUpSlot = upSlot;

        for (int i = 0; i < items.Count; i++) {
            if (items[i] == outlinedItem) { continue; }

            float heightDifference = Mathf.Abs(items[i].rectTransform.position.y - outlinedItem.rectTransform.position.y);
            if (items[i].rectTransform.position.x < outlinedItem.rectTransform.position.x && items[i].rectTransform.position.x > closestLeftSlot.rectTransform.position.x && Mathf.Abs(heightDifference - closestRowHeightDifferenceToTheLeft) < marginForControllerOutline) {
                closestLeftSlot = items[i];
            }
            if (items[i].rectTransform.position.x > outlinedItem.rectTransform.position.x && items[i].rectTransform.position.x < closestRightSlot.rectTransform.position.x && Mathf.Abs(heightDifference - closestRowHeightDifferenceToTheRight) < marginForControllerOutline) {
                closestRightSlot = items[i];
            }

            float widthDifference = Mathf.Abs(items[i].rectTransform.position.x - outlinedItem.rectTransform.position.x);
            if (items[i].rectTransform.position.y < outlinedItem.rectTransform.position.y && items[i].rectTransform.position.y > closestDownSlot.rectTransform.position.y && Mathf.Abs(widthDifference - closestColumnHeightDifferenceToTheDown) < marginForControllerOutline) {
                closestDownSlot = items[i];
            }
            if (items[i].rectTransform.position.y > outlinedItem.rectTransform.position.y && items[i].rectTransform.position.y < closestUpSlot.rectTransform.position.y && Mathf.Abs(widthDifference - closestColumnHeightDifferenceToTheUp) < marginForControllerOutline) {
                closestUpSlot = items[i];
            }
        }

        switch (direction) {
            case OutlineDirection.Left:
                return leftSlot;
            case OutlineDirection.Right:
                return rightSlot;
            case OutlineDirection.Down:
                return downSlot;
            case OutlineDirection.Up:
                return upSlot;
            case OutlineDirection.ClosestLeft:
                return closestLeftSlot;
            case OutlineDirection.ClosestRight:
                return closestRightSlot;
            case OutlineDirection.ClosestDown:
                return closestDownSlot;
            case OutlineDirection.ClosestUp:
                return closestUpSlot;
            default:
                return outlinedItem;
        }
    }

    public static SelectableItemForController GetCloseSelectableItem(List<SelectableItemForController> items, SelectableItemForController outlinedItem, OutlineDirection direction) {
        if (outlinedItem == null) {
            outlinedItem = items[0];
            return outlinedItem;
        }

        switch (direction) {

            case OutlineDirection.Left:
                SelectableItemForController closestLeftSlot = GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.ClosestLeft);
                if (outlinedItem.canJumpToTheOtherSide && closestLeftSlot == outlinedItem) {
                    return GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.Right);
                } else {
                    return closestLeftSlot;
                }
            case OutlineDirection.Right:
                SelectableItemForController closestRightSlot = GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.ClosestRight);
                if (outlinedItem.canJumpToTheOtherSide && closestRightSlot == outlinedItem) {
                    return GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.Left);
                } else {
                    return closestRightSlot;
                }
            case OutlineDirection.Down:
                SelectableItemForController closestDownSlot = GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.ClosestDown);
                if (outlinedItem.canJumpToTheOtherSide && closestDownSlot == outlinedItem) {
                    return GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.Up);
                } else {
                    return closestDownSlot;
                }
            case OutlineDirection.Up:
                SelectableItemForController closestUpSlot = GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.ClosestUp);
                if (outlinedItem.canJumpToTheOtherSide && closestUpSlot == outlinedItem) {
                    return GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.Down);
                } else {
                    return closestUpSlot;
                }
            default:
                return outlinedItem;
        }
    }

    #endregion
}

public enum ControllerType : byte {
    notDefined,
    xbox,
    playstation,
    noController,
}

public enum OutlineDirection : byte {
    Left,
    Right,
    Down,
    Up,
    ClosestLeft,
    ClosestRight,
    ClosestDown,
    ClosestUp,
}