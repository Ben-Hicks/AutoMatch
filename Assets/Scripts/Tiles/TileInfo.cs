using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour {
    

    public Position pos;

    public bool bActive;

    public Direction.Dir dirTowardCenter;
    public Direction.Dir dirCascadeFrom;

    public int nPathDistToPlayer;
    public int nDirectDistToPlayer;

    public void SetActive(bool _bActive) {

        bActive = _bActive;

        Board.Get().At(pos).SetDebugText(bActive.ToString());

    }

    public void SetDirTowardCenter(Direction.Dir _dirTowardCenter) {
        dirTowardCenter = _dirTowardCenter;
        dirCascadeFrom = Direction.Negate(dirTowardCenter);
    }

    public void UpdateDirectDistToPlayer() {
        //We assume that the player is always in the center of the board
        pos.DirectDistFrom(Board.Get().posCenter);
    }

    public void Init(int i, int j) {
        pos = new Position(i, j);
    }

    public void UpdatePathDistToPlayer(int _nPathDistToPlayer) {
        nPathDistToPlayer = _nPathDistToPlayer;
    }
}
