using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Property : MonoBehaviour {

    public GameObject go;
    
    public Tile tile;
    public bool bBlocksMovement;
    public bool bTakesDamage;
    public bool bTakesHealing;

    public bool bCanBeCollected;
    
    public Colour.Col colDefault;

    public enum PRIORITY { NEG, NONE, LOW, MID, HIGH, INFINITE };
    public PRIORITY overwritePriority;


    public Colour colour {
        get { return tile.colour; }
        set { tile.colour.SetCol(value.col); }
    }

    public void Init() {
        SetDefaultColour();
    }

    public virtual void SetDefaultColour() {
        if (colDefault == Colour.Col.RANDOM) {
            colour.SetRandomColour();
        } else {
            colour.SetCol(colDefault);
        }
    }

    //By default, tiles will be deleted when they're matched
    public virtual void FlagForDeletion() {

        if (bCanBeCollected == false) {
            //If the tile can't be collected, then we shouldn't set it for any deletion
        } else {
            tile.deletionStatus = Tile.DELETIONSTATUS.FLAGGED;
            Board.Get().setFlaggedToClear.Add(tile);
        }

    }

    public void SetGameObject(GameObject _go) {
        Debug.Assert(_go != null);

        go = _go;
    }

    public void SetTile(Tile _tile) {
        Debug.Assert(_tile != null);

        tile = _tile;
    }

    public virtual bool CanTakeHealing() {
        return bTakesHealing;
    }

    public virtual bool CanTakeDamage() {
        return bTakesDamage;
    }

    public void TakeHealing(int nAmount = 1) {
        if (CanTakeHealing()) {
            OnHealing(nAmount);
        }
    }

    public void TakeDamage(int nAmount = 1) {
        if (CanTakeDamage()) {
            OnDamage(nAmount);
        }
    }
    
    public virtual void OnHealing(int nAmount) {
        //By default, do nothing when trying to damage a standard tile
    }

    public virtual void OnDamage(int nAmount) {
        //By default, do nothing when trying to damage a standard tile
    }

    public abstract void OnCollect(Collection collection);

}
