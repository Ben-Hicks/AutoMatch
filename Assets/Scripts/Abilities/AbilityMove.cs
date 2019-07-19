using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMove : Ability {


    public AbilityMove(Entity _owner): base(_owner) {

    }

    public override IEnumerator AttemptManualUse() {
        //Since movement is the default action, we're just going to try once
        // to see if we're supposed to move, and if not - we can just exit without doing anything

        Tile tileTarget = InputController.Get().SelectedTile();
        

        if (tileTarget != null) {

            if (CanUse() && CanTarget(tileTarget)) {

                //if we actually found an ability we can use successfully, then let our selector know
                ((SelectorManual)owner.abilityselector).bUsedAbility = true;

                yield return UseWithTarget(tileTarget);
            } else {
                Debug.Log("Invalid target for move action at " + tileTarget.pos.ToString());
                yield return null;
            }
        }

    }

    public override void PayCost() {
        //TODO - something here
    }

    public override bool CanTarget(Tile _tileTarget) {

        return owner.tile.pos.GetAdjacentDir(_tileTarget.pos) != Direction.Dir.NONE &&
            _tileTarget.prop.bBlocksMovement == false;

    }

    public override bool CanUse() {
        return true;
    }

    public override IEnumerator ExecuteAbility() {

        Board.Get().MoveTile(owner.tile, owner.tile.pos.GetAdjacentDir(tileTarget.pos));
        yield return Board.Get().AnimateMovingTiles(owner.GetAnimTime(Board.Get().fStandardAnimTime));

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Position posToTarget) {
        return new List<Telegraph.TeleTileInfo>() {
            new Telegraph.TeleTileInfo {
                pos = posToTarget,
                telegraphType = Telegraph.TelegraphType.Movement
            }
        };
    }
}
