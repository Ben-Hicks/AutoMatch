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

        if(teleTileInfo.telegraphType != TelegraphType.None && telegraphType > teleTileInfo.telegraphType) {
            //If we are trying to set a telegraph type with lower priority than the current one (and it's not a reset), then ignore it
            return;
        }

        if (teleTileInfo.telegraphType == TelegraphType.None || telegraphType < teleTileInfo.telegraphType || teleTileInfo.markerType != MarkerType.None ) {
            //Update the marker type only if we're upgrading the priority of our telegraph type, 
            // or if we're maintaining the same priority, and the new markertype isn't null (also allow directly setting to None)

            telegraphType = teleTileInfo.telegraphType;
            SetTelegraphType();

            markerType = teleTileInfo.markerType;
            dir = teleTileInfo.dir;

            SetMarkerType();
        } else {
            //Otherwise, if the new telegraph has the same priority as the current one, and
            // the new marker is null, then we don't need to update the telegraph type or the marker type
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
