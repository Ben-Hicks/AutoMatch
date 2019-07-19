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
                    yield return null;
                }
            } else {
                yield return null;
            }
        }

    }

    //TODO cost

    public IEnumerator UseWithTarget(Direction.Dir dir) {
        Tile tileTarget = Board.Get().At(owner.tile.pos.PosInDir(dir));

        return UseWithTarget(tileTarget);
    }

    public IEnumerator UseWithTarget(Position _posTarget) {

        return UseWithTarget(Board.Get().At(_posTarget));
    }

    public IEnumerator UseWithTarget(Tile _tileTarget) {

        if (CanUse() && CanTarget(_tileTarget)) {

            SetTarget(_tileTarget);
            PayCost();

            yield return ExecuteAbility();

            //clean out the target
            SetTarget(owner.tile);
        }
        
    }

    public Ability(Entity _owner) {
        owner = _owner;
    }

    public bool CanTarget(Direction.Dir dir) {
        Tile tilePotentialTarget = Board.Get().At(owner.tile.pos.PosInDir(dir));
        return CanTarget(tilePotentialTarget);
    }

    public bool CanTarget(Position posTarget) {
        if(Board.Get().ValidTile(posTarget) == false) {
            return false;
        }
        return CanTarget(Board.Get().At(posTarget));
    }

    public abstract bool CanTarget(Tile _tileTarget);

    public void SetTarget(Direction.Dir dir) {
        SetTarget(Board.Get().At(owner.tile.pos.PosInDir(dir)));
    }

    public void SetTarget(Position posTarget) {
        SetTarget(Board.Get().At(posTarget));
    }

    public void SetTarget(Tile _tileTarget) {
        tileTarget = _tileTarget;
    }

    public abstract bool CanUse();

    public abstract void PayCost();

    protected abstract List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Position posToTarget);

    public List<Telegraph.TeleTileInfo> TelegraphedTiles(Position posToTarget) {
        if (CanTarget(Board.Get().At(posToTarget)) == false) {
            //If the tile is not a valid target, then telegraph it as invalid
            Telegraph.TeleTileInfo invTeleInfo = new Telegraph.TeleTileInfo {
                pos = posToTarget,
                telegraphType = Telegraph.TelegraphType.Harmful,
                markerType = Telegraph.MarkerType.Invalid,
                dir = Direction.Dir.NONE
            };
            return new List<Telegraph.TeleTileInfo>() { invTeleInfo };
        } else {
            //If the tile's valid, then determine the exact tiles the ability will affect

            return GenListTelegraphTiles(posToTarget);
        }
    }

    public abstract IEnumerator ExecuteAbility();
}
