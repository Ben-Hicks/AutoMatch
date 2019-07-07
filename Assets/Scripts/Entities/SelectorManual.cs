using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorManual : AbilitySelector {

    public override IEnumerator SelectAndUseAbility() {

        while (true) {

            Direction.Dir dirToMove = Direction.Dir.NONE;

            if (Input.GetKeyDown(KeyCode.Q)) {
                dirToMove = Direction.Dir.UL;
            } else if (Input.GetKeyDown(KeyCode.W)) {
                dirToMove = Direction.Dir.U;
            } else if (Input.GetKeyDown(KeyCode.E)) {
                dirToMove = Direction.Dir.UR;
            } else if (Input.GetKeyDown(KeyCode.A)) {
                dirToMove = Direction.Dir.DL;
            } else if (Input.GetKeyDown(KeyCode.S)) {
                dirToMove = Direction.Dir.D;
            } else if (Input.GetKeyDown(KeyCode.D)) {
                dirToMove = Direction.Dir.DR;
            }

            if (dirToMove != Direction.Dir.NONE) {
                Board.Get().SwapTile(Board.Get().tilePlayer, dirToMove);
                yield return Board.Get().AnimateMovingTiles();
                break;
            } else {
                yield return null;
            }
        }

    }

    // Start is called before the first frame update
    void Start() { 

        
    }

    // Update is called once per frame
    void Update() {

    }
}
