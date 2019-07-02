using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position {

    public int i;
    public int j;

    public Position PosInDir(Direction.Dir dir, int dist = 1) {
        switch (dir) {
            case Direction.Dir.D:
                return new Position() { i = i, j = j + 2 * dist };

            case Direction.Dir.DL:
                return new Position() { i = i - dist, j = j + dist };

            case Direction.Dir.DR:
                return new Position() { i = i + dist, j = j + dist };

            case Direction.Dir.U:
                return new Position() { i = i, j = j - 2 * dist};

            case Direction.Dir.UL:
                return new Position() { i = i - dist, j = j - dist };

            case Direction.Dir.UR:
                return new Position() { i = i + dist, j = j - dist };

            case Direction.Dir.NONE:
                return new Position() { i = i, j = j };

            default:
                Debug.LogError("Unrecognized Direction");
                return null;
        }
    }

}
