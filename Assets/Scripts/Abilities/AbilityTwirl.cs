using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTwirl : Ability {

    public override void InitProperties() {
        nMinRange = 1;
        nMaxRange = 1;
        tarType = TargetType.RELATIVE;
    }

    public override void PayCost(Entity owner) {
        //TODO - something here
    }

    public override bool CanTarget(Entity owner, Position posTarget) {

        //The target doesn't really affect anything since we're just spinning
        // the tiles around ourselves
        return true;
    }

    public override bool CanUse(Entity owner) {
        return true;
    }

    public override IEnumerator ExecuteAbility(Entity owner, Position posTarget) {

        Position curPos = owner.tile.pos.PosInDir(Direction.Dir.U);
        Direction.Dir curDir = Direction.Dir.DR;

        while (curDir != Direction.Dir.UR) {
            //If we're swapping tiles around, we have to make sure they're on the board
            if (Board.Get().ValidTile(curPos) == false ||
                Board.Get().ValidTile(curPos.PosInDir(curDir)) == false) continue;

            Board.Get().MoveTile(Board.Get().At(curPos), curDir);
            curPos = curPos.PosInDir(curDir);
            curDir = Direction.Clockwise(curDir);
        }

        yield return Board.Get().AnimateMovingTiles(owner.GetAnimTime(Board.Get().fStandardAnimTime));

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {
        List<Telegraph.TeleTileInfo> lstTeleTarget = new List<Telegraph.TeleTileInfo>();

        Position curPos = owner.tile.pos.PosInDir(Direction.Dir.U);
        Direction.Dir curDir = Direction.Dir.DR;

        while (curDir != Direction.Dir.UR) {
            //If we're swapping tiles around, we have to make sure they're on the board
            if (Board.Get().ValidTile(curPos) == false ||
                Board.Get().ValidTile(curPos.PosInDir(curDir)) == false) continue;

            lstTeleTarget.Add(new Telegraph.TeleTileInfo {
                pos = curPos,
                telegraphType = Telegraph.TelegraphType.Special
            });

            curPos = curPos.PosInDir(curDir);
            curDir = Direction.Clockwise(curDir);
        }

        return lstTeleTarget;
    }
}
