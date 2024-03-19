using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour {

    public float initialLayerDistance = 15f;
    public float distanceBetweenLayers = 50f;
    public float sizePerDistance = 0.1f;
    public List<ParallaxLayer> layers = new List<ParallaxLayer>();

    private void Awake() {
        SetLayersBounds();
    }

    private void Update() {
        SetLayersBounds();
    }

    public void SetLayersBounds() {
        for (int i = 0; i < layers.Count; i++) {
            layers[i].SetBounds(Vector3.zero, GetLayerDistance(i), sizePerDistance);
        }
    }

    public float GetLayerDistance(int index) {
        return -(initialLayerDistance + distanceBetweenLayers * index);
    }

}
