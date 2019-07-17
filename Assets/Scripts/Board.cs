using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : Singleton<Board> {

    public const int MINMATCHLENGTH = 3;

    public float fDelayBeforeCascade;
    public float fMatchAnimTime;
    public float fTileSwellTime;
    public float fTileSwellSize;

    public int nWidth;
    public int nHeight;

    public int nActiveWidth;
    public int nActiveHeight;

    public float fStandardAnimTime;
    public float fCascadeTime;
    public float fBoardScrollTime;

    public Position posCenter;
    public Tile tilePlayer;

    public GameObject pfTile;

    public bool bStartWithMatches;

    public List<List<Tile>> lstTiles;

    public List<Tile> lstAllTiles;
    public List<Tile> lstActiveTiles;
    public List<Position> lstTopTwoEdges;
    public List<Position> lstTopLeftEdges;
    public List<Position> lstTopRightEdges;
    public List<Position> lstBottomTwoEdges;
    public List<Position> lstBottomLeftEdges;
    public List<Position> lstBottomRightEdges;

    public HashSet<Tile> setFlaggedToClear;

    public Board() {
        lstTopLeftEdges = new List<Position>();
        lstTopTwoEdges = new List<Position>();
        lstTopRightEdges = new List<Position>();
        lstBottomRightEdges = new List<Position>();
        lstBottomLeftEdges = new List<Position>();
        lstBottomTwoEdges = new List<Position>();
    }

    public Tile At(Position pos) {
        return lstTiles[pos.i][pos.j];
    }


    public void CollectFlaggedTiles() {
        Debug.Log("Also consider if collections should be set up as coroutines so they can do animations");
        foreach (Tile tile in setFlaggedToClear) {

            if(tile.toCollectBy == null) {
                //If no one is set to collect this tile, then let the player collect it

                Debug.Log("Currently not letting players collect faraway tiles");
                return;
                tile.toCollectBy = GameController.Get().entHero.collection;
            } 

            tile.prop.OnCollect(tile.toCollectBy);

            tile.toCollectBy = null;
        }
        
    }


    public void GetPathDistsTo(Tile tileStart, ref Dictionary<Tile, int> dictDists) {

        int nInfinity = 999999999;

        foreach (Tile tile in lstAllTiles) {
            //initially say all tiles are extremely far away
            dictDists[tile] = nInfinity;
        }

        int nCurDist = 0;
        dictDists[tileStart] = 0;

        Queue<Tile> queueNextToProcess = new Queue<Tile>();
        //Initially we'll add the starting tile, as well as a special null tile to count when we've exhausted
        // finding tiles at the same distance
        queueNextToProcess.Enqueue(tileStart);
        queueNextToProcess.Enqueue(null);

        while (true) {

            Tile tileCur = queueNextToProcess.Dequeue();

            //Check if we've reached the end of elements all with distance, nCurDist
            if(tileCur == null) {
                //If we have no elements left to explore, then we can end
                if (queueNextToProcess.Count == 0) break;

                //Increase the distance and re-add the marker to mark the end of tiles at this distance
                nCurDist++;
                queueNextToProcess.Enqueue(null);
            } else {

                //If we have a tile as the next element we're exploring, then we can get all of its adjacent tiles
                List<Tile> lstAdj = GetAdjacentTiles(tileCur);

                foreach(Tile tile in lstAdj) {
                    //If we can't move onto this tile, then move on to the next one
                    if (tile.prop.bBlocksMovement) continue;

                    //If this tile was already explored, then move on to the next one
                    if (dictDists[tile] != nInfinity) continue;

                    //If we can move to this tile, and it wasn't explored already, then this
                    // must be our first time seeing it, so add it to the list of items to explore next
                    queueNextToProcess.Enqueue(tile);
                    //And we can record its length to ensure we don't try to visit it multiple times
                    dictDists[tile] = nCurDist + 1;
                }
            }
        }
    }

    public void UpdatePathDistsFromPlayer() {
        //Use the breadthfirst search algorithm to examine each tile's path-distance from the player's tile
        Dictionary<Tile, int> dictDistsToPlayer = new Dictionary<Tile, int>();
        GetPathDistsTo(GameController.Get().entHero.tile, ref dictDistsToPlayer);

        foreach (Tile tile in lstAllTiles) {
            tile.UpdatePathDistToPlayer(dictDistsToPlayer[tile]);
        }
    }

    public void UpdateDistsFromPlayer() {

        foreach(Tile tile in lstAllTiles) {
            tile.UpdateDirectDistToPlayer();
        }

        UpdatePathDistsFromPlayer();

    }

    public IEnumerator CleanupMatches() {

        CascadeAllTiles();

        CollectFlaggedTiles();

        yield return AnimateMatchedTiles();

        GenerateColoursForDeleted();
        SetAllStablePosForDeleted();

        

        yield return AnimateMovingTiles(fCascadeTime);

        //Once everything's cleaned up, we can clear out our list of flagged tiles
        setFlaggedToClear.Clear();
    }

    public void CascadeTile(Tile tile) {

        while (true) {
            Position posNext = tile.pos.PosInDir(tile.dirCascadeFrom);

            //If we've reached the edge of the board, we can stop swapping
            if (ValidTile(posNext) == false) {
                //Debug.Log("Stopping since " + posNext.i + "," + posNext.j + " isn't in the board");
                break;
            } else if (At(posNext).deletionStatus == Tile.DELETIONSTATUS.DELETED) {

                //Debug.Log("Stopping since " + posNext.i + "," + posNext.j + " can't be swapped into since it's deleted");
                break;
            } else {
                MoveTile(tile, tile.dirCascadeFrom);
            }

        }


        tile.deletionStatus = Tile.DELETIONSTATUS.DELETED;

    }

    public void CascadeAllTiles() {

        foreach (Tile tileToCascade in setFlaggedToClear) {
            //If we already set this tile to be deleted while dealing with another tile in the same cascade, then we don't need to look at it again
            if (tileToCascade.deletionStatus == Tile.DELETIONSTATUS.DELETED) continue;
            CascadeTile(tileToCascade);
        }

    }


    public void FlagMatchesInDir(Position posStart, Direction.Dir dir) {

        Position curPos = posStart;

        while (ValidTile(curPos)) {
            
            if (At(curPos).bActive == false) {
                //If the current tile is not active, then just skip it and look later
            } else { 

                //Check the MatchingLength of the current tile
                int nMatchLength = GetMatchingLength(curPos, dir);

                //If the match is long enough
                if (nMatchLength >= MINMATCHLENGTH) {

                    Entity entHighestCollectionPriority = null;
                    //first we'll find the highest collection priority entity in the match
                    for (int i = 0; i < nMatchLength; i++) {

                        //Attempt to get an entity component from the current tile 
                        //TODO:: Optimize this since repeated getcomponents is likely slow
                        Entity ent = At(curPos.PosInDir(dir, i)).prop.GetComponent<Entity>();

                        //if either we haven't seen a collector yet, or this new collector is faster
                        if (ent != null && (entHighestCollectionPriority == null || ent.collectionPriority > entHighestCollectionPriority.collectionPriority)) {
                            entHighestCollectionPriority = ent;
                        }
                    }

                    //Then flag each tile along this match, and update their collector reference if its better than their current one
                    for (int i = 0; i < nMatchLength; i++) {
                        Tile tile = At(curPos.PosInDir(dir, i));

                        tile.prop.FlagForDeletion();

                        if (entHighestCollectionPriority != null && (tile.toCollectBy == null || entHighestCollectionPriority.collectionPriority > tile.toCollectBy.entity.collectionPriority)) {
                            tile.toCollectBy = entHighestCollectionPriority.collection;
                        }


                    }
                }

            }

            //Advance to the next tile and check for matches there
            // NOTE - we can't jump ahead by the matching length of this tile since it could involve wild matches that would be needed
            //        for future overlapping matches
            curPos = curPos.PosInDir(dir);

        }
    }


    public int FlagMatches() {

        setFlaggedToClear = new HashSet<Tile>();

        foreach (Position posStart in lstTopTwoEdges) {
            FlagMatchesInDir(posStart, Direction.Dir.D);
        }

        foreach (Position posStart in lstTopLeftEdges) {
            FlagMatchesInDir(posStart, Direction.Dir.DR);
        }

        foreach (Position posStart in lstTopRightEdges) {
            FlagMatchesInDir(posStart, Direction.Dir.DL);
        }

        return setFlaggedToClear.Count;

    }

    public void MoveTile(Tile tile, Direction.Dir dir, int nDist = 1) {
        if (dir == Direction.Dir.NONE) return; //Since no moving needs to be done

        Position posCur = tile.pos;
        Position posDestination = tile.pos.PosInDir(dir, nDist);

        //Save a reference for the direction of the destination tile so that we can restore it later
        Direction.Dir dirCenterDestination = At(posDestination).dirTowardCenter;
        bool bActiveDestination = At(posDestination).bActive;

        if (ValidTile(posDestination) == false) {
            Debug.LogError("Can't move to an invalid position " + posDestination.ToString());
            return;
        }

        //Start at the tile's position and fetch the tiles ahead one at a time
        while(posCur.IsEqual(posDestination) == false) {
            //Debug.Log("Want to shift " + posCur.PosInDir(dir).ToString() + " to position " + posCur.ToString());

            
            //Copy the reference of the tile above us
            Direction.Dir dirCenter = At(posCur).dirTowardCenter;
            bool bActive = At(posCur).bActive;

            lstTiles[posCur.i][posCur.j] = At(posCur.PosInDir(dir));
            At(posCur).SetPositon(posCur);
            //Make sure we use the cascade direction for the previous tile in this location
            At(posCur).SetDirTowardCenter(dirCenter);
            At(posCur).SetActive(bActive);

            //Advance our tile up
            posCur = posCur.PosInDir(dir);

        }

        //Once everything has been pulled down in sequence and we've reached our destination position
        // we can set the destination position's tile to be our original tile
        
        lstTiles[posDestination.i][posDestination.j] = tile;
        At(posDestination).SetPositon(posDestination);

        //Restore the direction of the destination tile that we saved at the beginning
        At(posDestination).SetDirTowardCenter(dirCenterDestination);
        At(posDestination).SetActive(bActiveDestination);

    }

    public void SwapTiles(Position pos, Position other) {

        if(ValidTile(pos) == false || ValidTile(other) == false) {
            Debug.Log("Can't swap with an invalid position: " + pos.ToString() + " and " + pos.ToString());
        }

        //Save a reference to the tile we're swapping
        Tile tile = At(pos);

        //Swap the passed tile and the target tile as they are stored in the grid of tiles
        lstTiles[pos.i][pos.j] = At(other);
        lstTiles[other.i][other.j] = tile;

        //We need to fix up the swapped tiles to ensure their positions accurately represent their new position
        At(pos).SetPositon(pos);
        At(other).SetPositon(other);

        //We also need to make sure that the tiles have the proper cascading directions
        Direction.Dir dirCascadeSwap = At(pos).dirTowardCenter;
        At(pos).SetDirTowardCenter(At(other).dirTowardCenter);
        At(other).SetDirTowardCenter(dirCascadeSwap);

        bool bActiveSwap = At(pos).bActive;
        At(pos).SetActive(At(other).bActive);
        At(other).SetActive(bActiveSwap);
    }

    public List<Tile> GetAdjacentTiles(Tile tile) {

        List<Tile> lstAdj = new List<Tile>();

        for(Direction.Dir dir = 0; (int)dir <= Direction.NUMDIRECTIONS; dir++) {
            if (dir == Direction.Dir.NONE) continue;

            if (ValidTile(tile.pos.PosInDir(dir))) {
                lstAdj.Add(At(tile.pos.PosInDir(dir)));
            }
        }

        return lstAdj;
    }


    public int GetMatchingLength(Position posStart, Direction.Dir dir) {

        Position posCur = posStart.PosInDir(dir);
        int nMatchLength = 1;

        //As long as the next tile exists and is active
        while (ActiveTile(posCur)) {

            //Ensure that every currently matched tile also matches with the new current tile
            for (int i = 0; i < nMatchLength; i++) {
                if (At(posCur).colour.CanMatch(At(posStart.PosInDir(dir, i)).colour) == false) {
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

        for (int i = 0; i < nWidth; i++) {

            lstTiles.Add(new List<Tile>(nHeight));

            for (int j = 0; j < nHeight; j++) {

                //only placing tiles in locations where the i and j coords have the same parity
                if ((i + j) % 2 == 0) {
                    GameObject goTile = Instantiate(pfTile, this.transform);
                    lstTiles[i].Add(goTile.GetComponent<Tile>());
                    lstTiles[i][j].Init(i, j);

                    lstAllTiles.Add(lstTiles[i][j]);

                    //Determine if this tile is part of an edge
                    
                    //Top Edge
                    if (j == 0 || j == 1) {
                        lstTopTwoEdges.Add(lstTiles[i][j].pos);
                    }

                    //Top/Left Edges
                    if (j == 0 || i == 0) {
                        lstTopLeftEdges.Add(lstTiles[i][j].pos);
                    }

                    //Top/Right Edges
                    if (j == 0 || i == nWidth - 1) {
                        lstTopRightEdges.Add(lstTiles[i][j].pos);
                    }
                    
                    //Bot Edge
                    if (j == nHeight - 1 || j == nHeight - 2) {
                        lstBottomTwoEdges.Add(lstTiles[i][j].pos);
                    }

                    //Bot/Left Edges
                    if (j == nHeight - 1 || i == 0) {
                        lstBottomLeftEdges.Add(lstTiles[i][j].pos);
                    }

                    //Bot/Right Edges
                    if (j == nHeight - 1 || i == nWidth - 1) {
                        lstBottomRightEdges.Add(lstTiles[i][j].pos);
                    }


                } else {
                    //No tile should exist at this i,j so we'll just put a dummy null value
                    lstTiles[i].Add(null);
                }

            }

        }
    }

    public void InitMarkActiveTiles() {

        foreach (Tile tile in lstAllTiles) {
            if(Mathf.Abs(posCenter.i - tile.pos.i) <= nActiveWidth && Mathf.Abs(posCenter.j - tile.pos.j) <= nActiveHeight) {
                tile.SetActive(true);
                lstActiveTiles.Add(tile);
            } else {
                tile.SetActive(false);
            }
        }
    }

    public bool ActiveTile(Position pos) {
        return ValidTile(pos) && At(pos).bActive;
    }

    public bool ValidTile(Position pos) {
        return ValidTile(pos.i, pos.j);
    }
    public bool ValidTile(int i, int j) {
        return (i + j) % 2 == 0 && i >= 0 && i < nWidth && j >= 0 && j < nHeight;
    }

    public void InitColours() {

        for (int i = 0; i < nWidth; i++) {
            for (int j = 0; j < nHeight; j++) {

                if ((i + j) % 2 == 0) {
                    //if this is a valid tile then we can fill it
                    
                    PropertyController.Get().SpawnNewProperty(lstTiles[i][j], Direction.Dir.NONE);

                    if (bStartWithMatches == false) {
                        //if we don't want to start with matches, then we'll have to find a colour that won't form a match
                        for (int iColourAttempts = 0; iColourAttempts < 4; iColourAttempts++) {


                            //but we need to check the matches to the left and above to ensure that we're not picking a colour that would form a match of three

                            if (GetMatchingLength(new Position(i, j), Direction.Dir.U) >= 3) {
                                //Debug.Log("Would have had a match of three above, so skipping");
                            } else if (GetMatchingLength(new Position(i, j), Direction.Dir.UL) >= 3) {
                                //Debug.Log("Would have had a match of three to the left and above, so skipping");
                            } else if (GetMatchingLength(new Position(i, j), Direction.Dir.DL) >= 3) {
                                //Debug.Log("Would have had a match of three to the left and below, so skipping");
                            } else {
                                //Debug.Log("Breaking after " + iColourAttempts + " attempts");
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

    
    //TODO - Move this somewhere else
    public void InitPlayerTile() {
        Debug.Assert(nWidth % 2 == 1 && nHeight % 2 == 1);

        tilePlayer = At(posCenter);

        PropertyController.Get().PlaceProperty("Hero", tilePlayer);
        
        GameController.Get().entHero = tilePlayer.prop.GetComponent<EntHero>();

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

        for (int iDir = 0; iDir < Direction.NUMDIRECTIONS; iDir++) {
            for (int i = 0; i < dist; i++) {

                if (dist % 2 == 0 && i == dist / 2) {
                    //if we are halfway through an odd lengthed segment (if you include the following endpoint)
                    //then we can switch our cascade direction

                    Direction.Advance(ref dirCascadeCur);
                } else if (dist % 2 == 1 && i == (dist + 1) / 2) {
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

            if (dist == 1) {
                Direction.Advance(ref dirCascadeCur);
            }

        }

    }


    public void InitCascadeDirections() {

        At(posCenter).dirTowardCenter = Direction.Dir.NONE;

        for (int i = 1; i < nHeight; i++) {
            InitCascadeDirectionsRing(i);
        }

        FixEdgeCascadeDirections();
    }

    public override void Init() {
        InitTiles();
        InitMarkActiveTiles();
        InitColours();

        InitPlayerTile();
        InitCascadeDirections();

        Debug.Log("Starting gameloop");
        StartCoroutine(GameController.Get().GameLoop());
    }



    public IEnumerator AnimateMatchedTiles() {
        

        float fTimeStart = Time.timeSinceLevelLoad;

        while (true) {

            float fElapsedTime = Time.timeSinceLevelLoad - fTimeStart;
            float fProgress = Mathf.Min(1f, fElapsedTime / fMatchAnimTime);

            Vector3 v3NewScale = Vector3.Lerp(new Vector3(0f, 0f, 0f), new Vector3(fTileSwellSize, fTileSwellSize, fTileSwellSize),
                0.5f * Mathf.Cos(Mathf.PI * (fProgress - 0.15f)) + 0.45f);

            foreach (Tile tile in setFlaggedToClear) {

                tile.transform.localScale = v3NewScale;
                
            }

            //If our progress is complete, we can stop moving
            if (fProgress == 1f) {
                break;
            } else {
                //If we're not complete, we should yield for a frame
                yield return null;
            }
        }
        
    }

    public void RealignPlayer() {
        
        //If the player is not in the center of the board
        if(tilePlayer.pos.IsEqual(posCenter) == false) {
            //Then figure out how far they've moved (assuming in a straight line)
            Position.DirDist dirDist = posCenter.DirDistTo(tilePlayer.pos);

            Debug.Assert(dirDist.dir != Direction.Dir.NONE && dirDist.nDist != 0);
            Debug.Log("Player moved " + dirDist.dir + " by " + dirDist.nDist);

            ShiftBoard(Direction.Negate(dirDist.dir), dirDist.nDist);
        }

    }

    public IEnumerator AnimateMovingTiles(float fDuration) {

        //Then figure out all of the tiles that have moved from their original position
        List<Tile> lstMovingTiles = new List<Tile>();
        foreach (Tile tile in lstAllTiles) {
            if(tile.pos.IsEqual(tile.posLastStable) == false) {
                //If the tile has moved since its last stable snapshot, then we'll need to animate it
                lstMovingTiles.Add(tile);
                tile.v2StartLocation = Tile.GetBoardLocation(tile.posLastStable);
                tile.v2GoalLocation = Tile.GetBoardLocation(tile.pos);
            }
        }

        float fTimeStart = Time.timeSinceLevelLoad;

        while (true) {
            //Now that we know which tiles need to move, let's figure out where their positions should be at this point in time
            float fElapsedTime = Time.timeSinceLevelLoad - fTimeStart;

            float fProgress = Mathf.Min(1f, fElapsedTime / fDuration);
            

            foreach (Tile tile in lstMovingTiles) {
                tile.transform.localPosition = Library.LerpSmoothStep(tile.v2StartLocation, tile.v2GoalLocation, fProgress);
            }

            //If our progress is complete, we can stop moving
            if (fProgress == 1f) {
                break;

            } else {
                //If we're not complete, we should yield for a frame 
                yield return null;
            }
        }

        //Once we've finished moving everything, we can update the stable positions for each tile we've moved
        foreach(Tile tile in lstMovingTiles) {
            tile.SaveStablePos();
        }
        
    }

    public void ShiftBoard(Direction.Dir dir, int nDist = 1) {

        Debug.Log("Shifting board in direction " + dir + " by + " + nDist);

        //if shifting board up, then get all of the tiles along the bottom border

        List<Position> lstStartingEdgePositions = null;

        switch (dir) {
            case Direction.Dir.D:
                lstStartingEdgePositions = lstBottomTwoEdges;
                break;

            case Direction.Dir.DL:
                lstStartingEdgePositions = lstBottomLeftEdges;
                break;

            case Direction.Dir.DR:
                lstStartingEdgePositions = lstBottomRightEdges;
                break;

            case Direction.Dir.U:
                lstStartingEdgePositions = lstTopTwoEdges;
                break;

            case Direction.Dir.UL:
                lstStartingEdgePositions = lstTopLeftEdges;
                break;

            case Direction.Dir.UR:
                lstStartingEdgePositions = lstTopRightEdges;
                break;

            case Direction.Dir.NONE:
                //Since we're not shifting at all, then just return at this point
                return;
        }

        //Now that we've got the starting edge for the direction we're going to be pulling tiles towards, let's start at this 
        // edge and then progress in the opposite direction, pulling tiles down to the current location as we go
        Direction.Dir dirPullFrom = Direction.Negate(dir);

        foreach (Position posInColumn in lstStartingEdgePositions) {

            Position posTarget = posInColumn;
            Position posPullFrom = posTarget.PosInDir(dirPullFrom, nDist);

            //As long as there is a tile at the position we want to pull from, keep pulling
            while (ValidTile(posPullFrom)) {

                MoveTile(At(posPullFrom), dir, nDist);
                posTarget = posTarget.PosInDir(dirPullFrom);
                posPullFrom = posPullFrom.PosInDir(dirPullFrom);
            }

            //Once we've pulled all the tiles down, then we can generate new tiles for the tiles
            // from posTarget til the edge of the board
            while(ValidTile(posTarget)) {
                //Ask the property controller to give us a new tile
                PropertyController.Get().SpawnNewProperty(At(posTarget), dir);

                //Set the stable pos to come from off screen at the appropriate distance
                At(posTarget).posLastStable = posTarget.PosInDir(dirPullFrom, nDist);

                posTarget = posTarget.PosInDir(dirPullFrom);
            }
        }



    }


    // Update is called once per frame
    public void Update() {
        
    }


    public void SetStablePosForDeleted(Tile tile) {

        Direction.Dir dirTowardCenter = tile.dirTowardCenter;
        Direction.Dir dirCascadeFrom = tile.dirCascadeFrom;

        int nDeletedDistTowardCenter = 1;

        Position posClosestToCenter = tile.pos;

        while (true) {
            Position posToCheck = tile.pos.PosInDir(dirTowardCenter, nDeletedDistTowardCenter);

            if (At(posToCheck).deletionStatus == Tile.DELETIONSTATUS.DELETED && At(posToCheck).dirTowardCenter == dirTowardCenter) {
                //If the next tile in this direction is also deleted, and is part of the same cascade direction as us

                posClosestToCenter = posToCheck;
                nDeletedDistTowardCenter++;
            } else {

                //Reduce by one since we couldn't actually extend in this direction
                nDeletedDistTowardCenter--;
                break;
            }
        }

        int nDeletedDistAwayFromCenter = 1;

        while (true) {
            Position posToCheck = tile.pos.PosInDir(dirCascadeFrom, nDeletedDistAwayFromCenter);

            if (ValidTile(posToCheck)){
                Debug.Assert(At(posToCheck).deletionStatus == Tile.DELETIONSTATUS.DELETED);
                //If the next outward tile exists, and is also deleted
                nDeletedDistAwayFromCenter++;

            } else {
                //If we've gone as far in this dirction as possible (until the edge of the screen)
                nDeletedDistAwayFromCenter--;
                break;
            } 
        }

        int nDeletedDist = 1 + nDeletedDistTowardCenter + nDeletedDistAwayFromCenter;

        //Now that we've found the closest tile to the center, and the length of the same-direction deletions, we can let each tile
        // in the match know how long its deletion sequence is, and set it's stable pos appropriately
        for (int i = 0; i < nDeletedDist; i++) {
            At(posClosestToCenter.PosInDir(dirCascadeFrom, i)).posLastStable = posClosestToCenter.PosInDir(dirCascadeFrom, i + nDeletedDist);
            At(posClosestToCenter.PosInDir(dirCascadeFrom, i)).deletionStatus = Tile.DELETIONSTATUS.ACTIVE;
        }

    }


    public void GenerateColoursForDeleted() {

        foreach(Tile tile in setFlaggedToClear) {
            PropertyController.Get().SpawnNewProperty(tile, tile.dirCascadeFrom);
            tile.transform.localScale = new Vector3(1f, 1f, 1f);
        }

    }


    public void SetAllStablePosForDeleted() {

        //Look through each tile that we've flagged for clearing
        foreach (Tile tile in setFlaggedToClear) {
            //If we've already handled this position in another tile's run, then we don't need to handle it again
            if (At(tile.pos).deletionStatus == Tile.DELETIONSTATUS.ACTIVE) continue;

            SetStablePosForDeleted(tile);
        }

    }

}