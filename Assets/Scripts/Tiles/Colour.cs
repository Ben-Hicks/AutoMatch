using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colour : MonoBehaviour {

    public const int NUMCOLOURS = 6;
    public enum Col { WILD, BLUE, RED, GREEN, GOLD, PURPLE };

    public Col col;
    public Tile tile;

    public SpriteRenderer rendBackground;

    public bool CanMatch(Colour other) {
        if(col == Col.WILD || other.col == Col.WILD) {
            return true;
        }

        return col == other.col;
    }

    public void SetNextColour() {

        if ((int)col == NUMCOLOURS - 1) SetColour((Col)1);
        else SetColour(col + 1);
    }

    public void DisplayColour() {

        Color colToSet;

        switch (col) {
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

            default:
                Debug.LogError("Unrecognized Colour!");
                colToSet = Color.white;
                break;
        }

        rendBackground.color = colToSet;
    }

    public void SetColour(Col _col) {
        col = _col;

        DisplayColour();
    }

    // Start is called before the first frame update
    void Start() {
       

    }

    // Update is called once per frame
    void Update() {

    }
}
