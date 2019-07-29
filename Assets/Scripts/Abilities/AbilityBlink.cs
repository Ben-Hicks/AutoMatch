using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBlink : Ability {

    public override void InitProperties() {
        nMinRange = 2;
        nMaxRange = 3;
        tarType = TargetType.RELATIVE;
    }

    public override void PayCost(Entity owner) {
        //TODO - something here
    }

    public override bool CanTarget(Entity owner, Position posTarget) {
        //Check if there's any generic reasons why the targetting would be invalid
        if (base.CanTarget(owner, posTarget) == false) return false;

        //Currently only allowing teleports in a line
        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posTarget);
        if (dirDist.dir == Direction.Dir.NONE) return false;

        //Ensure that the targetted location doesn't block movement
        return Board.Get().At(posTarget).prop.bBlocksMovement == false;
    }

    public override bool CanUse(Entity owner) {
        return true;
    }

    public override IEnumerator ExecuteAbility(Entity owner, Position posTarget) {

        Board.Get().SwapTiles(owner.tile.pos, posTarget);

        yield return Board.Get().AnimateMovingTiles(owner.GetAnimTime(Board.Get().fStandardAnimTime));

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {
        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posToTarget);

        return new List<Telegraph.TeleTileInfo>() {
            new Telegraph.TeleTileInfo {
                pos = posToTarget,
                telegraphType = Telegraph.TelegraphType.Movement
            }
        };
    }
}
