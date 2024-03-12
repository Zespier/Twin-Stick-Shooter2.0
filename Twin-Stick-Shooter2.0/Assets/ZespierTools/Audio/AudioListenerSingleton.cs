using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerSingleton : MonoBehaviour {

    public AudioListener audioListener;

    public static AudioListenerSingleton instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

}
