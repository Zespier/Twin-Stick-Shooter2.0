using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFeedback : MonoBehaviour {

    public ParticleType type;
    public ParticleSystem particleSystem;

}
public enum ParticleType : byte {
    smallExplosion = 0,
    mediumExplosion= 1,
    bigExplosion= 2,
}