using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorStabber : SelectorEnemy {



    //TODO:: Make a Targetting Component behaviour that can be swapped out as needed
    public override void AcquireTarget() {
        SetTarget(GameController.Get().entHero);
    }

    public override void DecideNextAbility() {

        Direction.Dir dirAdjacentTarget = owner.tile.pos.GetAdjacentDir(entTarget.tile.pos);

        if(dirAdjacentTarget == Direction.Dir.NONE) {
            //If we aren't adjacent to the target, then keep moving toward them
            PlanMoveTowardTarget(entTarget.tile);
        } else {
            //If we are adjacent, then we should try to stab them
            intended = new Intended(owner.lstAbilities[(int)Entity.ABILSLOT.ABIL1], dirAdjacentTarget);
        }

        Debug.Log("Planning to use " + intended.abil + " with " + intended.intendType + " and dir: " + intended.dir + " and pos: " + intended.pos);
    }
   
}
