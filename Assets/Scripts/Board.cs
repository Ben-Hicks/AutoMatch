using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : Singleton<Board> {

    public int nWidth;
    public int nHeight;

    public Position posCenter;
    public Position posPlayer;

    public GameObject pfTile;
    

    public List<List<Tile>> lstTiles;

    public Tile At(Position pos) {
        return lstTiles[pos.i][pos.j];
    }

    public int GetMatchingLength(Position posStart, Direction.Dir dir) {

        Position posCur = posStart.PosInDir(dir);
        int nMatchLength = 1;

        //As long as the next tile exists
        while (ValidTile(posCur)) {
            
            //Ensure that every currently matched tile also matches with the new current tile
            for(int i=0; i< nMatchLength; i++) {
                if(At(posCur).colour.CanMatch(At(posStart.PosInDir(dir, i)).colour) == false) {
                    //If one of our previously matched tiles doesn't match with this new tile, then we have reached the end of this chain
                    return nMatchLength;
                }
            }
            //If we've cycled through all of the tiles before the current one, and they all matched, 
            // then we know this new tile extends our match length by 1

            //Move to check the next tile now
            posCur = posCur.PosInDir(dir);
            nMatchLength++;
        }

        //If we ever reach this point, then we've found a match that extends into the side of the board
        // we can just return whatever length we reached up til this point
        return nMatchLength;
    }


    // i represents columns
    // j represents rows in those columns
    public void InitTiles() {
        lstTiles = new List<List<Tile>>(nWidth);
        posCenter = new Position() { i = (nWidth - 1) / 2, j = (nHeight - 1) / 2 };

        for (int i=0; i<nWidth; i++) {

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
                    //if this is a valid tile then we can fill it

                    //Initially, we can pick a random colour
                    Colour.Col colRand = (Colour.Col)Random.Range(1, Colour.NUMCOLOURS);
                    lstTiles[i][j].colour.SetColour(colRand);

                    for (int iColourAttempts=0; iColourAttempts < 4; iColourAttempts++) {
                        

                        //but we need to check the matches to the left and above to ensure that we're not picking a colour that would form a match of three

                        if (GetMatchingLength(new Position(i, j), Direction.Dir.U) >= 3) {
                            Debug.Log("Would have had a match of three above, so skipping");
                        } else if (GetMatchingLength(new Position(i, j), Direction.Dir.UL) >= 3) {
                            Debug.Log("Would have had a match of three to the left and above, so skipping");
                        } else if (GetMatchingLength(new Position(i, j), Direction.Dir.DL) >= 3) {
                            Debug.Log("Would have had a match of three to the left and below, so skipping");
                        } else {
                            Debug.Log("Breaking after " + iColourAttempts + " attempts");
                            break;
                        }
                        //If we haven't broken, then we should advance our colour in the hopes the next colour won't have a match
                        lstTiles[i][j].colour.SetNextColour();
                    }
                }

            }
        }

    }

    public void InitPlayerTile() {
        Debug.Assert(nWidth % 2 == 1 && nHeight % 2 == 1);

        posPlayer = posCenter;
        At(posPlayer).colour.SetColour(Colour.Col.WILD);

        //Todo - fill with stuff to set up the player
    }


    public void FixEdgeCascadeDirections() {

        Position posCur = posCenter;

        At(posCenter).SetDirTowardCenter(Direction.Dir.D);

        //fix bot right ambiguous tiles
        while (true) {
            posCur = posCur.PosInDir(Direction.Dir.DR).PosInDir(Direction.Dir.D);
            if (ValidTile(posCur) == false) break;
            At(posCur).SetDirTowardCenter(Direction.Dir.U);
        }

        posCur = posCenter;
        //fix top left ambiguous tiles
        while (true) {
            posCur = posCur.PosInDir(Direction.Dir.UL).PosInDir(Direction.Dir.U);
            if (ValidTile(posCur) == false) break;
            At(posCur).SetDirTowardCenter(Direction.Dir.D);
        }

        posCur = posCenter;
        //fix left ambiguous tiles
        while (true) {
            posCur = posCur.PosInDir(Direction.Dir.UL).PosInDir(Direction.Dir.DL);
            if (ValidTile(posCur) == false) break;
            At(posCur).SetDirTowardCenter(Direction.Dir.DR);
        }

    }


    public void InitCascadeDirectionsRing(int dist) {
        Debug.Assert(dist > 0);

        Position posCur = posCenter.PosInDir(Direction.Dir.D, dist);
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
                    At(posCur).SetDirTowardCenter(dirCascadeCur);
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

        At(posCenter).dirTowardCenter = Direction.Dir.NONE;

        for(int i=1; i<nHeight; i++) {
            InitCascadeDirectionsRing(i);
        }

        FixEdgeCascadeDirections();
    }

    public override void Init() {
        InitTiles();
        InitColours();

       //InitPlayerTile();
        InitCascadeDirections();
    }

    // Update is called once per frame
    void Update() {

    }
}
