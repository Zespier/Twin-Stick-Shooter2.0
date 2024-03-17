using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public AudioMixer audioMixer;
    public AudioPool audioPool;
    public List<AudioClip> bulletImpacClips;
    public float pitchRange = 0.1f;

    private AudioOptimization _audioSource;

    private void OnEnable() {
        Events.OnBulletImpact += BulletImpactSound;
    }

    private void OnDisable() {
        Events.OnBulletImpact -= BulletImpactSound;
    }

    public void BulletImpactSound(Vector3 position) {
        _audioSource = audioPool.GetAvailableSource();
        _audioSource.Activate(position);
        _audioSource.audioSource.clip = bulletImpacClips[Random.Range(0, bulletImpacClips.Count)];
        _audioSource.audioSource.pitch = Random.Range(1 - pitchRange, 1 + pitchRange);
        _audioSource.audioSource.Play();
    }

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
