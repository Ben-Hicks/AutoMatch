using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudParticle : MonoBehaviour {

    public ParticleSystem partSys;

    // Start is called before the first frame update
    void Start() {

    }

    public void SetStartColour(Color color) {

        ParticleSystem.MainModule settings = GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(color);
    }

}
