using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public AudioMixer audioMixer;
    public VoicePool audioPool;
    public AudioSource defaultSettings;
    public AudioSource bulletImpactSettings;

    public float shootsVolume = 0.3f;
    public List<AudioClip> shootClips;

    public float pitchRange = 0.1f;
    public AudioSource shipSoundSource;
    public float shipSoundMaxPitch = 3f;
    public float shipSoundMinPitch = 1f;

    public float enemyLasersVolume = 0.3f;
    public List<AudioClip> enemyLasersClips;

    public float explosionVolume = 0.3f;
    public float enemyExplosionVolume = 0.6f;
    public AudioClip explosionClip;
    public AudioClip playerExplosionClip;

    private bool _canEnemyLaserSoundAgain;
    private bool _canEnemyExplosionSoundAgain;
    private bool _canPlayerExplosionSoundAgain;
    private bool _canShootSoundAgain;
    private bool _canBulletExplosionAgainstTheWallSoundAgain;

    public static AudioManager instance;
    private void Awake() {
        if (!instance) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }

    private void Update() {
        _canEnemyLaserSoundAgain = true;
        _canEnemyExplosionSoundAgain = true;
        _canPlayerExplosionSoundAgain = true;
        _canShootSoundAgain = true;
        _canBulletExplosionAgainstTheWallSoundAgain = true;
    }

    public void ShipSound(Vector3 speed) {
        float max = Mathf.Abs(speed.x) > Mathf.Abs(speed.y) ? Mathf.Abs(speed.x) : Mathf.Abs(speed.y);
        float targetValue = Mathf.Lerp(shipSoundMinPitch, shipSoundMaxPitch, (PlayerController.instance.Stats.Speed * max) / PlayerController.instance.Stats.Speed);

        float pitchLerpSpeed = 0.1f;
        if (shipSoundSource.pitch > targetValue) {
            pitchLerpSpeed = 0.2f;
        }

        shipSoundSource.pitch = Mathf.Lerp(shipSoundSource.pitch, targetValue, Time.deltaTime / pitchLerpSpeed);
    }

    public void EnemyLaserSound(Vector3 position) {

        if (_canEnemyLaserSoundAgain) {
            _canEnemyLaserSoundAgain = false;

            Voice _audioSource = audioPool.PlayVoice(enemyLasersClips[Random.Range(0, enemyLasersClips.Count)], enemyLasersVolume, VoicePriority.EnemyShoot, position, defaultSettings);
            _audioSource.audioSource.pitch = Random.Range(1.1f - pitchRange, 1.1f + pitchRange);
        }
    }

    public void ExplosionSound(Vector3 position, string whoGotExploded) {
        AudioClip audioClip = null;
        float volume = 0;
        VoicePriority voicePriority = VoicePriority.EnemyExploded;
        switch (whoGotExploded) {
            case "enemy":
            case "Enemy":
                audioClip = explosionClip;
                volume = enemyExplosionVolume;
                break;

            case "player":
            case "Player":
                audioClip = playerExplosionClip;
                volume = explosionVolume;
                voicePriority = VoicePriority.PlayerExploded;
                break;

            default:
                break;
        }

        if (voicePriority == VoicePriority.EnemyExploded && _canEnemyExplosionSoundAgain) {
            _canEnemyExplosionSoundAgain = false;

            Voice _audioSource = audioPool.PlayVoice(audioClip, volume, voicePriority, position, defaultSettings);
            _audioSource.audioSource.pitch = Random.Range(1.1f - pitchRange, 1.1f + pitchRange);


        } else if (voicePriority == VoicePriority.PlayerExploded && _canPlayerExplosionSoundAgain) {
            _canPlayerExplosionSoundAgain = false;

            Voice _audioSource = audioPool.PlayVoice(audioClip, volume, voicePriority, position, defaultSettings);
            _audioSource.audioSource.pitch = Random.Range(1.1f - pitchRange, 1.1f + pitchRange);
        }
    }

    public void ShootSound() {

        if (_canShootSoundAgain) {
            _canShootSoundAgain = false;

            Voice _audioSource = audioPool.PlayVoice(shootClips[Random.Range(0, shootClips.Count)], shootsVolume, VoicePriority.BulletShoot, PlayerController.instance.transform.position, defaultSettings);
            _audioSource.audioSource.pitch = Random.Range(1 - pitchRange, 1 + pitchRange);
        }
    }

    public void PlayBulletExplosionAgainstTheWall(Vector3 position) {


        if (_canBulletExplosionAgainstTheWallSoundAgain) {
            _canBulletExplosionAgainstTheWallSoundAgain = false;

            Voice _audioSource = audioPool.PlayVoice(explosionClip, explosionVolume / 2f, VoicePriority.BulletExplosion, position, bulletImpactSettings);
            _audioSource.audioSource.pitch = Random.Range(1.1f - pitchRange, 1.1f + pitchRange);
        }
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