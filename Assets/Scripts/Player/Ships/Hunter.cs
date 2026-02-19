using UnityEngine;
using UnityEngine.InputSystem;

public class Hunter : Ship {

    public bool _sprinting;

    public override void StartSpecialHability(InputAction.CallbackContext context) {
        //TODO: FUCKING RUN MORE AND INMUNE AND NO ATTACK

        _sprinting = true;

        Stats.HunterSpeed = 1.2f;
    }

    public override void EndSpecialHability(InputAction.CallbackContext context) {
        _sprinting = false;

        Stats.HunterSpeed = 1f;
    }
}

