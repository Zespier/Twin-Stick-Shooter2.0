using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioOptimization : MonoBehaviour {

    public AudioSource audioSource;

    private AudioListener _audioListener;
    private float _distanceFromPlayer;

    private void Start() {
        _audioListener = AudioListenerSingleton.instance.audioListener;
    }

    void Update() {

        _distanceFromPlayer = Vector3.Distance(transform.position, _audioListener.transform.position);

        ToggleAudioSource(_distanceFromPlayer <= audioSource.maxDistance);
    }

    void ToggleAudioSource(bool isAudible) {

        if (!isAudible && audioSource.isPlaying) {
            audioSource.Pause();
        } else if (isAudible && !audioSource.isPlaying) {
            audioSource.Play();
        }
    }
}