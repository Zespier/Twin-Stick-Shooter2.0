using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour {

    public string sceneAfterSplash = "MainMenu";
    [Range(0, 5)] public float splashDuration = 3f;
    public Image fadeImage;
    public Gradient fadeColorGradient;

    private float _timeCounter = 0;

    /// <summary>
    /// Fade of the splash image
    /// </summary>
    private void Update() {

        _timeCounter += Time.deltaTime;
        fadeImage.color = fadeColorGradient.Evaluate(_timeCounter / splashDuration);

        if (_timeCounter >= splashDuration) {
            SceneManager.LoadScene(sceneAfterSplash);
        }
    }

}
