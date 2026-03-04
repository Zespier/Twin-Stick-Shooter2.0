using UnityEngine;
using UnityEngine.InputSystem;

public class Initiall : Ship {

    public byte slots;
    public float shipSpeedPercentage = 1f;

    //no habilities
    public override void StartSpecialHability(InputAction.CallbackContext context) {
    }

    public override void EndSpecialHability(InputAction.CallbackContext context) {
    }
}
