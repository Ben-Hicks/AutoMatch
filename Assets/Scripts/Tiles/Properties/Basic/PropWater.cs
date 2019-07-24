using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropWater : Property {

    public override void SetDefaultColour() {
        colour.SetCol(Colour.Col.BLUE);
    }


    public override void OnCollect(Collection collection) {

        collection.nGold++;

    }
}
