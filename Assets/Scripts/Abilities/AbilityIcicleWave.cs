using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityIcicleWave : Ability {


    public AbilityIcicleWave(Entity _owner) : base(_owner) {

    }

    public override void PayCost() {
        //TODO - something here
    }

    public override bool CanTarget(Tile _tileTarget) {
        Debug.Log("Should make a way to check if a tile is in a straight line of the start");

        return owner.tile.pos.GetAdjacentDir(_tileTarget.pos) != Direction.Dir.NONE;

    }

    public override bool CanUse() {
        return true;
    }

    public override IEnumerator ExecuteAbility() {

        Position posCur = tileTarget.pos;
        Direction.Dir dir = owner.tile.pos.GetAdjacentDir(posCur);

        while (Board.Get().ValidTile(posCur)) {
            PropertyController.Get().PlaceProperty("Icicle", Board.Get().At(posCur));
            Board.Get().StartCoroutine(Board.Get().At(posCur).AnimateSwell());
            yield return new WaitForSeconds(0.1f);

            posCur = posCur.PosInDir(dir);
        }
        Debug.Log("TODO - expand tiles that are affected");

    }
}
