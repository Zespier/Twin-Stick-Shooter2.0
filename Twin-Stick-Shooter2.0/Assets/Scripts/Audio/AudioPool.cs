
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPool : MonoBehaviour {

    public List<AudioOptimization> sources;

    public int GetNumberOfActiveVoices() {
        int activeVoices = 0;
        for (int i = 0; i < sources.Count; i++) {
            if (sources[i] == null) {
                sources.RemoveAt(i);
                i--;
                continue;
            }
            if (sources[i].audioSource.isPlaying) {
                activeVoices++;
            }
        }

        return activeVoices;
    }

    /// <summary>
    /// Gets the nex available audio source
    /// </summary>
    /// <returns></returns>
    public AudioOptimization GetAvailableSource() {

        for (int i = 0; i < sources.Count; i++) {
            if (sources[i] == null) {
                sources.RemoveAt(i);
                i--;
                continue;
            }

            if (!sources[i].enabled) {

                return sources[i];
            }
        }

        sources.Add(Instantiate(sources[0], transform));
        sources[^1].Deactivate();
        return sources[^1];
    }
}
