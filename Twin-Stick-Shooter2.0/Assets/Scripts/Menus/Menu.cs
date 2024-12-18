using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public CanvasGroup canvasGroup;
    public List<Button> buttons;
    public RectTransform outline;
    public Credits credits;

    private bool _canMove = true;

    private void Awake() {
        Canvas_SetActive(true);
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
        if (canvasGroup.alpha > 0) {
            buttons[GetCurrentButtonOutlined()].onClick.Invoke();

        } else {
            credits.OnSelectButton(context);
        }
    }

    public void OnArrowMoveSelectionVertically(InputAction.CallbackContext context) {

        if (canvasGroup.alpha > 0) {

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

        } else {
            credits.OnArrowMoveSelectionVertically(context);
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
    /// Exits the game
    /// </summary>
    public void ExitGame() {
        Application.Quit();
    }

    /// <summary>
    /// Canvas group set up
    /// </summary>
    /// <param name="active"></param>
    public void Canvas_SetActive(bool active) {

        canvasGroup.alpha = active ? 1 : 0;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }

    public void Active() {
        Canvas_SetActive(true);
    }

    public void Activent() {
        Canvas_SetActive(false);

    }
}
