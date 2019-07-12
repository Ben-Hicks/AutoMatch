using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability {

    public Entity owner;

    public Tile tileTarget;


    //TODO cost

    public IEnumerator UseWithTarget(Direction.Dir dir) {
        Tile tileTarget = Board.Get().At(owner.tile.pos.PosInDir(dir));

        yield return UseWithTarget(tileTarget);
    }

    public IEnumerator UseWithTarget(Tile _tileTarget) {

        if (CanUse() && CanTarget(tileTarget)) {

            SetTarget(tileTarget);
            PayCost();

            yield return ExecuteAbility();

            //clean out the target
            SetTarget(null);
        }
        
    }

    public Ability(Entity _owner) {
        owner = _owner;
    }

    public bool CanTarget(Direction.Dir dir) {
        Tile tileTarget = Board.Get().At(owner.tile.pos.PosInDir(dir));
        return CanTarget(tileTarget);
    }

    public abstract bool CanTarget(Tile _tileTarget);

    public void SetTarget(Tile _tileTarget) {
        tileTarget = _tileTarget;
    }

    public abstract bool CanUse();

    public abstract void PayCost();

    public abstract IEnumerator ExecuteAbility();
}
