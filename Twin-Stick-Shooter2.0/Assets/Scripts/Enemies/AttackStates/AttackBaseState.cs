using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBaseState : MonoBehaviour {

    public Enemy controller;

    public abstract void OnStateEnter();

    public abstract void OnStateExit();

    public abstract void StateLateUpdate();

    public abstract void StateUpdate();

}
