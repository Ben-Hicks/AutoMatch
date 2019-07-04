using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : Singleton<Board> {

    public const int MINMATCHLENGTH = 3;

    public int nWidth;
    public int nHeight;

    public Position posCenter;
    public Position posPlayer;

    public GameObject pfTile;

    public bool bStartWithMatches;

    public List<List<Tile>> lstTiles;

    public List<Position> lstTopTwoEdges;
    public List<Position> lstTopLeftEdges;
    public List<Position> lstTopRightEdges;

    public List<Tile> lstFlaggedToClear;

    public Board() {
        lstTopLeftEdges = new List<Position>();
        lstTopTwoEdges = new List<Position>();
        lstTopRightEdges = new List<Position>();
    }

    public Tile At(Position pos) {
        return lstTiles[pos.i][pos.j];
    }

    public void CleanupMatches() {
        FlagMatches();
        CascadeAllTiles();
    }

    public void CascadeTile(Tile tile) {

        while (true) {
            Position posNext = tile.pos.PosInDir(tile.dirCascadeFrom);

            //If we've reached the edge of the board, we can stop swapping
            if (ValidTile(posNext) == false) {
                Debug.Log("Stopping since " + posNext.i + "," + posNext.j + " isn't in the board");
                break;
            } else if (At(posNext).deletionStatus == Tile.DELETIONSTATUS.DELETED) {

                Debug.Log("Stopping since " + posNext.i + "," + posNext.j + " can't be swapped into since it's deleted");
                break;
            } else if (At(posNext).deletionStatus == Tile.DELETIONSTATUS.FLAGGED) {

                Debug.Log("Yielding to " + posNext + " since it's flagged and it's further away from the center than we are");
                CascadeTile(At(posNext));
            } else {
                SwapTile(tile, tile.dirCascadeFrom);
            }

        }


        tile.deletionStatus = Tile.DELETIONSTATUS.DELETED;

    }

    public void CascadeAllTiles() {

        foreach(Tile tileToCascade in lstFlaggedToClear) {
            if (tileToCascade.deletionStatus == Tile.DELETIONSTATUS.DELETED) continue;
            CascadeTile(tileToCascade);
        }
        
        //Empty the list
        lstFlaggedToClear.Clear();
    }


    public void FlagMatchesInDir(Position posStart, Direction.Dir dir) {
        
        Position curPos = posStart;

        while (ValidTile(curPos)) {

            //Check the MatchingLength of the current tile
            int nMatchLength = GetMatchingLength(curPos, dir);

            //If the match is long enough
            if(nMatchLength >= MINMATCHLENGTH) {

                //Then flag each tile along this match
                for(int i=0; i < nMatchLength; i++) {


                    At(curPos.PosInDir(dir, i)).FlagClear();
                    lstFlaggedToClear.Add(At(curPos.PosInDir(dir, i)));
                }
            }

            //Advance past this current match
            curPos = curPos.PosInDir(dir, nMatchLength);

        }
    }


    public void FlagMatches() {

        lstFlaggedToClear = new List<Tile>();

        foreach (Position posStart in lstTopTwoEdges) {
            FlagMatchesInDir(posStart, Direction.Dir.D);
        }

        foreach (Position posStart in lstTopLeftEdges) {
            FlagMatchesInDir(posStart, Direction.Dir.DR);
        }

        foreach (Position posStart in lstTopRightEdges) {
            FlagMatchesInDir(posStart, Direction.Dir.DL);
        }

    }

    public void SwapTile(Tile tile, Direction.Dir dir, int nDist) {
        //Just perform nDist individual swaps
        for (int i=0; i<nDist; i++) {
            SwapTile(tile, dir);
        }
    }

    public void SwapTile(Tile tile, Direction.Dir dir) {

        Position thisPos = tile.pos;
        Position otherPos = tile.pos.PosInDir(dir);

        if(ValidTile(otherPos) == false) {
            Debug.LogError("Can't swap with an invalid tile");
        }

        //Swap the passed tile and the target tile as they are stored in the grid of tiles
        lstTiles[tile.pos.i][tile.pos.j] = At(otherPos);
        lstTiles[otherPos.i][otherPos.j] = tile;

        //We need to fix up the swapped tiles to ensure their positions accurately represent their new position
        At(thisPos).SetPositon(thisPos);
        At(otherPos).SetPositon(otherPos);

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

                    //Save references to the top and side edges
                    if(j == 0) {
                        lstTopLeftEdges.Add(lstTiles[i][j].pos);
                        lstTopRightEdges.Add(lstTiles[i][j].pos);
                        lstTopTwoEdges.Add(lstTiles[i][j].pos);
                    }else if(j == 1) {
                        lstTopTwoEdges.Add(lstTiles[i][j].pos);
                    }else if(i == 0) {
                        lstTopLeftEdges.Add(lstTiles[i][j].pos);
                    }else if(i == nWidth - 1) {
                        lstTopRightEdges.Add(lstTiles[i][j].pos);
                    }


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

                    if(bStartWithMatches == false) {
                        //if we don't want to start with matches, then we'll have to find a colour that won't form a match
                        for (int iColourAttempts = 0; iColourAttempts < 4; iColourAttempts++) {


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

        FlagMatches();
        //CleanupMatches();
    }



    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(KeyCode.O)) {
            Debug.Log("Swapping 0,0 down");
            Position posToSwap = new Position();
            SwapTile(At(posToSwap), Direction.Dir.D);

            Debug.Log("After swapping, " + posToSwap + " has stored position " + At(posToSwap).pos);
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            Debug.Log("Calling cascade tiles");
            CascadeAllTiles();
        }

    }
}




