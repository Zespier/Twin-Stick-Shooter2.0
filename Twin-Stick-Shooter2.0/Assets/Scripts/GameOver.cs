using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour {

    public CanvasGroup canvasGroup;
    public float fadeTime = 2f;
    public static GameOver instance;

    private void Awake() {
        if (!instance) { instance = this; }

        Canvas_SetActive(false);
    }

    public void ShowGameOverPanel() {
        Time.timeScale = 1;
        StartCoroutine(C_ShowGameOverPanel());
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator C_ShowGameOverPanel() {

        float timer = 0;

        while (timer < fadeTime) {
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeTime);
            yield return null;
            timer += Time.deltaTime;
        }

        canvasGroup.alpha = 1;
    }

    public void Canvas_SetActive(bool active) {

        canvasGroup.alpha = active ? 1 : 0;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }


}
