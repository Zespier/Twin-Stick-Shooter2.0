using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Credits : MonoBehaviour {

    public List<Button> buttons;
    public RectTransform outline;

    private bool _canMove = true;

    private void Awake() {
        gameObject.SetActive(false);
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
        Debug.Log("A");
        if (gameObject.activeSelf) {
            buttons[GetCurrentButtonOutlined()].onClick.Invoke();
        }
    }

    public void OnArrowMoveSelectionVertically(InputAction.CallbackContext context) {
        Debug.Log("A");

        if (gameObject.activeSelf) {

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
    }

    public void MoveOutlineUp() {

        int currentIndex = GetCurrentButtonOutlined() - 1;
        if (currentIndex < 0) { currentIndex = buttons.Count - 1; }

        outline.position = buttons[currentIndex].image.rectTransform.position;
    }

    public void MoveOutlineDown() {

        int currentIndex = (GetCurrentButtonOutlined() + 1) % buttons.Count;

        outline.position = buttons[currentIndex].image.rectTransform.position;
    }
}
