using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public CanvasGroup canvasGroup;

    private void Awake() {
        Canvas_SetActive(false);
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            MimicEscapeButton();
        }
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
        if (PlayerController.instance!= null && PlayerController.instance._dead) { return; }

        canvasGroup.alpha = active ? 1 : 0;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }

}
