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

    /// <summary>
    /// Gets the next usable parallax layer
    /// </summary>
    /// <param name="layerIndex"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Writes the initial layers
    /// </summary>
    private void SetInitialLayerIndexs() {
        for (int i = 0; i < layers.Count; i++) {
            layers[i].layer = i;
        }
    }

    /// <summary>
    /// Sets the initial bounds
    /// </summary>
    public void SetInitialLayersBounds() {
        for (int i = 0; i < layers.Count; i++) {
            layers[i].SetBounds(Vector3.zero, GetLayerDistance(layers[i].layer), sizePerDistance);
            layers[i].Coordinate = Vector2Int.zero;
            layers[i].Activate();
        }
    }

    /// <summary>
    /// Gets the distance from the camera of the parallax layer
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public float GetLayerDistance(int index) {
        return -(initialLayerDistance + distanceBetweenLayers * index);
    }

}
