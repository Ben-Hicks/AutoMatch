using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilitySelector : MonoBehaviour {

    public Entity owner;
    public Entity entTarget;

    public abstract IEnumerator SelectAndUseAbility();

    public void SetTarget(Entity _entTarget) {
        entTarget = _entTarget;
    }

    //Use whatever base logic to decide who to attack - typically the player
    public virtual void AcquireTarget() {

        SetTarget(GameController.Get().entHero);

    }

    public void Start() {
        owner = GetComponent<Entity>();

        AcquireTarget();
    }



    public IEnumerator MoveTowardTarget() {
        //Assuming the user has basic 1-tile movement options, this will move them one tile toward their set target

        Direction.Dir dirCurrentBest = Direction.Dir.NONE;

        Dictionary<Tile, int> dictPathDistsToTarget = new Dictionary<Tile, int>();

        Board.Get().GetPathDistsTo(entTarget.tile, ref dictPathDistsToTarget);

        int nClosestPathDist = -1;
        int nClosestDirectDist = -1;

        //For each tile we can immediately reach - TODO:: Abstract this to check the results of where we can end up as a result of our move actions
        foreach(Direction.Dir dir in Direction.lstAllDirs) {
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

        //If there's a direction we can move in (even if it's not necessarily closer), we'll move in that direction
        if (dirCurrentBest != Direction.Dir.NONE) {
            Ability abilToUse = owner.lstAbilities[(int)Entity.ABILSLOT.MOVEMENT];

            if (abilToUse.CanUse() && abilToUse.CanTarget(dirCurrentBest)) {

                yield return abilToUse.UseWithTarget(dirCurrentBest);
            }
        }
    }
}
