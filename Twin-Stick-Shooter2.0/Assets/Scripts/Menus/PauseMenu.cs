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
            PauseGame(canvasGroup.alpha == 0 ? true : false);
            Canvas_SetActive(canvasGroup.alpha == 0 ? true : false);
        }
    }

    public void MimicEscapeButton() {
        PauseGame(canvasGroup.alpha == 0 ? true : false);
        Canvas_SetActive(canvasGroup.alpha == 0 ? true : false);
    }

    public void PauseGame(bool pause) {
        Time.timeScale = pause ? 0 : 1;
    }

    public void Canvas_SetActive(bool active) {
        canvasGroup.alpha = active ? 1 : 0;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }

}
