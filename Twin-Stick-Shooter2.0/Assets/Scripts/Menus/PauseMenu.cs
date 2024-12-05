using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    public CanvasGroup canvasGroup;
    public List<Button> buttons;
    public RectTransform outline;

    private bool _canMove = true;

    public static PauseMenu instance;
    private void Awake() {
        if (!instance) { instance = null; }
        Canvas_SetActive(false);
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            MimicEscapeButton();
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
    /// On escape, opesn the menu
    /// </summary>
    public void MimicEscapeButton() {
        if (PlayerController.instance._dead) { return; }

        PauseGame(canvasGroup.alpha == 0 ? true : false);
        Canvas_SetActive(canvasGroup.alpha == 0 ? true : false);
    }

    /// <summary>
    /// Pauses the game
    /// </summary>
    /// <param name="pause"></param>
    public void PauseGame(bool pause) {
        if (PlayerController.instance != null && PlayerController.instance._dead) { Time.timeScale = 1; return; }

        Time.timeScale = pause ? 0 : 1;
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
