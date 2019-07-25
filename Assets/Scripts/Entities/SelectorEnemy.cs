﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO - only instantiate one of each of the types of AbilitySelectors and then share it among all 
//        entities of that type
public abstract class SelectorEnemy : AbilitySelector {

    public Intended intended;

    public Entity entTarget;

    public class Intended {
        public enum IntendType { DIRECTION, POSITION };
        public IntendType intendType;

        public Entity owner;
        public Ability abil;
        public Direction.Dir dir;
        public Position pos;

        public Intended(Ability _abil, Entity _owner, Direction.Dir _dir) {
            owner = _owner;
            intendType = IntendType.DIRECTION;
            abil = _abil;
            dir = _dir;
        }

        public Intended(Ability _abil, Entity _owner, Position _pos) {
            owner = _owner;
            intendType = IntendType.POSITION;
            abil = _abil;
            pos = _pos;
        }

        public Tile GetIntended() {
            if(intendType == IntendType.DIRECTION) {
                return Board.Get().At(owner.tile.pos.PosInDir(dir));
            } else {
                return Board.Get().At(pos);
            }
        }

        public void SetPass() {
            abil = owner.arAbilities[(int)Entity.ABILSLOT.PASS];
        }
    }

    public override void Start() {
        base.Start();
    }

    public void SetTarget(Entity _entTarget) {
        entTarget = _entTarget;
    }

    //Use whatever base logic to decide who to attack - typically the player
    public virtual void AcquireTarget() {

        SetTarget(GameController.Get().entHero);

    }

    public void PlanMoveTowardTarget(Tile tileTarget) {
        //Assuming the user has basic 1-tile movement options, this will move them one tile toward their set target

        Direction.Dir dirCurrentBest = Direction.Dir.NONE;

        Dictionary<Tile, int> dictPathDistsToTarget = new Dictionary<Tile, int>();

        Board.Get().GetPathDistsTo(tileTarget, ref dictPathDistsToTarget);

        int nClosestPathDist = -1;
        int nClosestDirectDist = -1;

        //For each tile we can immediately reach - TODO:: Abstract this to check the results of where we can end up as a result of our move actions
        foreach (Direction.Dir dir in Direction.lstAllDirs) {
            Position posToMoveTo = owner.tile.pos.PosInDir(dir);
            if (Board.Get().ValidTile(posToMoveTo) && Board.Get().At(posToMoveTo).prop.bBlocksMovement == false) {
                //If this tile is on the board, and we can move onto it, then check how far away it would be from the player

                int nPathDist = dictPathDistsToTarget[Board.Get().At(posToMoveTo)];
                int nDirectDist = posToMoveTo.DirectDistFrom(tileTarget.pos);

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
            intended = new Intended(owner.arAbilities[(int)Entity.ABILSLOT.MOVEMENT], owner, dirCurrentBest);
        } else {
            //Otherwise, if we can't move towards the target in a good way, just pass
            intended = new Intended(owner.arAbilities[(int)Entity.ABILSLOT.PASS], owner, Direction.Dir.NONE);
        }

        //Debug.Log("Planning to use " + intended.abil + " with " + intended.intendType + " and dir: " + intended.dir + " and pos: " + intended.pos);
    }

    public void PlanMoveAwayFromTarget(Tile tileTarget) {
        //Assuming the user has basic 1-tile movement options, this will move them one tile toward their set target

        Direction.Dir dirCurrentBest = Direction.Dir.NONE;

        //TODO:: Optimize this so that we can just pull from the saved player distances if the target is the player
        Dictionary<Tile, int> dictPathDistsToTarget = new Dictionary<Tile, int>();

        Board.Get().GetPathDistsTo(tileTarget, ref dictPathDistsToTarget);

        int nFurthestPathDist = -1;
        int nFurthestDirectDist = -1;

        //For each tile we can immediately reach - TODO:: Abstract this to check the results of where we can end up as a result of our move actions
        foreach (Direction.Dir dir in Direction.lstAllDirs) {
            Position posToMoveTo = owner.tile.pos.PosInDir(dir);
            if (Board.Get().ValidTile(posToMoveTo) && Board.Get().At(posToMoveTo).prop.bBlocksMovement == false) {
                //If this tile is on the board, and we can move onto it, then check how far away it would be from the player

                int nPathDist = dictPathDistsToTarget[Board.Get().At(posToMoveTo)];
                int nDirectDist = posToMoveTo.DirectDistFrom(tileTarget.pos);

                if (dirCurrentBest == Direction.Dir.NONE ||
                    nPathDist > nFurthestPathDist ||
                    (nPathDist == nFurthestPathDist && nDirectDist < nFurthestDirectDist)) {
                    //If this is the first non-none direction we've found, then we can accept it
                    //Otherwise, if we have a current best direction, check if we have a longer pathdistance in this direction
                    //And if its the same, then we can break ties by examining the direct distance


                    //Update the best direction (and the distances for the tile in that direction);
                    dirCurrentBest = dir;
                    nFurthestPathDist = nPathDist;
                    nFurthestDirectDist = nDirectDist;
                }
            }
        }

        //If there's a direction we can move in (even if it's not necessarily farther), we'll plan to move in that direction
        if (dirCurrentBest != Direction.Dir.NONE) {
            intended = new Intended(owner.arAbilities[(int)Entity.ABILSLOT.MOVEMENT], owner, dirCurrentBest);
        } else {
            //Otherwise, if we can't move away from the target in a good way, just pass
            intended = new Intended(owner.arAbilities[(int)Entity.ABILSLOT.PASS], owner, Direction.Dir.NONE);
        }

        //Debug.Log("Planning to use " + intended.abil + " with " + intended.intendType + " and dir: " + intended.dir + " and pos: " + intended.pos);
    }

    public abstract void DecideNextAbility();

    public override IEnumerator SelectAndUseAbility() {
        
        //First, check if the inteded ability and its targetting is still valid
        if(intended.abil.CanUse(intended.owner) == false || intended.abil.CanTarget(intended.owner, intended.GetIntended()) == false) {

            //if the ability either can't be used, or the target is now invalid, then by default, do nothing
            intended.SetPass();
        }

        Debug.Log("intended.GetIntended is " + intended.GetIntended());

        //Use the valid ability
        yield return intended.abil.UseWithTarget(intended.owner, intended.GetIntended());

        
    }

    public void PlanNextAbility() {

        if (Board.Get().ActiveTile(owner.tile.pos) == false) {
            //If we aren't active, then move toward the player so we reach the board
            SetTarget(GameController.Get().entHero);
            PlanMoveTowardTarget(entTarget.tile);
        } else {
            //If we are on an active tile, then we can let our behaviour selector choose how we should behave

            //Reacquire whatever target is most appropriate now
            AcquireTarget();


            //Decide what the next ability and targetting will be
            DecideNextAbility();
        }
    }
}
