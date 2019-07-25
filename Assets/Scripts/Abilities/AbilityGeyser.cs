using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGeyser : Ability {

    public const int nRange = 3;

    public override void PayCost(Entity owner) {
        //TODO - something here
    }

    public override bool CanTarget(Entity owner, Tile _tileTarget) {

        int nDirectDist = owner.tile.pos.DirectDistFrom(_tileTarget.pos);

        return nDirectDist <= nRange;
    }

    public override bool CanUse(Entity owner) {
        return true;
    }

    public override IEnumerator ExecuteAbility(Entity owner, Tile tileTarget) {

        foreach (Direction.Dir dir in Direction.lstAllDirs) {
            Board.Get().StartCoroutine(Board.Get().At(tileTarget.pos.PosInDir(dir)).AnimateSwell());
            Board.Get().At(tileTarget.pos.PosInDir(dir)).prop.TakeDamage();
            PropertyController.Get().PlaceProperty("Water", Board.Get().At(tileTarget.pos.PosInDir(dir)));
        }

        yield return new WaitForSeconds(owner.GetAnimTime(0.1f));

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {

        List<Telegraph.TeleTileInfo> lstToTelegraph = new List<Telegraph.TeleTileInfo>();

        foreach (Direction.Dir dir in Direction.lstAllDirs) {

            lstToTelegraph.Add(new Telegraph.TeleTileInfo {
                pos = posToTarget.PosInDir(dir),
                telegraphType = Telegraph.TelegraphType.Harmful,
                markerType = Telegraph.MarkerType.None,
                dir = Direction.Dir.NONE
            });
        }

        return lstToTelegraph;
    }
}
