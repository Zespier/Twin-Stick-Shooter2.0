using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public AudioMixer audioMixer;
    public AudioPool audioPool;
    public float shootsVolume = 0.3f;
    public List<AudioClip> shootClips;
    public float bulletImpactVolume = 0.3f;
    public List<AudioClip> bulletImpacClips;
    public float pitchRange = 0.1f;
    public AudioSource shipSoundSource;
    public float shipSoundMaxPitch = 3f;
    public float shipSoundMinPitch = 1f;

    private AudioOptimization _audioSource;

    public static AudioManager instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void OnEnable() {
        Events.OnBulletImpact += BulletImpactSound;
        Events.OnShootBullet += ShootSound;
    }

    private void Start() {
        //ambientSource.Play();
    }

    private void OnDisable() {
        Events.OnBulletImpact -= BulletImpactSound;
        Events.OnShootBullet -= ShootSound;
    }

    public void ShipSound(Vector3 speed) {
        float max = Mathf.Abs(speed.x) > Mathf.Abs(speed.y) ? Mathf.Abs(speed.x) : Mathf.Abs(speed.y);
        float targetValue = Mathf.Lerp(shipSoundMinPitch, shipSoundMaxPitch, (PlayerController.instance.speed * max) / PlayerController.instance.speed);

        float pitchLerpSpeed = 0.1f;
        if (shipSoundSource.pitch > targetValue) {
            pitchLerpSpeed = 0.2f;
        }

        shipSoundSource.pitch = Mathf.Lerp(shipSoundSource.pitch, targetValue, Time.deltaTime / pitchLerpSpeed);
    }

    public void BulletImpactSound(Vector3 position) {
        _audioSource = audioPool.GetAvailableSource();
        _audioSource.audioSource.clip = bulletImpacClips[Random.Range(0, bulletImpacClips.Count)];
        _audioSource.audioSource.pitch = Random.Range(1 - pitchRange, 1 + pitchRange);
        _audioSource.audioSource.volume = bulletImpactVolume;
        _audioSource.Activate(position);
        _audioSource.audioSource.Play();
    }

    public void ShootSound() {
        _audioSource = audioPool.GetAvailableSource();
        _audioSource.audioSource.clip = shootClips[Random.Range(0, shootClips.Count)];
        _audioSource.audioSource.pitch = Random.Range(1 - pitchRange, 1 + pitchRange);
        _audioSource.audioSource.volume = shootsVolume;
        _audioSource.Activate();
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
