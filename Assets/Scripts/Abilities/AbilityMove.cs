using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMove : Ability {

    public override void InitProperties() {
        nMinRange = 1;
        nMaxRange = 1;
        tarType = TargetType.RELATIVE;
    }

    public override IEnumerator AttemptManualUse(Entity owner) {
        //Since movement is the default action, we're just going to try once
        // to see if we're supposed to move, and if not - we can just exit without doing anything

        Tile tileTarget = InputController.Get().SelectedTile();
        

        if (tileTarget != null) {

            if (CanUse(owner) && CanTarget(owner, tileTarget.pos)) {

                //if we actually found an ability we can use successfully, then let our selector know
                ((SelectorManual)owner.abilityselector).bUsedAbility = true;

                yield return UseWithTarget(owner, tileTarget.pos);
            } else {
                Debug.Log("Invalid target for move action from " + owner.tile.pos + " to " + tileTarget.pos.ToString());
                yield return null;
            }
        }

    }

    public override void PayCost(Entity owner) {
        //TODO - something here
    }

    public override bool CanTarget(Entity owner, Position posTarget) {
        //Check if there's any generic reasons why the targetting would be invalid
        if (base.CanTarget(owner, posTarget) == false) return false;

        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posTarget);

        //Ensure that there is a straight line between us and the target
        return dirDist.dir != Direction.Dir.NONE && 
            Board.Get().At(owner.tile.pos.PosInDir(dirDist.dir)).prop.bBlocksMovement == false;
    }

    public override bool CanUse(Entity owner) {
        return true;
    }

    public override IEnumerator ExecuteAbility(Entity owner, Position posTarget) {

        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posTarget);

        Board.Get().MoveTile(owner.tile, dirDist.dir);
        yield return Board.Get().AnimateMovingTiles(owner.GetAnimTime(Board.Get().fStandardAnimTime));

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {
        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posToTarget);

        return new List<Telegraph.TeleTileInfo>() {
            new Telegraph.TeleTileInfo {
                pos = owner.tile.pos.PosInDir(dirDist.dir),
                telegraphType = Telegraph.TelegraphType.Movement
            }
        };
    }
}
