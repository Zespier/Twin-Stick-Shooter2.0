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