using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class YouWin : MonoBehaviour {
    public CanvasGroup canvasGroup;
    public float fadeTime = 2f;
    public List<Button> buttons;
    public RectTransform outline;

    private bool _canMove = true;

    public static YouWin instance;
    private void Awake() {
        if (!instance) { instance = this; }

        Canvas_SetActive(false);
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
    /// Shows the game over panle
    /// </summary>
    public void ShowYouWinPanel() {
        Time.timeScale = 1;
        StartCoroutine(C_ShowYouWinPanel());
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Animation of the game over panel
    /// </summary>
    /// <returns></returns>
    private IEnumerator C_ShowYouWinPanel() {

        float timer = 0;

        while (timer < fadeTime) {
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeTime);
            yield return null;
            timer += Time.deltaTime;
        }

        canvasGroup.alpha = 1;
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

}
