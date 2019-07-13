using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability {

    public Entity owner;

    public Tile tileTarget;

    //Try to use this ability (and gather any targets)
    public virtual IEnumerator AttemptManualUse() {

        while (true) {

            Tile tileTarget = InputController.Get().SelectedTile();

            if (Input.GetKeyUp(KeyCode.Escape)) {
                Debug.Log("Broke from selecting a target");
                yield break;
            }else if (tileTarget != null) {
                Debug.Log("We have a targetted tile");
                if (CanUse() && CanTarget(tileTarget)) {
                    Debug.Log("The target was valid so we'll use the ability");

                    //if we actually found an ability we can use successfully, then let our selector know
                    ((SelectorManual)owner.abilityselector).bUsedAbility = true;

                    yield return UseWithTarget(tileTarget);
                    break;
                } else {
                    Debug.Log("Invalid target");
                }
            } else {
                yield return null;
            }
        }

    }

    //TODO cost

    public IEnumerator UseWithTarget(Direction.Dir dir) {
        Tile tileTarget = Board.Get().At(owner.tile.pos.PosInDir(dir));

        yield return UseWithTarget(tileTarget);
    }

    public IEnumerator UseWithTarget(Tile _tileTarget) {

        if (CanUse() && CanTarget(_tileTarget)) {

            SetTarget(_tileTarget);
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
