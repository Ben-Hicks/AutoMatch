using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorEnemyBasic : SelectorEnemy {

    public override void AcquireTarget() {
        base.AcquireTarget();
    }

    public override void DecideNextAbility() {
        
        //Make a copy of the priorities for this to use (and tinker with)
        int[] arPriorities = new int[Entity.NUMABILSLOTS];
        System.Array.Copy(owner.arPriorities, arPriorities, Entity.NUMABILSLOTS);

        while (true) {

            Entity.ABILSLOT abilslotDesired = Entity.ABILSLOT.PASS;
            float fPriorityMostDesired = 0.0f;

            //Look through each non-pass action and see which has the highest randomized priority
            for (Entity.ABILSLOT iabilslot = Entity.ABILSLOT.MOVEMENT; iabilslot <= Entity.ABILSLOT.ABIL4; iabilslot++) {
                //If we definitely can't use it, then don't consider it at all
                if (owner.arAbilities[(int)iabilslot].CanUse(owner) == false ||
                    arPriorities[(int)iabilslot] == 0) continue;

                float fPriority = Random.Range(0f, arPriorities[(int)iabilslot]);

                //If the priority for this ability is generated to be the highest so far
                // then we can save it as the next ability to use
                if (fPriority > fPriorityMostDesired) {
                    abilslotDesired = iabilslot;
                    fPriorityMostDesired = fPriority;
                }
            }

            Debug.Log("randomly selected to use " + abilslotDesired);

            //If we've selected to move, then we should choose the best way to move
            if(abilslotDesired == Entity.ABILSLOT.MOVEMENT) {
                PlanBasicMove(entTarget.tile);
                break;
            }

            //Otherwise, we have a move complicated ability to use that we'll need to check

            int nDirectDist = owner.tile.pos.DirectDistFrom(entTarget.tile.pos);
            //Now that we've decided which ability we'll use, we can check if we are in range
            if (nDirectDist < owner.arAbilities[(int)abilslotDesired].nMinRange) {
                //If we are closer than the minimum range of the ability, then we should
                // try to move away from the target entity
                Debug.Log("Were too close so we should move away");
                PlanMoveAwayFromTarget(entTarget.tile);
                break;
            } else if (nDirectDist > owner.arAbilities[(int)abilslotDesired].nMaxRange) {
                //If we are further than the maximum range of the ability, then we should
                // try to move closer to the target entity
                Debug.Log("Were too far so we should move closer");
                PlanMoveTowardTarget(entTarget.tile);
                break;
            } else {
                //If we're in range, then let's plan to use the current ability
                Ability abilInteded = owner.arAbilities[(int)abilslotDesired];

                if (abilInteded.CanTarget(owner, entTarget.tile.pos) == false) {
                    //If we can't actually target this location, then repeat this process 
                    // and hope the next selected action is better (and disallow this ability
                    // from being chosen again)
                    arPriorities[(int)abilslotDesired] = 0;
                    Debug.Log("We couldn't target the player with " + abilslotDesired + " so we're retrying");
                    continue;
                } else {
                    //If we can actually target the targets tile, then let's formally intend to
                    Debug.Log("We successfully chose and can use " + abilslotDesired);
                    intended = new Intended(abilInteded, owner, abilInteded.tarType, entTarget.tile.pos);
                    break;
                }
            }
        }


    }

}
