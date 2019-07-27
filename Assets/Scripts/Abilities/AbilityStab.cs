using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityStab : Ability {

    public const int nLength = 2;

    public override void InitProperties() {
        nMinRange = 1;
        nMaxRange = nLength;
    }

    public override void PayCost(Entity owner) {
        //TODO - something here
    }

    public override bool CanTarget(Entity owner, Position posTarget) {
        //Check if there's any generic reasons why the targetting would be invalid
        if (base.CanTarget(owner, posTarget) == false) return false;

        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posTarget);

        //Ensure that there is a straight line between us and the target
        return dirDist.dir != Direction.Dir.NONE;
    }

    public override bool CanUse(Entity owner) {
        return true;
    }

    public override IEnumerator ExecuteAbility(Entity owner, Position posTarget) {

        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posTarget);
        
        Position posCur = owner.tile.pos.PosInDir(dirDist.dir);
        int nCurLength = 1;

        while (Board.Get().ActiveTile(posCur) && nCurLength <= nLength) {


            Board.Get().StartCoroutine(Board.Get().At(posCur).AnimateSwell());
            Board.Get().At(posCur).prop.TakeDamage();

            yield return new WaitForSeconds(owner.GetAnimTime(0.05f));

            posCur = posCur.PosInDir(dirDist.dir);
            nCurLength++;
        }

        yield return new WaitForSeconds(owner.GetAnimTime(0.1f));

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {

        List<Telegraph.TeleTileInfo> lstToTelegraph = new List<Telegraph.TeleTileInfo>();

        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posToTarget);
        
        Position posCur = owner.tile.pos.PosInDir(dirDist.dir);
        int nCurLength = 1;

        while (Board.Get().ActiveTile(posCur) && nCurLength <= nLength) {

            lstToTelegraph.Add(new Telegraph.TeleTileInfo {
                                        pos = posCur,
                                        telegraphType = Telegraph.TelegraphType.Harmful,
                                        markerType = Telegraph.MarkerType.None,
                                        dir = Direction.Dir.NONE
                                    });

            posCur = posCur.PosInDir(dirDist.dir);
            nCurLength++;
        }

        return lstToTelegraph;
    }
}
