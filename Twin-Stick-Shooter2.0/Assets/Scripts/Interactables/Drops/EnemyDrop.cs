using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrop : DropBase, IDrop, IPickable, IInteractable
{
    public Rigidbody rb;

    #region Inheritances

    public void Drop()
    {
    }

    public void Land()
    {
        DeactivatePhysics(rb, true);
    }

    public void PickUp()
    {
    }

    public void Interact()
    {
    }

    #endregion
}
