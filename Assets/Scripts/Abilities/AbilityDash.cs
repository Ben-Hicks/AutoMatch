using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDash : Ability {

    public const int nDist = 3;

    public AbilityDash(Entity _owner) : base(_owner) {

    }

    public override void PayCost() {
        //TODO - something here
    }

    public override bool CanTarget(Tile _tileTarget) {
        Debug.Log("Should make a way to check if a tile is in a straight line of the start");

        return owner.tile.pos.GetAdjacentDir(_tileTarget.pos) != Direction.Dir.NONE;

    }

    public override bool CanUse() {
        return true;
    }

    public override IEnumerator ExecuteAbility() {
        
        Direction.Dir dir = owner.tile.pos.GetAdjacentDir(tileTarget.pos);
        int nCurDist = 0;

        while (nCurDist < nDist && Board.Get().ValidTile(owner.tile.pos.PosInDir(dir)) && Board.Get().At(owner.tile.pos.PosInDir(dir)).prop.bBlocksMovement == false) {
            Board.Get().MoveTile(owner.tile, dir);
            nCurDist++;
        }

        yield return Board.Get().AnimateMovingTiles();
        
    }
}
