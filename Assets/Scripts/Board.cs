using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : Singleton<Board> {

    public int nWidth;
    public int nHeight;

    public Position posPlayer;

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

    public bool ValidTile(Position pos) {
        return ValidTile(pos.i, pos.j);
    }
    public bool ValidTile(int i, int j) {
        return (i+j)%2 == 0 && i >= 0 && i < nWidth && j >= 0 && j < nHeight;
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

    public void InitPlayerTile() {
        Debug.Assert(nWidth % 2 == 1 && nHeight % 2 == 1);

        posPlayer = new Position() { i = (nWidth - 1) / 2, j = (nHeight - 1) / 2 };
        lstTiles[posPlayer.i][posPlayer.j].colour.SetColour(Colour.Col.WILD);

        //Todo - fill with stuff to set up the player
    }


    public void FixEdgeCascadeDirections() {

        Position posCur = posPlayer;
        //fix bot right ambiguous tiles
        while (true) {
            posCur = posCur.PosInDir(Direction.Dir.DR).PosInDir(Direction.Dir.D);
            if (ValidTile(posCur) == false) break;
            lstTiles[posCur.i][posCur.j].SetDirTowardCenter(Direction.Dir.U);
        }

        posCur = posPlayer;
        //fix top left ambiguous tiles
        while (true) {
            posCur = posCur.PosInDir(Direction.Dir.UL).PosInDir(Direction.Dir.U);
            if (ValidTile(posCur) == false) break;
            lstTiles[posCur.i][posCur.j].SetDirTowardCenter(Direction.Dir.D);
        }

        posCur = posPlayer;
        //fix left ambiguous tiles
        while (true) {
            posCur = posCur.PosInDir(Direction.Dir.UL).PosInDir(Direction.Dir.DL);
            if (ValidTile(posCur) == false) break;
            lstTiles[posCur.i][posCur.j].SetDirTowardCenter(Direction.Dir.DR);
        }

    }


    public void InitCascadeDirectionsRing(int dist) {
        Debug.Assert(dist > 0);

        Position posCur = posPlayer.PosInDir(Direction.Dir.D, dist);
        Direction.Dir dirCascadeCur = Direction.Dir.U;
        Direction.Dir curDir = Direction.Dir.UR;

        for (int iDir=0; iDir < Direction.NUMDIRECTIONS; iDir++) {
            for (int i=0; i<dist; i++) {

                if(dist%2 == 0 && i == dist / 2) {
                    //if we are halfway through an odd lengthed segment (if you include the following endpoint)
                    //then we can switch our cascade direction

                    Direction.Advance(ref dirCascadeCur);
                } else if(dist%2 == 1 && i == (dist+1)/2) {
                    //if we are halfway through an even lengthed segment (if you include the following endpoint)
                    //then we can switch advance our cascade direction

                    Direction.Advance(ref dirCascadeCur);
                }

                //Only actually set the tile if that tile exists
                if (ValidTile(posCur)) {
                    lstTiles[posCur.i][posCur.j].SetDirTowardCenter(dirCascadeCur);
                }

                posCur = posCur.PosInDir(curDir);

            }
            

            Direction.Advance(ref curDir);

            if(dist == 1) {
                Direction.Advance(ref dirCascadeCur);
            }

        }
        
    }


    public void InitCascadeDirections() {

        lstTiles[posPlayer.i][posPlayer.j].dirTowardCenter = Direction.Dir.NONE;

        for(int i=1; i<nHeight; i++) {
            InitCascadeDirectionsRing(i);
        }

        FixEdgeCascadeDirections();
    }

    public override void Init() {
        InitTiles();
        InitColours();

        InitPlayerTile();
        InitCascadeDirections();
    }

    // Update is called once per frame
    void Update() {

    }
}
