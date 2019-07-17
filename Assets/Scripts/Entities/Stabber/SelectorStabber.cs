using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorStabber : AbilitySelector {



    //TODO:: Make a Targetting Component behaviour that can be swapped out as needed
    public override void AcquireTarget() {
        SetTarget(GameController.Get().entHero);
    }

    public override IEnumerator SelectAndUseAbility() {

        //Initially, we'll retarget - maybe this should only be done periodically?
        AcquireTarget();

        //We currently only have a movement action, so we'll always pick that
        yield return MoveTowardTarget();
    }


   
}
