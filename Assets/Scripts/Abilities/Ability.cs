using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability {

    public int nMaxRange;
    public int nMinRange;

    public enum TargetType { RELATIVE, ABSOLUTE };
    public TargetType tarType;

    public Ability() {
        InitProperties();
    }
    
    //Define the values of properties of this ability (cost/range/etc)
    public abstract void InitProperties();

    //Try to use this ability (and gather any targets)
    public virtual IEnumerator AttemptManualUse(Entity owner) {

        while (true) {

            Tile tileTarget = InputController.Get().SelectedTile();

            if (Input.GetKeyUp(KeyCode.Escape)) {
                Debug.Log("Broke from selecting a target");
                yield break;
            }else if (tileTarget != null) {
                Debug.Log("We have a targetted tile");
                if (CanUse(owner) && CanTarget(owner, tileTarget)) {
                    Debug.Log("The target was valid so we'll use the ability");

                    //if we actually found an ability we can use successfully, then let our selector know
                    ((SelectorManual)owner.abilityselector).bUsedAbility = true;

                    yield return UseWithTarget(owner, tileTarget);
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

    public IEnumerator UseWithTarget(Entity owner, Direction.Dir dir) {
        Tile tileTarget = Board.Get().At(owner.tile.pos.PosInDir(dir));

        return UseWithTarget(owner, tileTarget);
    }

    public IEnumerator UseWithTarget(Entity owner, Position posTarget) {

        return UseWithTarget(owner, Board.Get().At(posTarget));
    }

    public IEnumerator UseWithTarget(Entity owner, Tile tileTarget) {

        if (CanUse(owner) && CanTarget(owner, tileTarget)) {
            
            PayCost(owner);

            yield return ExecuteAbility(owner, tileTarget);

        }
        
    }

    public bool CanTarget(Entity owner, Direction.Dir dir) {
        Tile tilePotentialTarget = Board.Get().At(owner.tile.pos.PosInDir(dir));
        return CanTarget(owner, tilePotentialTarget);
    }

    public bool CanTarget(Entity owner, Position posTarget) {
        if(Board.Get().ValidTile(posTarget) == false) {
            return false;
        }
        return CanTarget(owner, Board.Get().At(posTarget));
    }

    public virtual bool CanTarget(Entity owner, Tile _tileTarget) {
        int nTargettedDist = owner.tile.pos.DirectDistFrom(_tileTarget.pos);

        return nTargettedDist >= nMinRange && nTargettedDist <= nMaxRange;
    }

    public abstract bool CanUse(Entity owner);

    public abstract void PayCost(Entity owner);

    protected abstract List<Telegraph.TeleTileInfo> GenListTelegraphTiles(Entity owner, Position posToTarget);

    public List<Telegraph.TeleTileInfo> TelegraphedTiles(Entity owner, Position posToTarget) {
        if (CanTarget(owner, Board.Get().At(posToTarget)) == false) {
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

            return GenListTelegraphTiles(owner, posToTarget);
        }
    }

    public abstract IEnumerator ExecuteAbility(Entity owner, Tile tileTarget);
}
