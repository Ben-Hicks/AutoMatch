using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropBlocker : Property {


    public override void OnCollect(Collection collection) {
        base.OnCollect(collection);

        collection.nGold++;
        
    }
}
