using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDeTrucos : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    private void Awake() {
        Canvas_SetActive(false);
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.F1)) {
            MimicF1();
        }
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
