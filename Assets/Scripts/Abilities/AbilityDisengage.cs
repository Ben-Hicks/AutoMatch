using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDisengage : Ability {

    public const int nAttackDist = 1;
    public const int nJumpbackDist = 2;

    public override void InitProperties() {
        nMinRange = 1;
        nMaxRange = nAttackDist;
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

        Direction.Dir dirAttack = dirDist.dir;
        Direction.Dir dirJumpback = Direction.Negate(dirAttack);

        Position posCur = owner.tile.pos.PosInDir(dirAttack);
        int nCurDist = 1;

        while (Board.Get().ActiveTile(posCur) && nCurDist <= nAttackDist) {

            Board.Get().StartCoroutine(Board.Get().At(posCur).AnimateSwell());
            Board.Get().At(posCur).prop.TakeDamage();

            yield return new WaitForSeconds(owner.GetAnimTime(0.05f));

            posCur = posCur.PosInDir(dirDist.dir);
            nCurDist++;
        }

        nCurDist = 0;

        while (nCurDist < nJumpbackDist && Board.Get().ActiveTile(owner.tile.pos.PosInDir(dirJumpback)) &&
            Board.Get().At(owner.tile.pos.PosInDir(dirJumpback)).prop.bBlocksMovement == false) {
            Board.Get().MoveTile(owner.tile, dirJumpback);
            nCurDist++;
        }

        yield return Board.Get().AnimateMovingTiles(owner.GetAnimTime(Board.Get().fStandardAnimTime));

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {
        List<Telegraph.TeleTileInfo> lstTeleTarget = new List<Telegraph.TeleTileInfo>();

        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posToTarget);

        Direction.Dir dirAttack = dirDist.dir;
        Direction.Dir dirJumpback = Direction.Negate(dirAttack);

        Position posCur = owner.tile.pos;

        //Include each tile in a line in the targetted direction (up to the maximum dist away)
        for (int i = 0; i < nAttackDist; i++) {
            posCur = posCur.PosInDir(dirAttack);

            lstTeleTarget.Add(new Telegraph.TeleTileInfo {
                pos = posCur,
                telegraphType = Telegraph.TelegraphType.Harmful
            });
        }

        //Reset posCur to our current position so we can telegraph the movement
        posCur = owner.tile.pos;

        //Include each tile in a line in the opposite direction (up to the maximum dist away)
        for (int i = 0; i < nJumpbackDist; i++) {
            posCur = posCur.PosInDir(dirJumpback);

            lstTeleTarget.Add(new Telegraph.TeleTileInfo {
                pos = posCur,
                telegraphType = Telegraph.TelegraphType.Movement
            });
        }

        return lstTeleTarget;
    }
}
