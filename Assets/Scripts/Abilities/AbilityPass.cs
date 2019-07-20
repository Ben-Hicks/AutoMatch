using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPass : Ability {

    public AbilityPass(Entity _owner) : base(_owner) {

    }

    public override IEnumerator AttemptManualUse() {
        //This should always be accepted, but I currently don't have plans for how to manually select this
      
        yield return UseWithTarget(owner.tile);

    }

    public override void PayCost() {
        //TODO - something here
    }

    public override bool CanTarget(Tile _tileTarget) {
        return true;
    }

    public override bool CanUse() {
        return true;
    }

    public override IEnumerator ExecuteAbility() {

        Debug.Log(owner + " is Passing");
        yield return new WaitForSeconds(0.5f);

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Position posToTarget) {
        return new List<Telegraph.TeleTileInfo>() {
            new Telegraph.TeleTileInfo {
                pos = owner.tile.pos,
                telegraphType = Telegraph.TelegraphType.Movement
            }
        };
    }
}

