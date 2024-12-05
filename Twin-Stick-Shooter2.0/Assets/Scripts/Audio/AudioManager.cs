using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour {

    public AudioMixer audioMixer;
    public AudioPool audioPool;
    public int maxRealVoices = 52;
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

    private AudioCalls audioCalls;

    public int CurrentActiveVoices => GetNumberOfActiveVoices();

    public static AudioManager instance;
    private void Awake() {
        if (!instance) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        audioCalls = new AudioCalls(this, audioPool);

        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
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

    public void EnemyLaserSound(Vector3 position) {
        AudioOptimization _audioSource = audioPool.GetAvailableSource();
        _audioSource.audioSource.clip = enemyLasersClips[UnityEngine.Random.Range(0, enemyLasersClips.Count)];
        _audioSource.audioSource.pitch = UnityEngine.Random.Range(1.1f - pitchRange, 1.1f + pitchRange);
        _audioSource.audioSource.volume = enemyLasersVolume;
        _audioSource.Activate(position);
        _audioSource.audioSource.Play();
    }

    public void ExplosionSound(Vector3 position, string whoGotExploded) {
        AudioClip audioClip = null;
        float volume = 0;
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
                break;

            default:
                break;
        }
        audioCalls.PlaySound(AudioCategory.GenericPoolSoundMaxPriority, audioClip, position: position, volume: volume, spatialBlendSettings: defaultSettings);
    }

    public void ShootSound() {
        audioCalls.PlaySound(AudioCategory.GenericPoolSoundLowPriority, shootClips[UnityEngine.Random.Range(0, shootClips.Count)], pitch: UnityEngine.Random.Range(1 - pitchRange, 1 + pitchRange), position: PlayerController.instance.transform.position, spatialBlendSettings: defaultSettings, volume: shootsVolume);
    }

    public void PlayShortSound(ShortSound type, Vector3 position) {
        switch (type) {
            case ShortSound.smallExplosion:
                audioCalls.PlaySound(AudioCategory.GenericPoolSoundLowPriority, explosionClip, position: position, spatialBlendSettings: bulletImpactSettings, volume: explosionVolume / 2f);
                break;

            default:
                break;
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

    #region Maybe Useful later
    ///// <summary>
    ///// Lerps the pitch of the sound of the ship depending on the speed
    ///// </summary>
    ///// <param name="speed"></param>
    //public void ShipSound(Vector3 speed) {
    //    float max = Mathf.Abs(speed.x) > Mathf.Abs(speed.y) ? Mathf.Abs(speed.x) : Mathf.Abs(speed.y);
    //    float targetValue = Mathf.Lerp(shipSoundMinPitch, shipSoundMaxPitch, (PlayerController.instance.speed * max) / PlayerController.instance.speed);

    //    float pitchLerpSpeed = 0.1f;
    //    if (shipSoundSource.pitch > targetValue) {
    //        pitchLerpSpeed = 0.2f;
    //    }

    //    shipSoundSource.pitch = Mathf.Lerp(shipSoundSource.pitch, targetValue, Time.deltaTime / pitchLerpSpeed);
    //}

    //private IEnumerator DebugTime(AudioClip clip) {
    //    DateTime before = DateTime.Now;
    //    TimeSpan beforeTime = before.TimeOfDay;

    //    while (clip.loadState != AudioDataLoadState.Loaded) {
    //        yield return null;
    //    }

    //    DateTime after = DateTime.Now;
    //    TimeSpan afterTime = after.TimeOfDay;

    //    TimeSpan duration = afterTime - beforeTime;


    //    Debug.Log($"The audio took => {duration.TotalSeconds} to load");
    //}


    //private void OnEnable() {
    //    Events.OnNextCharacter += PlayDialogueSound;
    //}

    //private void OnDisable() {
    //    Events.OnNextCharacter -= PlayDialogueSound;
    //}

    /// <summary>
    /// Sound for the dialogue
    /// Gets the next available audio source on the pool
    /// </summary>
    /// <param name="position"></param>
    //public void PlayDialogueSound(Voice voice) {
    //    switch (voice) {
    //        case Voice.player:
    //            PlayDialogueSound_Normal(player.position);
    //            break;
    //        case Voice.director:
    //            PlayDialogueSound_Deep(player.position);
    //            break;
    //        default:
    //            PlayDialogueSound_Normal(player.position);
    //            Debug.LogError("Non identified voice");
    //            break;
    //    }
    //}
    //public void PlayDialogueSound_Normal(Vector3 position) {
    //    audioCalls.PlaySound(normalVoice, position, volume: generalVolume);
    //}
    //public void PlayDialogueSound_Deep(Vector3 position) {
    //    audioCalls.PlaySound(deepVoice, position, volume: generalVolume);
    //}

    //public void PlayStepSound() {
    //    if (playableSteps == null || playableSteps.Count <= 0) {
    //        playableSteps = new List<int>() { 0, 1, 2, 3, 4 };
    //    }

    //    int randomIndex = UnityEngine.Random.Range(0, playableSteps.Count);
    //    audioCalls.PlaySound(stepSounds[playableSteps[randomIndex]], pitch: 0.75f + UnityEngine.Random.Range(-pitchRange, pitchRange), volume: stepsVolume);
    //    playableSteps.RemoveAt(randomIndex);
    //}

    //public void PlayLockTickSound() {
    //    audioCalls.PlaySound(lockTickSound, 1 + UnityEngine.Random.Range(-pitchRange, pitchRange), volume: generalVolume);
    //}

    #endregion

    public int VoiceSpaceMargin(AudioCategory audioCategory) {
        switch (audioCategory) {
            default:
            case AudioCategory.GenericPoolSoundMaxPriority:
                return 0;
            case AudioCategory.ConstantAmbientSound:
            case AudioCategory.ConstantBrazierSound:
                return 15;
            case AudioCategory.FrequentReactivation:
                return 0;
            case AudioCategory._0DelayNeeded:
                return 0;
            case AudioCategory.GenericPoolSoundLowPriority:
                return 10;
        }
    }

    private int GetNumberOfActiveVoices() {
        int activeVoices = 0;

        activeVoices += audioPool.GetNumberOfActiveVoices();

        return activeVoices;
    }
}

public enum ShortSound {
    smallExplosion = 0,
}