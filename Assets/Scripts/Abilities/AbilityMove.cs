using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMove : Ability {


    public AbilityMove(Entity _owner): base(_owner) {

    }

    public override void PayCost() {
        //TODO - something here
    }

    public override bool CanTarget(Tile _tileTarget) {
        Debug.Log("_tileTarget is " + _tileTarget);

        return owner.tile.pos.GetAdjacentDir(_tileTarget.pos) != Direction.Dir.NONE &&
            _tileTarget.prop.bBlocksMovement == false;

    }

    public override bool CanUse() {
        return true;
    }

    public override IEnumerator ExecuteAbility() {
        Debug.Log("Executing Move Ability");
        Board.Get().SwapTile(owner.tile, owner.tile.pos.GetAdjacentDir(tileTarget.pos));
        yield return Board.Get().AnimateMovingTiles();

    }
}
