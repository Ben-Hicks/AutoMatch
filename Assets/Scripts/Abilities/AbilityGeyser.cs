using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGeyser : Ability {

    public override void InitProperties() {
        nMinRange = 2;
        nMaxRange = 3;
        tarType = TargetType.ABSOLUTE;
    }

    public override void PayCost(Entity owner) {
        //TODO - something here
    }

    public override bool CanTarget(Entity owner, Position posTarget) {
        //Check if there's any generic reasons why the targetting would be invalid
        if (base.CanTarget(owner, posTarget) == false) return false;

        return true;
    }

    public override bool CanUse(Entity owner) {
        return true;
    }

    public override IEnumerator ExecuteAbility(Entity owner, Position posTarget) {

        foreach (Direction.Dir dir in Direction.lstAllDirs) {
            Board.Get().StartCoroutine(Board.Get().At(posTarget.PosInDir(dir)).AnimateSwell());
            Board.Get().At(posTarget.PosInDir(dir)).prop.TakeDamage();
            PropertyController.Get().PlaceProperty("Water", Board.Get().At(posTarget.PosInDir(dir)));
        }

        yield return new WaitForSeconds(owner.GetAnimTime(0.1f));

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {

        List<Telegraph.TeleTileInfo> lstToTelegraph = new List<Telegraph.TeleTileInfo>();

        foreach (Direction.Dir dir in Direction.lstAllDirs) {

            lstToTelegraph.Add(new Telegraph.TeleTileInfo {
                pos = posToTarget.PosInDir(dir),
                telegraphType = Telegraph.TelegraphType.Harmful,
                markerType = Telegraph.MarkerType.None,
                dir = Direction.Dir.NONE
            });
        }

        return lstToTelegraph;
    }
}
