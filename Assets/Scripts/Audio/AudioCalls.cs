using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCalls {

    public AudioCalls(AudioManager manager, AudioPool audioPool) {
        this.manager = manager;
        this.audioPool = audioPool;
    }

    public AudioManager manager;
    public AudioPool audioPool;

    public bool TooMuchVoices(AudioCategory audioCategory) {
        return manager.CurrentActiveVoices >= manager.maxRealVoices - manager.VoiceSpaceMargin(audioCategory);
    }

    /// <summary>
    /// Plays a clip on an available audioSource inside the pool
    /// </summary>
    /// <param name="position"></param>
    public AudioOptimization PlaySound(AudioCategory audioCategory, AudioClip clip, Transform newParent = default, Vector3 position = default, float pitch = 1, float volume = 1, AudioSource spatialBlendSettings = default) {

        if (TooMuchVoices(audioCategory)) { return null; }

        AudioOptimization audioOptimization = audioPool.GetAvailableSource();
        audioOptimization.audioCategory = audioCategory;
        audioOptimization.audioSource.clip = clip;
        audioOptimization.audioSource.pitch = pitch;
        audioOptimization.audioSource.volume = volume;
        Transfer3DSpatialBlendSettings(spatialBlendSettings == default ? manager.defaultSettings : spatialBlendSettings, audioOptimization.audioSource);
        if (newParent != default) { audioOptimization.transform.SetParent(newParent); }
        audioOptimization.Activate(position);

        return audioOptimization;
    }

    public void Transfer3DSpatialBlendSettings(AudioSource from, AudioSource to) {
        to.panStereo = from.panStereo;
        to.spatialBlend = from.spatialBlend;
        to.reverbZoneMix = from.reverbZoneMix;
        to.dopplerLevel = from.dopplerLevel;
        to.spread = from.spread;
        to.minDistance = from.minDistance;
        to.maxDistance = from.maxDistance;
        to.rolloffMode = from.rolloffMode;
        to.SetCustomCurve(AudioSourceCurveType.CustomRolloff, from.GetCustomCurve(AudioSourceCurveType.CustomRolloff));
        to.SetCustomCurve(AudioSourceCurveType.SpatialBlend, from.GetCustomCurve(AudioSourceCurveType.SpatialBlend));
        to.SetCustomCurve(AudioSourceCurveType.Spread, from.GetCustomCurve(AudioSourceCurveType.Spread));
        to.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, from.GetCustomCurve(AudioSourceCurveType.ReverbZoneMix));
    }
}
