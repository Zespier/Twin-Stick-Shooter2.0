using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour {

    public Bounds bounds = new Bounds();

    private Camera _mainCamera;

    public Bounds _cameraBounds;


    private void Awake() {
        _mainCamera = Camera.main;
        _cameraBounds = new Bounds();
    }

    public float GetWidthFromCenter() {
        //Angle of view(in degrees) = 2 ArcTan(sensor width / (2 x focal length))) *(180 / π)

        float distance = -transform.position.y;
        float angle = 2 * Mathf.Atan(_mainCamera.sensorSize.x / (2 * _mainCamera.focalLength)) * (180 / Mathf.PI);
        angle /= 2f;
        float sin = Mathf.Abs(Mathf.Sin(angle * Mathf.Deg2Rad));
        float hipotenusa = distance / sin;

        float widthFromCenter = (Mathf.Sin(angle) * hipotenusa);

        return widthFromCenter;
    }

    public float GetHeightFromCenter() {

        float distance = -transform.position.y;
        float angle = 2 * Mathf.Atan(_mainCamera.sensorSize.y / (2 * _mainCamera.focalLength)) * (180 / Mathf.PI);
        angle /= 2f;

        float hipotenusa = distance / Mathf.Sin(angle);

        float heightFromCenter = (Mathf.Sin(angle) * hipotenusa);

        return heightFromCenter;
    }

    public void SetBounds(Vector3 position, float distance, float sizePerDistance) {

        position.y = distance;

        transform.position = position;
        transform.localScale = Vector3.one * (1 - distance * sizePerDistance);

        float widthFromCenter = GetWidthFromCenter();
        float heightFromCenter = GetHeightFromCenter();

        bounds.center = position;
        bounds.size = 40 * transform.localScale.x * Vector3.one;

        _cameraBounds.min = new Vector3(_mainCamera.transform.position.x - widthFromCenter, distance, _mainCamera.transform.position.z - heightFromCenter);
        _cameraBounds.max = new Vector3(_mainCamera.transform.position.x + widthFromCenter, distance + 1, _mainCamera.transform.position.z + heightFromCenter);

        if (bounds.min.x > _cameraBounds.min.x) {
            Debug.Log("Nos salimos");
        } else {
            Debug.Log(" ");
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_cameraBounds.center, _cameraBounds.size);
    }

}
