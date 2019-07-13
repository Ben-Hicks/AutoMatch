using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Property : MonoBehaviour {

    public GameObject go;
    
    public Tile tile;
    public bool bBlocksMovement;

    public enum PRIORITY { NEG, NONE, LOW, MID, HIGH, INFINITE };
    public PRIORITY overwritePriority;


    public Colour colour {
        get { return tile.colour; }
        set { tile.colour.SetCol(value.col); }
    }

    public virtual void SetDefaultPropertyValues() {
        bBlocksMovement = false;
    }

    public void Init() {
        SetDefaultColour();
        SetDefaultPropertyValues();
    }

    public virtual void SetDefaultColour() {
        colour.SetRandomColour();
    }

    //By default, tiles will be deleted when they're matched
    public virtual void FlagForDeletion() {

        tile.deletionStatus = Tile.DELETIONSTATUS.FLAGGED;
        Board.Get().setFlaggedToClear.Add(tile);

    }

    public void SetGameObject(GameObject _go) {
        Debug.Assert(_go != null);

        go = _go;
    }

    public void SetTile(Tile _tile) {
        Debug.Assert(_tile != null);

        tile = _tile;
    }

    public abstract void OnCollect(Collection collection);



}
