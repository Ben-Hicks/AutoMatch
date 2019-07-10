using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropBlocker : Property {


    public override void SetDefaultPropertyValues() {
        bBlocksMovement = true;
    }

    public override void OnCollect(Collection collection) {
        collection.nGold++;

        Board.Get().setFlaggedToClear.Add(tile);
    }
}
