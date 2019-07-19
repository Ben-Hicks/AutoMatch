using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectorEnemy : AbilitySelector {

    public Ability abilIntended;
    public Position posIntended;

    public void Start() {
        AcquireTarget();
        DecideNextAbility();
    }

    public void PlanMoveTowardTarget() {
        //Assuming the user has basic 1-tile movement options, this will move them one tile toward their set target

        Direction.Dir dirCurrentBest = Direction.Dir.NONE;

        Dictionary<Tile, int> dictPathDistsToTarget = new Dictionary<Tile, int>();

        Board.Get().GetPathDistsTo(entTarget.tile, ref dictPathDistsToTarget);

        int nClosestPathDist = -1;
        int nClosestDirectDist = -1;

        //For each tile we can immediately reach - TODO:: Abstract this to check the results of where we can end up as a result of our move actions
        foreach (Direction.Dir dir in Direction.lstAllDirs) {
            Position posToMoveTo = owner.tile.pos.PosInDir(dir);
            if (Board.Get().ValidTile(posToMoveTo) && Board.Get().At(posToMoveTo).prop.bBlocksMovement == false) {
                //If this tile is on the board, and we can move onto it, then check how far away it would be from the player

                int nPathDist = dictPathDistsToTarget[Board.Get().At(posToMoveTo)];
                int nDirectDist = posToMoveTo.DirectDistFrom(entTarget.tile.pos);

                if (dirCurrentBest == Direction.Dir.NONE ||
                    nPathDist < nClosestPathDist ||
                    (nPathDist == nClosestPathDist && nDirectDist < nClosestDirectDist)) {
                    //If this is the first non-none direction we've found, then we can accept it
                    //Otherwise, if we have a current best direction, check if we have a shorter pathdistance in this direction
                    //And if its the same, then we can break ties by examining the direct distance


                    //Update the best direction (and the distances for the tile in that direction);
                    dirCurrentBest = dir;
                    nClosestPathDist = nPathDist;
                    nClosestDirectDist = nDirectDist;
                }
            }
        }

        //If there's a direction we can move in (even if it's not necessarily closer), we'll plan to move in that direction
        if (dirCurrentBest != Direction.Dir.NONE) {
            abilIntended = owner.lstAbilities[(int)Entity.ABILSLOT.MOVEMENT];
            posIntended = owner.tile.pos.PosInDir(dirCurrentBest);
        } else {
            //Otherwise, if we can't move towards the target in a good way, just pass
            abilIntended = owner.lstAbilities[(int)Entity.ABILSLOT.PASS];
            posIntended = owner.tile.pos;
        }

        
    }

    public abstract void DecideNextAbility();

    public override IEnumerator SelectAndUseAbility() {
        
        //First, check if the inteded ability and its targetting is still valid
        if(abilIntended == null || abilIntended.CanUse() == false || abilIntended.CanTarget(Board.Get().At(posIntended)) == false) {

            //if the ability either can't be used, or the target is now invalid, then by default, do nothing
            abilIntended = owner.lstAbilities[(int)Entity.ABILSLOT.PASS];
            posIntended = owner.tile.pos;
        }

        //Use the valid ability
        yield return abilIntended.UseWithTarget(posIntended);

        //Reacquire whatever target is most appropriate now
        AcquireTarget();

        //Decide what the next ability and targetting will be
        DecideNextAbility();

        //Let the TelegraphController know to broadcast the intended next action
        StartCoroutine(TelegraphController.Get().AnimateEnemyTelegraph(abilIntended, posIntended));
    }
}
