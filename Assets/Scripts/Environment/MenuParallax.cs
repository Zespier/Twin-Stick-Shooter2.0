using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuParallax : MonoBehaviour {

    public float speed = 1f;

    /// <summary>
    /// moves th eparallax only on the menu
    /// </summary>
    private void Update() {
        transform.position += Time.deltaTime * speed * new Vector3(-1, 0, -1);
    }

}
