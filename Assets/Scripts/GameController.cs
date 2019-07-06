using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController> {

    

    public float fDelayBetweenClears;

    public override void Init() {
        
    }

    //TODO:: Consider if this can be initialized by this class rather than the board, but in a way that 
    //       could ensure the board gets a chance to finish initiallizing itself
    public IEnumerator GameLoop() {

        while (true) {
            //Initially ensure that our starting matches all get cleaned up

            while (true) {
                int nFlagged = Board.Get().FlagMatches();

                //If there were no flagged matches, we can leave this loop
                if (nFlagged == 0) break;

                yield return new WaitForSeconds(fDelayBetweenClears);

                //If there are flagged matches, keep cleaning them up
                yield return Board.Get().CleanupMatches();

                
            }


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

            //An example of waiting for an input and halting progress on other things
            /*while (true) {
                //For now, wait until the P key is pressed
                if (Input.GetKeyDown(KeyCode.P)) {
                    Debug.Log("Pressed P");
                    break;
                }

                yield return null;

            }*/

        }
    }


}
