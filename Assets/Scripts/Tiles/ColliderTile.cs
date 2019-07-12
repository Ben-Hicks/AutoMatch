using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTile : MonoBehaviour {

    public Tile owner;

    public void OnMouseOver() {
        Debug.Log("Mouse Over");
    }

    public void OnMouseUpAsButton() {
        Debug.Log("Mouse button " + Time.timeSinceLevelLoad);
        InputController.Get().RegisterTileClicked(owner);
    }

}
