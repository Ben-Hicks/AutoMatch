using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorRejuvenator : SelectorEnemy {

    bool bMovedLastTurn;

    //TODO:: Make a Targetting Component behaviour that can be swapped out as needed
    public override void AcquireTarget() {
        SetTarget(GameController.Get().entHero);
    }

    public override void DecideNextAbility() {

        Direction.Dir dirAdjacentTarget = owner.tile.pos.GetAdjacentDir(entTarget.tile.pos);

        if (dirAdjacentTarget == Direction.Dir.NONE) {
            //If we aren't adjacent to the target, then we can either move in a random direction, or heal
            if (bMovedLastTurn) {
                intended = new Intended(owner.lstAbilities[(int)Entity.ABILSLOT.ABIL1], owner.tile.pos);
                bMovedLastTurn = false;
            } else {
                //Choose a random direction to move in
                intended = new Intended(owner.lstAbilities[(int)EntHero.ABILSLOT.MOVEMENT], Direction.lstAllDirs[Random.Range(0, Direction.NUMDIRECTIONS)]);
                bMovedLastTurn = true;
            }
        } else {
            //If we are adjacent, then we should try to run away from the target
            intended = new Intended(owner.lstAbilities[(int)Entity.ABILSLOT.MOVEMENT], Direction.Negate(dirAdjacentTarget));
        }

        Debug.Log("Planning to use " + intended.abil + " with " + intended.intendType + " and dir: " + intended.dir + " and pos: " + intended.pos);
    }

}
