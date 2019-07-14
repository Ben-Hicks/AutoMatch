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

    public int nPathDistToPlayer;
    public int nDirectDistToPlayer;

    public int nI;
    public int nJ;
    
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

    public void UpdateDirectDistToPlayer() {
        nDirectDistToPlayer = pos.DirectDistFrom(GameController.Get().entHero.tile.pos);
    }

    public void UpdatePathDistToPlayer(int _nPathDistToPlayer) {
        nPathDistToPlayer = _nPathDistToPlayer;
        SetDebugText(nDirectDistToPlayer.ToString() + " " + nPathDistToPlayer.ToString());
    }
    

    public void SetDebugText(string s) {
        if (txtDebug == null) txtDebug = GetComponentInChildren<Text>();
        
        txtDebug.text = s;
    }


    public IEnumerator AnimateSwell() {

        float fTimeStart = Time.timeSinceLevelLoad;
        float fSwellSize = Board.Get().fTileSwellSize;

        while (true) {

            float fElapsedTime = Time.timeSinceLevelLoad - fTimeStart;
            float fProgress = Mathf.Min(1f, fElapsedTime / Board.Get().fTileSwellTime);

            Vector3 v3NewScale = Vector3.Lerp(new Vector3(1f, 1f, 1f), new Vector3(fSwellSize, fSwellSize, fSwellSize),
                Mathf.Sin(Mathf.PI * fProgress));

            transform.localScale = v3NewScale;

            //If our progress is complete, we can stop moving
            if (fProgress == 1f) {
                break;
            } else {
                //If we're not complete, we should yield for a frame
                yield return null;
            }
        }

        
    }

    // Start is called before the first frame update
    void Start() {

        

    }

    // Update is called once per frame
    void Update() {

    }
}
