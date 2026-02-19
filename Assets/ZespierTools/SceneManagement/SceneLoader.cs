using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    private void Awake() {
        Time.timeScale = 1.0f;
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
