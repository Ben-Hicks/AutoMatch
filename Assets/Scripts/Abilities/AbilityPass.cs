using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPass : Ability {

    public override void InitProperties() {
        nMinRange = 0;
        nMaxRange = 10000;
        tarType = TargetType.RELATIVE;
    }

    public override IEnumerator AttemptManualUse(Entity owner) {
        //This should always be accepted, but I currently don't have plans for how to manually select this
      
        yield return UseWithTarget(owner, owner.tile.pos);

    }

    public override void PayCost(Entity owner) {
        //TODO - something here
    }

    public override bool CanTarget(Entity owner, Position posTarget) {
        return true;
    }

    public override bool CanUse(Entity owner) {
        return true;
    }

    public override IEnumerator ExecuteAbility(Entity owner, Position posTarget) {

        Debug.Log(owner + " is Passing");
        yield return new WaitForSeconds(0.5f);

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {
        return new List<Telegraph.TeleTileInfo>() {
            new Telegraph.TeleTileInfo {
                pos = owner.tile.pos,
                telegraphType = Telegraph.TelegraphType.Movement
            }
        };
    }
}

