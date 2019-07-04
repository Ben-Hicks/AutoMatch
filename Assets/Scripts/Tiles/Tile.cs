using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour {

    public Position pos;
    public Position posLastStable;

    public const float fTileDistHorz = 0.75f;
    public const float fTileDistVert = 0.43333f;

    public const float fTileGapHorz = 0.14f;
    public const float fTileGapVert = 0.1f;

    public Direction.Dir dirTowardCenter;
    public Direction.Dir dirCascadeFrom;

    public int nI;
    public int nJ;

    public Colour colour;
    public Text txtDebug;

    public enum DELETIONSTATUS { ACTIVE, FLAGGED, DELETED};
    public DELETIONSTATUS deletionStatus;
    

    public void Init(int i, int j) {
        SetPositon(new Position() {
            i = i,
            j = j
        });

        PositionTile();

    }

    public void SetDirTowardCenter(Direction.Dir _dirTowardCenter) {
        dirTowardCenter = _dirTowardCenter;
        dirCascadeFrom = Direction.Negate(dirTowardCenter);
       // colour.DisplayColour((Colour.Col)(dirTowardCenter - 1));
    }

    public void SetPositon(Position _pos) {
        pos = _pos;

        nI = pos.i;
        nJ = pos.j;

        PositionTile();
    }

    public void PositionTile() {

        this.transform.localPosition = new Vector2(pos.i * fTileDistHorz + (pos.i - 1) * fTileGapHorz,
                                                  -(pos.j * fTileDistVert + (pos.j - 1) * fTileGapVert));

    }

    public void SaveStablePos() {
        posLastStable = pos;
    }

    public void FlagClear() {

        SetDebugText("Flagged");
        //TODO:: Have triggers like bombs or static-unclearable tiles

        //TODO:: Also add collecting mana/xp/whatever when cleared

        deletionStatus = DELETIONSTATUS.FLAGGED;
    }

    public void SetDebugText(string s) {
        if (txtDebug == null) txtDebug = GetComponentInChildren<Text>();
        
        txtDebug.text = s;
    }

   

    // Start is called before the first frame update
    void Start() {

        

    }

    // Update is called once per frame
    void Update() {

    }
}
