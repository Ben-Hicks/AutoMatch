using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability {

    public Entity owner;

    public Tile tileTarget;


    //TODO cost

    public abstract bool CanTarget(Tile _tileTarget);

    public void SetTarget(Tile _tileTarget) {
        tileTarget = _tileTarget;
    }

    public abstract bool CanUse();

    public abstract void PayCost();

    public abstract IEnumerator ExecuteAbility();
}
