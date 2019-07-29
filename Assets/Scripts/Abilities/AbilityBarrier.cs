using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBarrier : Ability {

    public override void InitProperties() {
        nMinRange = 1;
        nMaxRange = 2;
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

    private void SpawnBlocker(Position pos) {
        //Save a copy of the current colour so that we can restore it
        Colour.Col colPrev = Board.Get().At(pos).colour.col;

        PropertyController.Get().PlaceProperty("Blocker", Board.Get().At(pos));
        Board.Get().At(pos).colour.SetCol(colPrev);

        Board.Get().StartCoroutine(Board.Get().At(pos).AnimateSwell());
    }

    public override IEnumerator ExecuteAbility(Entity owner, Position posTarget) {

        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posTarget);

        Direction.Dir dirSide1 = Direction.Clockwise(Direction.Clockwise(dirDist.dir));
        Direction.Dir dirSide2 = Direction.CounterClockwise(Direction.CounterClockwise(dirDist.dir));

        Position posSide1 = posTarget.PosInDir(dirSide1);
        Position posSide2 = posTarget.PosInDir(dirSide2);

        SpawnBlocker(posTarget);
        SpawnBlocker(posSide1);
        SpawnBlocker(posSide2);

        yield return new WaitForSeconds(owner.GetAnimTime(0.1f));

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {
        List<Telegraph.TeleTileInfo> lstTeleTarget = new List<Telegraph.TeleTileInfo>();

        Position.DirDist dirDist = owner.tile.pos.DirDistTo(posToTarget);

        Direction.Dir dirSide1 = Direction.Clockwise(Direction.Clockwise(dirDist.dir));
        Direction.Dir dirSide2 = Direction.CounterClockwise(Direction.CounterClockwise(dirDist.dir));

        Position posSide1 = posToTarget.PosInDir(dirSide1);
        Position posSide2 = posToTarget.PosInDir(dirSide2);

        lstTeleTarget.Add(new Telegraph.TeleTileInfo {
            pos = posToTarget,
            telegraphType = Telegraph.TelegraphType.Special,
            markerType = Telegraph.MarkerType.None,
            dir = Direction.Dir.NONE
        });

        lstTeleTarget.Add(new Telegraph.TeleTileInfo {
            pos = posSide1,
            telegraphType = Telegraph.TelegraphType.Special,
            markerType = Telegraph.MarkerType.None,
            dir = Direction.Dir.NONE
        });

        lstTeleTarget.Add(new Telegraph.TeleTileInfo {
            pos = posSide2,
            telegraphType = Telegraph.TelegraphType.Special,
            markerType = Telegraph.MarkerType.None,
            dir = Direction.Dir.NONE
        });

        return lstTeleTarget;
    }
}
