using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public Position pos;

    public const float fTileDistHorz = 0.75f;
    public const float fTileDistVert = 0.43333f;

    public const float fTileGapHorz = 0.14f;
    public const float fTileGapVert = 0.1f;

    public int nI;
    public int nJ;

    public Colour colour;
    public Direction.Dir dirTowardCenter;
    

    public void Init(int i, int j) {
        pos = new Position() {
            i = i,
            j = j
        };

        nI = i;
        nJ = j;

        PositionTile();

    }

    public void PositionTile() {

        this.transform.localPosition = new Vector2(pos.i * fTileDistHorz + (pos.i - 1) * fTileGapHorz,
                                                   pos.j * fTileDistVert + (pos.j - 1) * fTileGapVert);

    }

    // Start is called before the first frame update
    void Start() {

        

    }

    // Update is called once per frame
    void Update() {

    }
}
