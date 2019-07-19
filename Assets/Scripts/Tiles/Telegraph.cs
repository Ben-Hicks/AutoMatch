using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telegraph : MonoBehaviour {

    public Tile tile;

    public enum TelegraphType { None, Helpful, Harmful, Movement, Special };
    public Color[] lstTelegraphColors = new Color[5] { Color.clear, Color.green, Color.red, Color.grey, Color.yellow };
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

            Sprite sprMarker = (Sprite)Resources.Load(sPath);

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

        Debug.Log("SetAlpha called with " + fAlpha);
    }

    public void SetTelegraph(TeleTileInfo teleTileInfo) {

        telegraphType = teleTileInfo.telegraphType;
        markerType = teleTileInfo.markerType;
        dir = teleTileInfo.dir;

        SetTelegraphType();
        SetMarkerType();

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
