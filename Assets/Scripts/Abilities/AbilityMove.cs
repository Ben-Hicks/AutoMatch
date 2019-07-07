using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMove : Ability {

    public override void PayCost() {
        //TODO - something here
    }

    public override bool CanTarget(Tile _tileTarget) {

        return owner.tile.pos.GetAdjacentDir(_tileTarget.pos) != Direction.Dir.NONE;

    }

    public override bool CanUse() {
        return true;
    }

    public override IEnumerator ExecuteAbility() {

        Board.Get().SwapTile(owner.tile, owner.tile.pos.GetAdjacentDir(tileTarget.pos));

        return null;
    }
}
