using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorStabber : SelectorEnemy {



    //TODO:: Make a Targetting Component behaviour that can be swapped out as needed
    public override void AcquireTarget() {
        SetTarget(GameController.Get().entHero);
        Debug.Log("Target acquired " + entTarget);
    }

    public override void DecideNextAbility() {
        //For now, just move towards the target
        PlanMoveTowardTarget();
    }
   
}
