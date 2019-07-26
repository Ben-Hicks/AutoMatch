using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityIcicleWave : Ability {

    public override void InitProperties() {
        nMinRange = 1;
        nMaxRange = 100;
    }

    public override void PayCost(Entity owner) {
        //TODO - something here
    }

    public override bool CanTarget(Entity owner, Tile _tileTarget) {
        //Check if there's any generic reasons why the targetting would be invalid
        if (base.CanTarget(owner, _tileTarget) == false) return false;

        Debug.Log("Should make a way to check if a tile is in a straight line of the start");

        return true;

    }

    public override bool CanUse(Entity owner) {
        return true;
    }

    public override IEnumerator ExecuteAbility(Entity owner, Tile tileTarget) {

        Position posCur = tileTarget.pos;
        Direction.Dir dir = owner.tile.pos.GetAdjacentDir(posCur);

        while (Board.Get().ActiveTile(posCur)) {
            PropertyController.Get().PlaceProperty("Icicle", Board.Get().At(posCur));
            Board.Get().StartCoroutine(Board.Get().At(posCur).AnimateSwell());
            yield return new WaitForSeconds(owner.GetAnimTime(0.1f));

            posCur = posCur.PosInDir(dir);
        }

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {

        Direction.Dir dirAim = owner.tile.pos.GetAdjacentDir(posToTarget);

        return new List<Telegraph.TeleTileInfo>() {
            new Telegraph.TeleTileInfo {
                pos = posToTarget,
                telegraphType = Telegraph.TelegraphType.Harmful,
                markerType = Telegraph.MarkerType.Direction,
                dir = dirAim
            }
        };
    }
}
