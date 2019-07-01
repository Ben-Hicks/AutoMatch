using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Direction {

    public enum Dir {NONE, UL, U, UR, DL, D, DR };

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




}
