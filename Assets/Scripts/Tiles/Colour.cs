using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colour : MonoBehaviour {

    public const int NUMCOLOURS = 10;
    public enum Col { WILD, BLUE, RED, GREEN, GOLD, PURPLE, CYAN, BLACK, PINK, ORANGE, RANDOM };
    public static Color[] arColors = new Color[] {
        new Color(255f/255f, 255f/255f, 255f/255f),
        new Color(1f/255f, 0, 72f/255f),
        new Color(154f/255f, 0, 18f/255f),
        new Color(2f/255f, 57f/255f, 0),
        new Color(221f/255f, 189f/255f, 1f/255f),
        new Color(67f/255f, 0, 118f/255f),
        new Color(165f/255f, 187f/255f, 252f/255f),
        new Color(0, 0, 0),
        new Color(245f/255f, 171f/255f, 202f/255f),
        new Color(161f/255f, 83f/255f, 0)
    };
    

    public Col col;
    public Color colorSet;
    
    public CloudParticle cloudParticle;
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
                colToSet = arColors[(int)_col];
                break;

            case Col.BLUE:
                colToSet = arColors[(int)_col];
                break;

            case Col.RED:
                colToSet = arColors[(int)_col];
                break;

            case Col.GREEN:
                colToSet = arColors[(int)_col];
                break;

            case Col.GOLD:
                colToSet = arColors[(int)_col];
                break;

            case Col.PURPLE:
                colToSet = arColors[(int)_col];
                break;

            case Col.CYAN:
                colToSet = arColors[(int)_col];
                break;

            case Col.BLACK:
                colToSet = arColors[(int)_col];
                break;

            case Col.PINK:
                colToSet = arColors[(int)_col];
                break;

            case Col.ORANGE:
                colToSet = arColors[(int)_col];
                break;

            default:
                Debug.LogError("Unrecognized Colour!");
                colToSet = Color.white;
                break;
        }

        colorSet = colToSet;
        rendBackground.color = colToSet;
        cloudParticle.SetStartColour(colToSet);
    }

    public void SetCol(Col _col) {
        col = _col;

        DisplayColour();
    }
    
}
