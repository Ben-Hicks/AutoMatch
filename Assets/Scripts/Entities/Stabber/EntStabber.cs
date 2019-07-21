﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntStabber : Entity {

    public static int nCurID = 0;
    public int nID;

    public override void OnCollect(Collection collection) {
        base.OnCollect(collection);

    }

    public override void SetDefaultColour() {
        colour.SetCol(Colour.Col.RED);
        nID = nCurID;
        nCurID++;
    }

}
