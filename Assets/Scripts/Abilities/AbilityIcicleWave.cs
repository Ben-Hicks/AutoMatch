using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityIcicleWave : Ability {

    public override void InitProperties() {
        nMinRange = 1;
        nMaxRange = 100;
        tarType = TargetType.RELATIVE;
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

        while (Board.Get().ActiveTile(posCur)) {
            PropertyController.Get().PlaceProperty("Icicle", Board.Get().At(posCur));
            Board.Get().StartCoroutine(Board.Get().At(posCur).AnimateSwell());
            yield return new WaitForSeconds(owner.GetAnimTime(0.1f));

            if (Board.Get().At(posCur).prop.CanTakeDamage()) {
                Board.Get().At(posCur).prop.TakeDamage();
                break;
            }

            posCur = posCur.PosInDir(dirDist.dir);
        }

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {

        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posToTarget);

        return new List<Telegraph.TeleTileInfo>() {
            new Telegraph.TeleTileInfo {
                pos = owner.tile.pos.PosInDir(dirDist.dir),
                telegraphType = Telegraph.TelegraphType.Harmful,
                markerType = Telegraph.MarkerType.Direction,
                dir = dirDist.dir
            }
        };
    }
}
