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

    public SpriteRenderer rendMarker;

    public Direction.Dir dir;

    
    public void SetTelegraphType(TelegraphType _telegraphType) {
        telegraphType = _telegraphType;

        rendBackground.color = lstTelegraphColors[(int)telegraphType];
    }

    public void SetMarkerType(MarkerType _markerType) {
        markerType = _markerType;

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
    }

    public void SetTelegraph(TelegraphType _telegraphType, MarkerType _markerType = MarkerType.None) {

        SetTelegraphType(_telegraphType);
        SetMarkerType(_markerType);

    }


}
