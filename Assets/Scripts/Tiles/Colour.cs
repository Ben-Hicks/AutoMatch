using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colour : MonoBehaviour {

    public const int NUMCOLOURS = 8;
    public enum Col { WILD, BLUE, RED, GREEN, GOLD, PURPLE, CYAN, BLACK };

    public Col col;

    public SpriteRenderer rendBackground;

    public bool CanMatch(Colour other) {
        if(col == Col.WILD || other.col == Col.WILD) {
            return true;
        }

        return col == other.col;
    }

    public void SetNextColour() {

        if ((int)col == NUMCOLOURS - 1) SetCol((Col)1);
        else SetCol(col + 1);
    }

    public void SetRandomColour() {
        Col colRand = (Col)Random.Range(1, NUMCOLOURS);
        SetCol(colRand);
    }

    public void DisplayColour() {
        DisplayColour(col);
    }

    public void DisplayColour(Colour.Col _col) {

        Color colToSet;

        switch (_col) {
            case Col.WILD:
                colToSet = Color.gray;
                break;

            case Col.BLUE:
                colToSet = Color.blue;
                break;

            case Col.RED:
                colToSet = Color.red;
                break;

            case Col.GREEN:
                colToSet = Color.green;
                break;

            case Col.GOLD:
                colToSet = Color.yellow;
                break;

            case Col.PURPLE:
                colToSet = Color.magenta;
                break;

            case Col.CYAN:
                colToSet = Color.cyan;
                break;

            case Col.BLACK:
                colToSet = Color.black;
                break;

            default:
                Debug.LogError("Unrecognized Colour!");
                colToSet = Color.white;
                break;
        }

        rendBackground.color = colToSet;
    }

    public void SetCol(Col _col) {
        col = _col;

        DisplayColour();
    }
    
}
