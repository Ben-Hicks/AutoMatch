using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityImpact : Ability {

    public const int nAttackDist = 1;
    public const int nKnockbackDist = 2;

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

        Position posCur = owner.tile.pos.PosInDir(dirDist.dir);


        Board.Get().StartCoroutine(Board.Get().At(posCur).AnimateSwell());
        yield return new WaitForSeconds(owner.GetAnimTime(0.3f));

        Tile tileTarget = Board.Get().At(posCur);

        if (tileTarget.prop.CanTakeDamage()) {
            tileTarget.prop.TakeDamage();

            //If we successfully hit a target that can take damage, then knock them back
            Direction.Dir dirKnockback = dirDist.dir;

            int nCurDist = 0;

            while (nCurDist < nKnockbackDist && Board.Get().ActiveTile(tileTarget.pos.PosInDir(dirKnockback)) &&
                Board.Get().At(tileTarget.pos.PosInDir(dirKnockback)).prop.bBlocksMovement == false) {
                Board.Get().MoveTile(tileTarget, dirKnockback);
                nCurDist++;
            }

            yield return Board.Get().AnimateMovingTiles(owner.GetAnimTime(Board.Get().fStandardAnimTime));
        }

        

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {
        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posToTarget);

        return new List<Telegraph.TeleTileInfo>() {
            new Telegraph.TeleTileInfo {
                pos = owner.tile.pos.PosInDir(dirDist.dir),
                telegraphType = Telegraph.TelegraphType.Harmful,
                markerType = Telegraph.MarkerType.Direction,
                dir = Direction.Dir.NONE
            }
        };
    }
}
