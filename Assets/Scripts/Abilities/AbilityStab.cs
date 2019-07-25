using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityStab : Ability {

    public const int nLength = 2;

    public override void PayCost(Entity owner) {
        //TODO - something here
    }

    public override bool CanTarget(Entity owner, Tile _tileTarget) {
        Debug.Log("Should make a way to check if a tile is in a straight line of the start");

        return owner.tile.pos.GetAdjacentDir(_tileTarget.pos) != Direction.Dir.NONE;

    }

    public override bool CanUse(Entity owner) {
        return true;
    }

    public override IEnumerator ExecuteAbility(Entity owner, Tile tileTarget) {

        Position posCur = tileTarget.pos;
        Direction.Dir dir = owner.tile.pos.GetAdjacentDir(posCur);
        int nCurLength = 1;

        while (Board.Get().ActiveTile(posCur) && nCurLength <= nLength) {


            Board.Get().StartCoroutine(Board.Get().At(posCur).AnimateSwell());
            Board.Get().At(posCur).prop.TakeDamage();

            yield return new WaitForSeconds(owner.GetAnimTime(0.05f));

            posCur = posCur.PosInDir(dir);
            nCurLength++;
        }

        yield return new WaitForSeconds(owner.GetAnimTime(0.1f));

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {

        List<Telegraph.TeleTileInfo> lstToTelegraph = new List<Telegraph.TeleTileInfo>();

        Position posCur = posToTarget;
        Direction.Dir dirAim = owner.tile.pos.GetAdjacentDir(posToTarget);
        int nCurLength = 1;

        while (Board.Get().ActiveTile(posCur) && nCurLength <= nLength) {

            lstToTelegraph.Add(new Telegraph.TeleTileInfo {
                                        pos = posCur,
                                        telegraphType = Telegraph.TelegraphType.Harmful,
                                        markerType = Telegraph.MarkerType.None,
                                        dir = Direction.Dir.NONE
                                    });

            posCur = posCur.PosInDir(dirAim);
            nCurLength++;
        }

        return lstToTelegraph;
    }
}
