/*
 * Tips for audio optimization:
 * - SPATIAL BLEND: 
 *      The spatial blend sets how much the audio is going to be affected by 3D calculations (attenuation, doppler, etc.).
 *      If the audio source gets closer to the audio listener, a good practice would be to lerp into 2D to benefit from 2D audio properties
 *      - Optimization => Set the audio clip to "Force Mono". If the spatial blend is set to 1 (3D), then the audio is being reproduced as mono, because the two audio channels originate from the same spot.
 *      _ Possible problems => If the audio gets too loud deselect Normalize.
 *      
 * - Checking the distance while is playing
 * - Always pause the audio source istead of enable, maybe this reproduces faster the audios.
 * - If an audio is too far away in the moment of instantiation, it would be cool to use the feature of the virtual voices, still reproducing and come back alive as if it was playing, but I don't know how
 */

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioOptimization : MonoBehaviour {

    public AudioSource audioSource;
    public bool constantSound = false;

    private AudioListener _audioListener;
    private float _distanceFromPlayer;
    private double _soundTime;

    private void Start() {
        _audioListener = AudioListenerSingleton.instance.audioListener;
        Deactivate();
    }

    void Update() {

        _distanceFromPlayer = Vector3.Distance(transform.position, _audioListener.transform.position);

        ToggleAudioSource(_distanceFromPlayer <= audioSource.maxDistance);

        if (!constantSound && AudioSettings.dspTime >= _soundTime) {
            Deactivate();
        }
    }

    void ToggleAudioSource(bool isAudible) {

        if (!isAudible && audioSource.isPlaying) {
            audioSource.Pause();
        } else if (constantSound && isAudible && !audioSource.isPlaying) {
            audioSource.Play();
        }
    }

    public void Activate() {
        enabled = true;
        audioSource.enabled = true;

        _soundTime = AudioSettings.dspTime + (double)audioSource.clip.length;
    }

    public void Activate(Vector3 position) {
        enabled = true;
        audioSource.enabled = true;

        _soundTime = AudioSettings.dspTime + (double)audioSource.clip.length;

        transform.position = position;
    }

    public void Deactivate() {
        audioSource.enabled = false;
        enabled = false;

        transform.position = Vector3.zero;
    }
}