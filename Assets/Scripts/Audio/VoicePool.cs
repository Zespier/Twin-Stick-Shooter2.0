
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoicePool : MonoBehaviour {

    public int maxVoicesClamped = 64;
    public int realMaxVoices = 96;
    public float timeToFadeAVoice = 0.1f;
    public List<Voice> _fadingVoices = new List<Voice>();
    public Voice prefab;
    //This is a very precise-manual ring buffer or something I don't remember the name, but it's a manual Queue to get the best performance
    public Voice[] voices;
    public int head;
    public int tail;
    public int count;

    private void Awake() {
        voices = new Voice[realMaxVoices];
        for (int i = 0; i < realMaxVoices; i++) {
            voices[i] = Instantiate(prefab, transform);
            voices[i].pool = this;
        }
    }

    private void Update() {
        for (int i = 0; i < _fadingVoices.Count; i++) {
            Voice voice = _fadingVoices[i];
            if (Time.time - voice.timeOfFade >= timeToFadeAVoice) {

                //This is the only possible case of messing up with the ring buffer. If by any case, the element 2 ends sooner than the first one, should't happen, but I don't know everything is a litte bit unpredictable these days
                voice.audioSource.Stop();
                _fadingVoices.RemoveAt(i);
                i--;
                head++;

                if (head == realMaxVoices) {
                    head = 0;
                }

                count--;


            } else {
                voice.audioSource.volume = Mathf.Lerp(1, 0, (Time.time - voice.timeOfFade) / timeToFadeAVoice);
            }
        }
    }

    public Voice PlayVoice(AudioClip clip, float volume, VoicePriority voicePriority, Vector3 position, AudioSource spatialBlendSettings) {
        if (count == maxVoicesClamped) {
            StealVoice();
        }

        if (count == realMaxVoices) {
            StealVoice(inmediate: true);
        }

        Voice voice = voices[tail];
        voice.audioSource.clip = clip;
        voice.volume = volume;
        Transfer3DSpatialBlendSettings(spatialBlendSettings, voice.audioSource);
        voice.Activate(position);

        tail++;
        if (tail == realMaxVoices) {
            tail = 0;
        }

        count++;

        return voice;
    }

    public void StealVoice(bool inmediate = false) {
        int bestIndex = -1;
        VoicePriority worstPriority = (VoicePriority)byte.MaxValue;
        float oldestTime = float.MaxValue;

        for (int i = 0; i < count; i++) {
            int index = (head + i) % maxVoicesClamped;
            Voice voice = voices[index];

            if (voice.voicePriority < worstPriority || (voice.voicePriority == worstPriority && voice.startTime < oldestTime)) {
                worstPriority = voice.voicePriority;
                oldestTime = voice.startTime;
                bestIndex = index;
            }
        }

        if (bestIndex != head) {
            Voice aux = voices[bestIndex];
            voices[bestIndex] = voices[head];
            voices[head] = aux;
        }

        StopVoice(canStopInmediate: inmediate);
    }

    public void StopVoice(bool canStopInmediate = false) {

        if (canStopInmediate) {

            voices[head].audioSource.Stop();
            for (int i = 0; i < _fadingVoices.Count; i++) {
                if (_fadingVoices[i] == voices[head]) {
                    _fadingVoices.RemoveAt(i);
                    break;
                }
            }

            head++;

            if (head == realMaxVoices) {
                head = 0;
            }

            count--;

        } else {
            voices[head].timeOfFade = Time.time;
            _fadingVoices.Add(voices[head]);
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
