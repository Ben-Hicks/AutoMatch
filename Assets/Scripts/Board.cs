using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : Singleton<Board> {

    public int nWidth;
    public int nHeight;

    public GameObject pfTile;
    

    public List<List<Tile>> lstTiles;


    // i represents columns
    // j represents rows in those columns
    public void InitTiles() {
        lstTiles = new List<List<Tile>>(nWidth);

        for(int i=0; i<nWidth; i++) {

            lstTiles.Add(new List<Tile>(nHeight));

            for(int j=0; j<nHeight; j++) {

                //only placing tiles in locations where the i and j coords have the same parity
                if((i+j)%2 == 0) {
                    GameObject goTile = Instantiate(pfTile, this.transform);
                    lstTiles[i].Add(goTile.GetComponent<Tile>());
                    lstTiles[i][j].Init(i, j);
                } else {
                    //No tile should exist at this i,j so we'll just put a dummy null value
                    lstTiles[i].Add(null);
                }

            }

        }
    }

    public void InitColours() {
    
        for(int i=0; i<nWidth; i++) {
            for(int j=0; j<nHeight; j++) {
                
                if((i+j)%2 == 0) {
                    //if this is a valid tile
                    lstTiles[i][j].colour.SetColour((Colour.Col)Random.Range(1, Colour.NUMCOLOURS));
                }

            }
        }

    }

    public override void Init() {
        InitTiles();
        InitColours();
    }

    // Update is called once per frame
    void Update() {

    }
}
