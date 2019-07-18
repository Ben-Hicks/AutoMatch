using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphController : Singleton<TelegraphController> {

    public Ability abilDisplay;
    public float fDelayStandard;
    public float fDelayEnemyTelegraph;

    public float fFadeoutTime;

    public IEnumerator AnimateTelegraph(Ability _abilDisplay, float fDelay) {

        Debug.Log("Fill this in with the actual display for the needed tiles for the ability");
        Debug.Log("Also set the alpha to 1");

        //As long as we're still supposed to be displaying this ability, keep displaying it forever
        while(abilDisplay == _abilDisplay) {
            yield return new WaitForSeconds(0.1f); 
        }

        //After the ability to display has changed, fadeout the telegraphs
        float fFadeoutStartTime = Time.timeSinceLevelLoad;
        while (true) {

            float fElapsedTime = Time.timeSinceLevelLoad - fFadeoutStartTime;
            float fProgress = Mathf.Min(1f, fElapsedTime / fFadeoutTime);

            Debug.Log("set the alpha for each tile in abilDisplay");
            Debug.Log("though be sure to not change the alpha if it is a member of the current abilDisplay's list of affected tiles");

            if (fProgress == 1f) break;
        }

    }

    public void SetAbilityToTelegraph(Ability _abilDisplay, float fDelay) {
        abilDisplay = _abilDisplay;

        StartCoroutine(AnimateTelegraph(_abilDisplay, fDelay));

    }

    public override void Init() {
        
    }
}
