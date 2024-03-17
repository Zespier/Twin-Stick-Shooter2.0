
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPool : MonoBehaviour {

    public List<AudioOptimization> sources;

    public AudioOptimization GetAvailableSource() {

        for (int i = 0; i < sources.Count; i++) {
            if (!sources[i].enabled) {

                sources[i].Activate();
                return sources[i];
            }
        }

        sources.Add(Instantiate(sources[0], transform));
        sources[^1].Deactivate();
        return sources[^1];
    }

}
