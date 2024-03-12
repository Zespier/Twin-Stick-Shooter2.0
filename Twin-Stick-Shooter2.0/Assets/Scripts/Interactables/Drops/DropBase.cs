using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DropBase : MonoBehaviour
{

    public virtual void DeactivatePhysics(Rigidbody rb, bool deactivate)
    {
        rb.isKinematic = deactivate;
        rb.detectCollisions = !deactivate;
    }

    public virtual Vector3 RandomDrop()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(1.5f, 2f), Random.Range(-1f, 1f));
    }

}
