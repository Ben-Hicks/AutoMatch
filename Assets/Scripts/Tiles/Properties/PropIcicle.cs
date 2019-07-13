using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropIcicle : Property {


    public override void SetDefaultColour() {
        colour.SetCol(Colour.Col.CYAN);
    }

    public override void OnCollect(Collection collection) {
        collection.nGold++;

        Board.Get().setFlaggedToClear.Add(tile);
    }
}
