using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : Singleton<InputController> {

    public const float fTimeHoldingInput = 0.1f;
    private Tile tileSelected;
    private float fTimeSinceSelected;

    private bool bCheckedKeyboardInput;

    public Tile SelectedTile() {
        //If we haven't already found an input this frame, then check if the keyboard is being pressed
        if(fTimeSinceSelected != 0f) {
            CheckKeyboardInput();
        } else {
            CheckKeyboardInput();
        }

        //Check if our inputted 
        if(fTimeSinceSelected <= fTimeHoldingInput) {
            return tileSelected;
        } else {
            return null;
        }

    }

    public void RegisterTileClicked(Tile tileClicked) {
        //we'll assume that only one tile is being clicked on per frame - if we overwrite something, then oh well
        tileSelected = tileClicked;
        fTimeSinceSelected = 0f;
    }

    public void CheckKeyboardInput() {

        Direction.Dir dirInputted = Direction.Dir.NONE;

        if (Input.GetKeyDown(KeyCode.Q)) {
            dirInputted = Direction.Dir.UL;
        } else if (Input.GetKeyDown(KeyCode.W)) {
            dirInputted = Direction.Dir.U;
        } else if (Input.GetKeyDown(KeyCode.E)) {
            dirInputted = Direction.Dir.UR;
        } else if (Input.GetKeyDown(KeyCode.A)) {
            dirInputted = Direction.Dir.DL;
        } else if (Input.GetKeyDown(KeyCode.S)) {
            dirInputted = Direction.Dir.D;
        } else if (Input.GetKeyDown(KeyCode.D)) {
            dirInputted = Direction.Dir.DR;
        }

        if(dirInputted != Direction.Dir.NONE) {
            Position posInputted = GameController.Get().entHero.tile.pos.PosInDir(dirInputted);
            if (Board.Get().ActiveTile(posInputted)) {
                //If we are trying to select a tile that exists
                tileSelected = Board.Get().At(posInputted);
                fTimeSinceSelected = 0f;
            }

        }

    }

    public override void Init() {

    }

    // Update is called once per frame
    void Update() {
        fTimeSinceSelected += Time.deltaTime;
    }
}
