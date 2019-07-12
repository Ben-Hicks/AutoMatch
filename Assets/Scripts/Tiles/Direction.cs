using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Direction {

    public const int NUMDIRECTIONS = 6;
    public enum Dir {NONE, UR, U, UL, DL, D, DR };
    public static List<Dir> lstAllDirs = new List<Dir>(new[] { Dir.UR, Dir.U, Dir.UL, Dir.DL, Dir.D, Dir.DR });

    public static Dir Negate(Dir d) {
        switch (d) {
            case Dir.D:
                return Dir.U;

            case Dir.U:
                return Dir.D;

            case Dir.UL:
                return Dir.DR;

            case Dir.DR:
                return Dir.UL;

            case Dir.DL:
                return Dir.UR;

            case Dir.UR:
                return Dir.DL;

            case Dir.NONE:
                return Dir.NONE;

            default:
                Debug.LogError("Unrecognized direction");
                return Dir.DR;

        }
    }

    public static void Advance(ref Dir d) {
        if (d == Dir.DR) d = Dir.UR;
        else d++;
    }




}
