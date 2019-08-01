using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionParticle : MonoBehaviour {

    public float fTotalTime;
    public float fExplosionTime;
    public float fTravelTime;
    public ParticleSystem partSys;


    void Start() {

        partSys = GetComponent<ParticleSystem>();
        fTotalTime = partSys.main.duration;
        fTravelTime = fTotalTime - fExplosionTime;

        this.transform.position = new Vector3(transform.position.x,
                                             transform.position.y, -1f);
    }

    public void SetStartColour(Color color) {
        //ParticleSystem.MainModule mainmod = partSys.main;
        // mainmod.startColor = color;
        ParticleSystem.MainModule settings = GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(color);
    }

    public IEnumerator MoveToDestination(Tile tileStart) {

        //First, figure out where we should move toward
        GameObject goDestination;
        if(tileStart.toCollectBy == null) {
            //If no one's collecting us, then just have the tile itself act as 
            // the collecting object
            goDestination = tileStart.gameObject;
        } else {
            goDestination = tileStart.toCollectBy.gameObject;
        }

        //Also make sure that we set our colour to the colour of the collected tile

        SetStartColour(tileStart.colour.colorSet);

        float fStartTime = Time.timeSinceLevelLoad;

        while(Time.timeSinceLevelLoad - fStartTime <= fExplosionTime) {
            //until we have finished exploding, don't move
            yield return null;
        }

        float fStartTravelTime = Time.timeSinceLevelLoad;

        float fProgress = 0f;
        while(fProgress <= 1f) {
            fProgress = Mathf.Min(1f, (Time.timeSinceLevelLoad - fStartTravelTime) / fTravelTime);
            transform.position = Library.LerpSmoothIn(tileStart.transform.position, goDestination.transform.position, fProgress);
            yield return null;
        }

        tileStart.toCollectBy = null;
        //Once we've finished moving to the destination, then destroy ourselves
        Destroy(this.gameObject);
    }
}
