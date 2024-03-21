using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour {

    public int layer = 0;
    public SpriteRenderer spriteRenderer;
    public Bounds bounds = new Bounds();
    public bool _canBeUsed = true;

    private Camera _mainCamera;
    private float _sizePerDistance;
    private Vector3 up;
    private Vector3 down;
    private Vector3 left;
    private Vector3 right;

    public Bounds _cameraBounds;
    public Vector2Int Coordinate { get; set; }


    private void Awake() {
        _mainCamera = Camera.main;
        _cameraBounds = new Bounds();
    }

    private void Update() {
        SetCameraBounds(GetWidthFromCenter(), GetHeightFromCenter());
        CheckDeactivate();
        CheckBorders();
    }

    public float GetWidthFromCenter() {
        //Angle of view(in degrees) = 2 ArcTan(sensor width / (2 x focal length))) *(180 / π)

        float distance = -transform.position.y;
        float angle = 2 * Mathf.Atan(_mainCamera.sensorSize.x / (2 * _mainCamera.focalLength)) * (180 / Mathf.PI);
        angle /= 2f;
        float cos = Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad));
        float hipotenusa = distance / cos;

        float widthFromCenter = (Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad)) * hipotenusa);

        return widthFromCenter;
    }

    public float GetHeightFromCenter() {


        float distance = -transform.position.y;
        float angle = 2 * Mathf.Atan(_mainCamera.sensorSize.y / (2 * _mainCamera.focalLength)) * (180 / Mathf.PI);
        angle /= 2f;
        float cos = Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad));
        float hipotenusa = distance / cos;

        float heightFromCenter = (Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad)) * hipotenusa);

        return heightFromCenter;
    }

    public void SetBounds(Vector3 position, float distance, float sizePerDistance) {

        _sizePerDistance = sizePerDistance;

        position.y = distance;

        transform.position = position;
        transform.localScale = Vector3.one * -(distance * sizePerDistance);
        bounds = spriteRenderer.bounds;

        up = bounds.center + Vector3.forward * bounds.extents.z;
        down = bounds.center - Vector3.forward * bounds.extents.z;
        left = bounds.center + Vector3.left * bounds.extents.x;
        right = bounds.center + Vector3.right * bounds.extents.x;
    }

    private void CheckDeactivate() {
        if (!bounds.Intersects(_cameraBounds)) {
            Deactivate();
        }
    }

    private void CheckBorders() {

        if (_cameraBounds.Contains(up)) {
            if (ParallaxManager.instance.layers.Exists(l => !l._canBeUsed && l.Coordinate == (Coordinate + Vector2Int.up))) { return; }
            var newLayer = ParallaxManager.instance.GetUsableLayer(layer);
            newLayer.Activate();
            newLayer.SetBounds(bounds.center + Vector3.forward * bounds.size.z, transform.position.y, _sizePerDistance);
            newLayer.Coordinate += Vector2Int.up;

        } else if (_cameraBounds.Contains(down)) {
            if (ParallaxManager.instance.layers.Exists(l => !l._canBeUsed && l.Coordinate == (Coordinate + Vector2Int.down))) { return; }
            var newLayer = ParallaxManager.instance.GetUsableLayer(layer);
            newLayer.Activate();
            newLayer.SetBounds(bounds.center - Vector3.forward * bounds.size.z, transform.position.y, _sizePerDistance);
            newLayer.Coordinate += Vector2Int.down;

        } else if (_cameraBounds.Contains(left)) {
            if (ParallaxManager.instance.layers.Exists(l => !l._canBeUsed && l.Coordinate == (Coordinate + Vector2Int.left))) { return; }
            var newLayer = ParallaxManager.instance.GetUsableLayer(layer);
            newLayer.Activate();
            newLayer.SetBounds(bounds.center + Vector3.left * bounds.size.z, transform.position.y, _sizePerDistance);
            newLayer.Coordinate += Vector2Int.left;

        } else if (_cameraBounds.Contains(right)) {
            if (ParallaxManager.instance.layers.Exists(l => !l._canBeUsed && l.Coordinate == (Coordinate + Vector2Int.right))) { return; }
            var newLayer = ParallaxManager.instance.GetUsableLayer(layer);
            newLayer.Activate();
            newLayer.SetBounds(bounds.center + Vector3.right * bounds.size.z, transform.position.y, _sizePerDistance);
            newLayer.Coordinate += Vector2Int.right;

        }
    }

    private void SetCameraBounds(float widthFromCenter, float heightFromCenter) {
        _cameraBounds.center = new Vector3(_mainCamera.transform.position.x, transform.position.y, _mainCamera.transform.position.z);
        _cameraBounds.size = new Vector3(widthFromCenter * 2, 1, heightFromCenter * 2f);
    }

    public void Activate() {
        _canBeUsed = false;
        gameObject.SetActive(true);
    }
    private void Deactivate() {
        _canBeUsed = true;
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_cameraBounds.center, _cameraBounds.size);
    }

}
