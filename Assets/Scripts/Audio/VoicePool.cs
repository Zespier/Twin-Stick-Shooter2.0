
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoicePool : MonoBehaviour {

    public int maxVoices = 64;
    public float timeToFadeAVoice = 0.1f;
    public List<Voice> _fadingVoices = new List<Voice>();
    public Voice prefab;
    //This is a very precise-manual ring buffer or something I don't remember the name, but it's a manual Queue to get the best performance
    public Voice[] voices;
    public int head;
    public int tail;
    public int count;
    public AudioSource defaultSettings;

    private void Awake() {
        voices = new Voice[maxVoices];
    }

    private void Update() {
        for (int i = 0; i < _fadingVoices.Count; i++) {
            Voice voice = _fadingVoices[i];
            if (Time.time - voice.timeOfFade >= timeToFadeAVoice) {


                voice.audioSource.Stop();
                _fadingVoices.RemoveAt(i);
                i--;
                head++;

                if (head == maxVoices) {
                    head = 0;
                }

                count--;


            } else {
                voice.audioSource.volume = Mathf.Lerp(1, 0, (Time.time - voice.timeOfFade) / timeToFadeAVoice);
            }
        }
    }

    public Voice PlayVoice(AudioClip clip, float volume, Vector3 position, AudioSource spatialBlendSettings) {
        if (count == maxVoices) {
            StealVoice();
        }

        Voice voice = voices[tail];
        voice.audioSource.clip = clip;
        voice.volume = volume;
        Transfer3DSpatialBlendSettings(spatialBlendSettings == default ? defaultSettings : spatialBlendSettings, voice.audioSource);
        voice.Activate(position);

        tail++;
        if (tail == maxVoices) {
            tail = 0;
        }

        count++;

        return voice;
    }

    public void StealVoice() {
        int bestIndex = -1;
        VoicePriority worstPriority = (VoicePriority)byte.MaxValue;
        float oldestTime = float.MaxValue;

        for (int i = 0; i < count; i++) {
            int index = (head + i) % maxVoices;
            Voice voice = voices[index];

            if (voice.voicePriority < worstPriority || (voice.voicePriority == worstPriority && voice.startTime < oldestTime)) {
                worstPriority = voice.voicePriority;
                oldestTime = voice.startTime;
                bestIndex = index;
            }
        }

        Voice shitVoice = voices[bestIndex];
        StopVoice(shitVoice);
    }

    public void StopVoice(Voice voice, bool canStopInmediate = false) {

        if (canStopInmediate) {

            voice.audioSource.Stop();
            for (int i = 0; i < _fadingVoices.Count; i++) {
                if (_fadingVoices[i] == voice) {
                    _fadingVoices.RemoveAt(i);
                    break;
                }
            }

            head++;

            if (head == maxVoices) {
                head = 0;
            }

            count--;

        } else {
            voice.timeOfFade = Time.time;
            _fadingVoices.Add(voice);
        }
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
