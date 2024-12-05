using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuDeTrucos : MonoBehaviour {
    public CanvasGroup canvasGroup;
    public List<Button> buttons;
    public RectTransform outline;

    private bool _canMove = true;

    public static MenuDeTrucos instance;
    private void Awake() {
        if (!instance) { instance = this; }
        Canvas_SetActive(false);
    }

    private void Update() {

        if (YouWin.instance.canvasGroup.alpha == 1) {
            Canvas_SetActive(false);
        }

        if (UpgradeCardManager.instance.canvas.gameObject.activeSelf) {
            Canvas_SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.F1)) {
            MimicF1();
        }
    }

    private IEnumerator C_DontMoveMoreThanOneTime() {
        _canMove = false;
        yield return null;
        _canMove = true;
    }

    public int GetCurrentButtonOutlined() {

        for (int i = 0; i < buttons.Count; i++) {
            if (Vector3.Distance(buttons[i].image.rectTransform.position, outline.position) < 10) {
                return i;
            }
        }

        Debug.LogError("OU shiet");
        return 0;
    }

    public void OnSelectButton(InputAction.CallbackContext context) {
        if (canvasGroup.alpha == 1) {
            buttons[GetCurrentButtonOutlined()].onClick.Invoke();
        }
    }

    public void OnArrowMoveSelectionVertically(InputAction.CallbackContext context) {

        if (canvasGroup.alpha == 0) { return; }

        Vector2 direction = context.ReadValue<Vector2>();
        if (direction.x > 0) {
            //MoveOutline(OutlineDirection.Right);
        } else if (direction.x < 0) {
            //MoveOutline(OutlineDirection.Left);
        } else if (direction.y > 0) {
            MoveOutlineUp();
        } else if (direction.y < 0) {
            MoveOutlineDown();
        }
    }

    public void MoveOutlineUp() {
        if (!_canMove) { return; }

        StartCoroutine(C_DontMoveMoreThanOneTime());

        int currentIndex = GetCurrentButtonOutlined() - 1;
        if (currentIndex < 0) { currentIndex = buttons.Count - 1; }

        outline.position = buttons[currentIndex].image.rectTransform.position;
    }

    public void MoveOutlineDown() {
        if (!_canMove) { return; }

        StartCoroutine(C_DontMoveMoreThanOneTime());

        int currentIndex = (GetCurrentButtonOutlined() + 1) % buttons.Count;

        outline.position = buttons[currentIndex].image.rectTransform.position;
    }

    /// <summary>
    /// Opens the tricks menu
    /// </summary>
    public void MimicF1() {
        if (PlayerController.instance._dead) { return; }

        Canvas_SetActive(canvasGroup.alpha == 0 ? true : false);
    }

    /// <summary>
    /// Set up for hte canvas group
    /// </summary>
    /// <param name="active"></param>
    public void Canvas_SetActive(bool active) {
        if (PlayerController.instance != null && PlayerController.instance._dead) { return; }

        canvasGroup.alpha = active ? 1 : 0;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }

}
