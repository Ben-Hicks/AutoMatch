using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position {

    public int i;
    public int j;

    public struct DirDist {
        public Direction.Dir dir;
        public int nDist;
    }

    public Position(int _i = 0, int _j = 0) {
        i = _i;
        j = _j;
    }

    public Position(Position _other) {
        i = _other.i;
        j = _other.j;
    }

    public static Position operator +(Position a, Position b) {
        return new Position(a.i + b.i, a.j + b.j);
    }

    public static Position operator -(Position a, Position b) {
        return new Position(a.i - b.i, a.j - b.j);
    }

    public bool IsEqual(Position other) {
        return i == other.i && j == other.j;
    }

    public override string ToString() {
        return "(" + i + "," + j + ")";
    }

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

    public Direction.Dir GetAdjacentDir(Position posEnd) {

        if (PosInDir(Direction.Dir.D).IsEqual(posEnd)) return Direction.Dir.D;
        if (PosInDir(Direction.Dir.DL).IsEqual(posEnd)) return Direction.Dir.DL;
        if (PosInDir(Direction.Dir.DR).IsEqual(posEnd)) return Direction.Dir.DR;
        if (PosInDir(Direction.Dir.U).IsEqual(posEnd)) return Direction.Dir.U;
        if (PosInDir(Direction.Dir.UL).IsEqual(posEnd)) return Direction.Dir.UL;
        if (PosInDir(Direction.Dir.UR).IsEqual(posEnd)) return Direction.Dir.UR;

        //if none of the above directions worked we'll just return null
        return Direction.Dir.NONE;

    }


    public int DirectDistFrom(Position end) {

        int dX = Mathf.Abs(end.i - i);
        int dY = Mathf.Abs(end.j - j);

        return dX + Mathf.Max(0, (dY - dX) / 2);

    }

    //We assume that there is a straight line to the other position
    public DirDist DirDistTo(Position posOther) {
        Direction.Dir dir = Direction.Dir.NONE;
        int nDist = 0;

        if (IsEqual(posOther)) {
            //Then do nothing since we've already initialized our direction and dist to be none/0
        } else if (i == posOther.i && j < posOther.j) {
            //If we're in the same column but above the other position
            dir = Direction.Dir.D;
            nDist = (posOther.j - j)/2;
        } else if (i == posOther.i) {
            //If we're in the same column, then we must be below the other position
            dir = Direction.Dir.U;
            nDist = (j - posOther.j)/2;
        } else {
            //Then we must be along some diagonal
            int nDistX = posOther.i - i;
            int nDistY = posOther.j - j;

            if (Mathf.Abs(nDistX) != Mathf.Abs(nDistY)) {
                //If there isn't a straight diagonal line, then we should 
                // just break and return None/0

            } else {

                nDist = Mathf.Abs(nDistX);

                if (nDistX > 0 && nDistY < 0) {
                    //Then we are below and to the left
                    dir = Direction.Dir.UR;
                } else if (nDistX > 0 && nDistY > 0) {
                    //Then we are above and to the left;
                    dir = Direction.Dir.DR;
                } else if (nDistX < 0 && nDistY < 0) {
                    //Then we are below and to the right;
                    dir = Direction.Dir.UL;
                } else if (nDistX < 0 && nDistY > 0) {
                    //Then we are above and to the right;
                    dir = Direction.Dir.DL;
                }
            }
        }

        return new DirDist() { dir = dir, nDist = nDist };
    }


}
