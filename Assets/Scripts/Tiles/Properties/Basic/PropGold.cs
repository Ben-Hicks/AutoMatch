using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropGold : Property {


    public override void OnCollect(Collection collection) {

        collection.nGold++;

    }
}