/*  Code graveyard
 * 
 * public void SetCascadeDist(Position pos) {

        Direction.Dir dirTowardCenter = At(pos).dirTowardCenter;
        Direction.Dir dirCascadeFrom = At(pos).dirCascadeFrom;
        
        int nCascadeTowardCenter = 1;

        Position posClosestToCenter = pos;

        while (true) {
            Position posToCheck = pos.PosInDir(dirTowardCenter, nCascadeTowardCenter);

            if(At(posToCheck).clearFlag.bClear == false || At(posToCheck).dirTowardCenter != dirTowardCenter) {
                //If we've gone as far in this dirction as possible (with cleared tiles and tiles that face the same way)

                //Reduce by one since we couldn't actually extend in this direction
                nCascadeTowardCenter--;
                break;
            } else {
                posClosestToCenter = posToCheck;
                nCascadeTowardCenter++;
            }
        }

        int nCascadeAwayFromCenter = 1;

        while (true) {
            Position posToCheck = pos.PosInDir(dirCascadeFrom, nCascadeAwayFromCenter);

            if (ValidTile(posToCheck) == false || At(posToCheck).clearFlag.bClear == false) {
                //If we've gone as far in this dirction as possible (with cleared tiles and tiles that actually exist
                nCascadeAwayFromCenter--;
                break;
            } else {
                nCascadeAwayFromCenter++;
            }
        }

        int nCascadeDist = 1 + nCascadeTowardCenter + nCascadeAwayFromCenter;

        //Now that we've found the closest tile to the center, and the length of the same-direction cascade, we can let each tile
        // in the match know how long its cascade is
        for(int i=0; i<nCascadeDist; i++) {
            At(posClosestToCenter.PosInDir(dirCascadeFrom, i)).clearFlag = new Tile.ClearFlag() { bClear = true, nCascadeDist = nCascadeDist };
            At(posClosestToCenter.PosInDir(dirCascadeFrom, i)).SetDebugText(nCascadeDist.ToString());
        }

    }


    public void SetAllCascadeDists() {

        //Look through each tile that we've flagged for clearing
        foreach (Position pos in lstFlaggedToClear) {
            //If we've already handled this position in another match's update, then we don't need to handle it again
            if (At(pos).clearFlag.bClear == true) continue;

            SetCascadeDist(pos);
        }

    }*/
