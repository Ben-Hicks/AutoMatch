using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntRejuvenator : Entity {

    public static int nCurID = 0;
    public int nID;

    public override void OnCollect(Collection collection) {

    }

    public override void SetDefaultColour() {
        colour.SetCol(Colour.Col.GREEN);
        nID = nCurID;
        nCurID++;
    }

    public override void InitSpecialAbilities() {
        lstAbilities[(int)ABILSLOT.ABIL1] = new AbilityRejuvenate(this);
    }

}