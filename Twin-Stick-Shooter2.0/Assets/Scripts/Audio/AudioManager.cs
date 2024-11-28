using System;
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

    public float enemyLasersVolume = 0.3f;
    public List<AudioClip> enemyLasersClips;

    public float explosionVolume = 0.3f;
    public float enemyExplosionVolume = 0.6f;
    public AudioClip explosionClip;
    public AudioClip playerExplosionClip;

    public float pitchRange = 0.1f;
    public AudioSource shipSoundSource;
    public float shipSoundMaxPitch = 3f;
    public float shipSoundMinPitch = 1f;

    private AudioOptimization _audioSource;

    public static AudioManager instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    /// <summary>
    /// Lerps the pitch of the sound of the ship depending on the speed
    /// </summary>
    /// <param name="speed"></param>
    public void ShipSound(Vector3 speed) {
        float max = Mathf.Abs(speed.x) > Mathf.Abs(speed.y) ? Mathf.Abs(speed.x) : Mathf.Abs(speed.y);
        float targetValue = Mathf.Lerp(shipSoundMinPitch, shipSoundMaxPitch, (PlayerController.instance.speed * max) / PlayerController.instance.speed);

        float pitchLerpSpeed = 0.1f;
        if (shipSoundSource.pitch > targetValue) {
            pitchLerpSpeed = 0.2f;
        }

        shipSoundSource.pitch = Mathf.Lerp(shipSoundSource.pitch, targetValue, Time.deltaTime / pitchLerpSpeed);
    }

    /// <summary>
    /// Sound for bullets impact
    /// Gets the next available audio source on the pool
    /// </summary>
    /// <param name="position"></param>
    public void BulletImpactSound(Vector3 position) {
        _audioSource = audioPool.GetAvailableSource();
        _audioSource.audioSource.clip = bulletImpacClips[UnityEngine.Random.Range(0, bulletImpacClips.Count)];
        _audioSource.audioSource.pitch = UnityEngine.Random.Range(1 - pitchRange, 1 + pitchRange);
        _audioSource.audioSource.volume = bulletImpactVolume;
        _audioSource.Activate(position);
        _audioSource.audioSource.Play();

        StartCoroutine(DebugTime(_audioSource.audioSource.clip));
    }

    private IEnumerator DebugTime(AudioClip clip) {
        DateTime before = DateTime.Now;
        TimeSpan beforeTime = before.TimeOfDay;

        while (clip.loadState != AudioDataLoadState.Loaded) {
            yield return null;
        }

        DateTime after = DateTime.Now;
        TimeSpan afterTime = after.TimeOfDay;

        TimeSpan duration = afterTime - beforeTime;


        Debug.Log($"The audio took => {duration.TotalSeconds} to load");
    }

    /// <summary>
    /// Sound for playerShooting
    /// Gets the next available audio source on the pool
    /// </summary>
    /// <param name="position"></param>
    public void ShootSound() {
        _audioSource = audioPool.GetAvailableSource();
        _audioSource.audioSource.clip = shootClips[UnityEngine.Random.Range(0, shootClips.Count)];
        _audioSource.audioSource.pitch = UnityEngine.Random.Range(1 - pitchRange, 1 + pitchRange);
        _audioSource.audioSource.volume = shootsVolume;
        _audioSource.Activate();
        _audioSource.audioSource.Play();
    }

    /// <summary>
    /// Sound for enemies lasers
    /// Gets the next available audio source on the pool
    /// </summary>
    /// <param name="position"></param>
    public void EnemyLaserSound(Vector3 position) {
        _audioSource = audioPool.GetAvailableSource();
        _audioSource.audioSource.clip = enemyLasersClips[UnityEngine.Random.Range(0, enemyLasersClips.Count)];
        _audioSource.audioSource.pitch = UnityEngine.Random.Range(1.1f - pitchRange, 1.1f + pitchRange);
        _audioSource.audioSource.volume = enemyLasersVolume;
        _audioSource.Activate(position);
        _audioSource.audioSource.Play();
    }

    /// <summary>
    /// Sound for explosions, depending on player or enemy
    /// Gets the next available audio source on the pool
    /// </summary>
    /// <param name="position"></param>
    public void ExplosionSound(Vector3 position, string whoGotExploded) {
        _audioSource = audioPool.GetAvailableSource();
        switch (whoGotExploded) {
            case "enemy":
            case "Enemy":
                _audioSource.audioSource.clip = explosionClip;
                _audioSource.audioSource.volume = enemyExplosionVolume;
                break;

            case "player":
            case "Player":
                _audioSource.audioSource.clip = playerExplosionClip;
                _audioSource.audioSource.volume = explosionVolume;
                break;

            default:
                break;
        }
        _audioSource.Activate(position);
        _audioSource.audioSource.Play();
    }

    #region Volume management

    /// <summary>
    /// Changes the volume of master
    /// </summary>
    /// <param name="volume"></param>
    public void VolumeMaster(float volume) {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    /// <summary>
    /// Changes the volume of sounds
    /// </summary>
    /// <param name="volume"></param>
    public void VolumeSounds(float volume) {
        audioMixer.SetFloat("Sounds", Mathf.Log10(volume) * 20);
    }

    /// <summary>
    /// Changes the volume of music
    /// </summary>
    /// <param name="volume"></param>
    public void VolumeMusic(float volume) {
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    #endregion
}
