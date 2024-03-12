using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public AudioMixer audioMixer;



    #region Volume management

    public void VolumeMaster(float volume) {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    public void VolumeSounds(float volume) {
        audioMixer.SetFloat("Sounds", Mathf.Log10(volume) * 20);
    }

    public void VolumeMusic(float volume) {
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    #endregion
}
