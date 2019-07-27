using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDash : Ability {

    public const int nDist = 3;

    public override void InitProperties() {
        nMinRange = 1;
        nMaxRange = nDist;
        tarType = TargetType.RELATIVE;
    }

    public override void PayCost(Entity owner) {
        //TODO - something here
    }

    public override bool CanTarget(Entity owner, Position posTarget) {
        //Check if there's any generic reasons why the targetting would be invalid
        if (base.CanTarget(owner, posTarget) == false) return false;

        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posTarget);

        return dirDist.dir != Direction.Dir.NONE;

    }

    public override bool CanUse(Entity owner) {
        return true;
    }

    public override IEnumerator ExecuteAbility(Entity owner, Position posTarget) {

        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posTarget);
        int nCurDist = 0;

        while (nCurDist < nDist && Board.Get().ActiveTile(owner.tile.pos.PosInDir(dirDist.dir)) && 
            Board.Get().At(owner.tile.pos.PosInDir(dirDist.dir)).prop.bBlocksMovement == false) {
            Board.Get().MoveTile(owner.tile, dirDist.dir);
            nCurDist++;
        }

        yield return Board.Get().AnimateMovingTiles(owner.GetAnimTime(Board.Get().fStandardAnimTime));
        
    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {
        List<Telegraph.TeleTileInfo> lstTeleTarget = new List<Telegraph.TeleTileInfo>();

        Direction.Dir dir = owner.tile.pos.GetAdjacentDir(posToTarget);
        Position posCur = owner.tile.pos.PosInDir(dir);

        //Include each tile in a line in the targetted direction (up to the maximum dist away)
        for (int i=0; i<nDist; i++){
            posCur = posCur.PosInDir(dir);

            lstTeleTarget.Add(new Telegraph.TeleTileInfo {
                pos = posCur,
                telegraphType = Telegraph.TelegraphType.Movement
            });
        }

        return lstTeleTarget;
    }
}
