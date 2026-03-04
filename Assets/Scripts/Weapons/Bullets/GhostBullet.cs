using UnityEngine;

public class GhostBullet : MonoBehaviour {

    public ushort id;
    public float speed;
    public bool isEnemyBullet;
    public byte tier = 1;

    private void Update() {
        Movement();
    }

    public void Movement() {
        Vector3 newPosition = transform.position;
        newPosition += Time.deltaTime * speed * transform.forward;
        newPosition = new Vector3(newPosition.x, 0, newPosition.z);

        transform.position = newPosition;
    }
}
