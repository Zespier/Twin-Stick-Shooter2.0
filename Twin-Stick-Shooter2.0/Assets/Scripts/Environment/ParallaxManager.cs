using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour {

    public float initialLayerDistance = 15f;
    public float distanceBetweenLayers = 50f;
    public float sizePerDistance = 0.1f;
    public List<ParallaxLayer> layers = new List<ParallaxLayer>();

    public ParallaxLayer _newLayer { get; set; }

    public static ParallaxManager instance;
    private void Awake() {
        if (!instance) { instance = this; }

        SetInitialLayerIndexs();
        SetInitialLayersBounds();
    }

    public ParallaxLayer GetUsableLayer(int layerIndex) {
        for (int i = 0; i < layers.Count; i++) {
            if (layers[i].layer == layerIndex && !layers[i].gameObject.activeSelf) {
                return layers[i];
            }
        }

        _newLayer = Instantiate(layers.Find(l => l.layer == layerIndex), transform);
        layers.Add(_newLayer);
        return _newLayer;
    }

    private void SetInitialLayerIndexs() {
        for (int i = 0; i < layers.Count; i++) {
            layers[i].layer = i;
        }
    }

    public void SetInitialLayersBounds() {
        for (int i = 0; i < layers.Count; i++) {
            layers[i].SetBounds(Vector3.zero, GetLayerDistance(layers[i].layer), sizePerDistance);
            layers[i].Coordinate = Vector2Int.zero;
            layers[i].Activate();
        }
    }

    public float GetLayerDistance(int index) {
        return -(initialLayerDistance + distanceBetweenLayers * index);
    }

}
