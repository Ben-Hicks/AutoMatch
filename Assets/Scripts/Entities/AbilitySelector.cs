﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilitySelector : MonoBehaviour {

    public Entity owner;
    

    public abstract IEnumerator SelectAndUseAbility();
    
    public virtual void Start() {
        owner = GetComponent<Entity>();
    }



    
}
