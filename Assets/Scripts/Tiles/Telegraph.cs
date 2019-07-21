using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telegraph : MonoBehaviour {

    public Tile tile;

    public enum TelegraphType { None, Movement, Helpful, Special, Harmful  };
    public static Color[] lstTelegraphColors = new Color[5] { Color.clear, Color.grey, Color.green, Color.yellow, Color.red };
    public TelegraphType telegraphType;

    public SpriteRenderer rendBackground;

    public enum MarkerType { None, Direction, Invalid };
    public MarkerType markerType;

    public struct TeleTileInfo {
        public Position pos;
        public TelegraphType telegraphType;
        public MarkerType markerType;
        public Direction.Dir dir;
    };

    public SpriteRenderer rendMarker;

    public Direction.Dir dir;

    public void Start() {
        SetTelegraphType();
        SetMarkerType();
    }

    public void SetTelegraphType() {

        rendBackground.color = lstTelegraphColors[(int)telegraphType];
        if (telegraphType != TelegraphType.None) {
            Debug.Log("Set color to " + rendBackground.color + " since telegraphType is " + telegraphType);
        }
    }

    public void SetMarkerType() {

        if(markerType == MarkerType.None) {
            rendMarker.sprite = null;
        } else {

            string sPath = "Sprites/spr" + markerType.ToString();

            Debug.Log("Searching for sprite " + sPath);

            Sprite sprMarker = Resources.Load<Sprite>(sPath);

            if(sprMarker == null) {
                Debug.Log("Sprite not found with path " + sPath);
            }

            rendMarker.sprite = sprMarker;

            switch (markerType) {
                case MarkerType.Direction:
                    rendMarker.color = rendBackground.color;
                    break;

                case MarkerType.Invalid:
                    rendMarker.color = Color.red;
                    break;
            }
        }
    }

    public void SetAlpha(float fAlpha) {
        Color colBackground = rendBackground.color;
        Color colMarker = rendMarker.color;

        colBackground.a = fAlpha;
        colMarker.a = fAlpha;

        rendBackground.color = colBackground;
        rendMarker.color = colMarker;
    }

    public void SetTelegraph(TeleTileInfo teleTileInfo) {

        if(telegraphType != TelegraphType.None && telegraphType < teleTileInfo.telegraphType) {
            //If we are trying to set a telegraph type with lower priority than the current one (and it's not a reset), then ignore it
            return;
        }

        telegraphType = teleTileInfo.telegraphType;
        SetTelegraphType();


        if (telegraphType > teleTileInfo.telegraphType || markerType != MarkerType.None ) {
            //Update the marker type only if we're upgrading the priority of our telegraph type, 
            // or if we're maintaining the same priority, and the new markertype isn't null
            markerType = teleTileInfo.markerType;
            dir = teleTileInfo.dir;

            SetMarkerType();
        }
        
        
        

    }

    public void ClearTelegraph() {
        SetTelegraph(new TeleTileInfo {
            pos = tile.pos,
            telegraphType = TelegraphType.None,
            markerType = MarkerType.None,
            dir = Direction.Dir.NONE
        });
    }

}
