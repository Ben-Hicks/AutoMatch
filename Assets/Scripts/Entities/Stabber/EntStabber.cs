using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntStabber : Entity {

    public override void OnCollect(Collection collection) {

    }

    public override void SetDefaultColour() {
        colour.SetCol(Colour.Col.RED);
    }

    public override void SetDefaultPropertyValues() {
        bBlocksMovement = true;
        nCollectionPriority = 5;
    }

}
