using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouWin : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeTime = 2f;

    public static YouWin instance;
    private void Awake() {
        if (!instance) { instance = this; }

        Canvas_SetActive(false);
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
