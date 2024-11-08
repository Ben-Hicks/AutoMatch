﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityRejuvenate : Ability {

    public override void InitProperties() {
        nMinRange = 3;
        nMaxRange = 100;
    }

    public override void PayCost(Entity owner) {
        //TODO - something here
    }

    public override bool CanTarget(Entity owner, Position posTarget) {
        if (base.CanTarget(owner, posTarget) == false) return false;

        return true;
    }

    public override bool CanUse(Entity owner) {
        return true;
    }

    public override IEnumerator ExecuteAbility(Entity owner, Position posTarget) {

        foreach(Direction.Dir dir in Direction.lstAllDirs) {
            Board.Get().StartCoroutine(Board.Get().At(owner.tile.pos.PosInDir(dir)).AnimateSwell());
            Board.Get().At(owner.tile.pos.PosInDir(dir)).prop.TakeHealing();
        }

        Board.Get().At(owner.tile.pos).prop.TakeDamage();

        yield return new WaitForSeconds(owner.GetAnimTime(0.1f));

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget) {

        List<Telegraph.TeleTileInfo> lstToTelegraph = new List<Telegraph.TeleTileInfo>();

        foreach (Direction.Dir dir in Direction.lstAllDirs) {

            lstToTelegraph.Add(new Telegraph.TeleTileInfo {
                pos = owner.tile.pos.PosInDir(dir),
                telegraphType = Telegraph.TelegraphType.Helpful,
                markerType = Telegraph.MarkerType.None,
                dir = Direction.Dir.NONE
            });
        }

        lstToTelegraph.Add(new Telegraph.TeleTileInfo {
            pos = owner.tile.pos,
            telegraphType = Telegraph.TelegraphType.Harmful,
            markerType = Telegraph.MarkerType.None,
            dir = Direction.Dir.NONE
        });


        return lstToTelegraph;
    }
}
