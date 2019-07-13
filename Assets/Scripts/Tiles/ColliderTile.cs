using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTile : MonoBehaviour {

    public Tile owner;

    public void OnMouseOver() {

    }

    public void OnMouseUpAsButton() {
        InputController.Get().RegisterTileClicked(owner);
    }

}
