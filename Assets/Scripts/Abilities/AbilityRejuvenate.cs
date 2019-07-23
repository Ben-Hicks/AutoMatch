using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityRejuvenate : Ability {

    public AbilityRejuvenate(Entity _owner) : base(_owner) {
        
    }

    public override void PayCost() {
        //TODO - something here
    }

    public override bool CanTarget(Tile _tileTarget) {
        Debug.Log("Any target for rejuvenate is valid - assuming no targetting process is necessary");
        return true;
    }

    public override bool CanUse() {
        return true;
    }

    public override IEnumerator ExecuteAbility() {

        foreach(Direction.Dir dir in Direction.lstAllDirs) {
            Board.Get().StartCoroutine(Board.Get().At(owner.tile.pos.PosInDir(dir)).AnimateSwell());
            Board.Get().At(owner.tile.pos.PosInDir(dir)).prop.TakeHealing();

        }
        yield return new WaitForSeconds(owner.GetAnimTime(0.1f));

    }

    protected override List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Position posToTarget) {

        List<Telegraph.TeleTileInfo> lstToTelegraph = new List<Telegraph.TeleTileInfo>();

        foreach (Direction.Dir dir in Direction.lstAllDirs) {

            lstToTelegraph.Add(new Telegraph.TeleTileInfo {
                pos = owner.tile.pos.PosInDir(dir),
                telegraphType = Telegraph.TelegraphType.Helpful,
                markerType = Telegraph.MarkerType.None,
                dir = Direction.Dir.NONE
            });
        }


        return lstToTelegraph;
    }
}
