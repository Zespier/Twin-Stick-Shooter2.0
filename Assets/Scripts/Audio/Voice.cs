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
public class Voice : MonoBehaviour {

    public VoicePool pool;
    public AudioSource audioSource;
    public VoicePriority voicePriority;
    public float startTime;
    public Vector3 position;
    public float timeOfFade;
    public float volume;

    private double _soundTime;

    public AudioListener _audioListener => AudioListenerSingleton.instance.audioListener;

    void Update() {
        position = transform.position;

        if (AudioSettings.dspTime >= _soundTime) {

            pool.StopVoice(this);
        }

        if (_audioListener != null) {
            //TODO: audioSource.maxDistance seems to be expensive, I want to manually set the maxDistance for every sound so this is just a check
            if ((position - _audioListener.transform.position).sqrMagnitude <= audioSource.maxDistance) {
                pool.StopVoice(this);
            }
        }
    }

    public void Activate() {
        audioSource.volume = volume;
        audioSource.Play();
        startTime = Time.time;
        _soundTime = AudioSettings.dspTime + (double)audioSource.clip.length;
    }

    public void Activate(Vector3 position) {
        Activate();
        transform.position = position;
    }

    public void Activate(Transform newParent) {
        transform.parent = newParent;
        Activate(newParent.position);
    }
}

public enum VoicePriority : byte {
    EnemyShoot = 0,
    BulletExplosion = 1,
    BulletShoot = 2,
    OwnerBulletShoots = 3,
    EnemyExploded = 4,
    PlayerExploded = 5,
}
