using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour {

    public Position pos;
    public Position posLastStable;

    public Vector2 v2StartLocation;
    public Vector2 v2GoalLocation;

    public const float fTileDistHorz = 0.75f;
    public const float fTileDistVert = 0.43333f;

    public const float fTileGapHorz = 0.14f;
    public const float fTileGapVert = 0.1f;

    public Direction.Dir dirTowardCenter;
    public Direction.Dir dirCascadeFrom;

    public int nI;
    public int nJ;

    public bool bCannotBeCleared;
    public Text txtDebug;

    public enum DELETIONSTATUS { ACTIVE, FLAGGED, DELETED}; //TODO:: Get rid of this if possible
    public DELETIONSTATUS deletionStatus;

    public Collection toCollectBy;

    public Property prop;

    private Colour privcolour;
    public Colour colour {
        get {
            if (privcolour == null) privcolour = GetComponent<Colour>();
            return privcolour;
        }
    }

    public void Init(int i, int j) {
        SetPositon(new Position() {
            i = i,
            j = j
        });

        this.transform.localPosition = GetBoardLocation(pos);
        SaveStablePos();
    }

    public void SetProperty(Property _prop) {

        prop = _prop;
        prop.SetTile(this);
        prop.Init();
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
    }

    public static Vector2 GetBoardLocation(Position pos) {
        return new Vector2(pos.i * fTileDistHorz + (pos.i - 1) * fTileGapHorz,
                         -(pos.j * fTileDistVert + (pos.j - 1) * fTileGapVert));
    }

    public void SaveStablePos() {
        posLastStable = pos;
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
