using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Library{
    


    public static float LerpSmoothIn(float a, float b, float fProgress) {
        return Mathf.Lerp(a, b, Mathf.Sin(fProgress * Mathf.PI * 0.5f));
    }

    public static float LerpSmoothOut(float a, float b, float fProgress) {
        return Mathf.Lerp(a, b, 1f - Mathf.Cos(fProgress * Mathf.PI * 0.5f));
    }

    public static float LerpSmoothStep(float a, float b, float fProgress) {
        return Mathf.Lerp(a, b, fProgress * fProgress * (3f - 2f * fProgress));
    }

    public static Vector2 LerpSmoothIn(Vector2 a, Vector2 b, float fProgress) {
        return Vector2.Lerp(a, b, Mathf.Sin(fProgress * Mathf.PI * 0.5f));
    }

    public static Vector2 LerpSmoothOut(Vector2 a, Vector2 b, float fProgress) {
        return Vector2.Lerp(a, b, 1f - Mathf.Cos(fProgress * Mathf.PI * 0.5f));
    }

    public static Vector2 LerpSmoothStep(Vector2 a, Vector2 b, float fProgress) {
        return Vector2.Lerp(a, b, fProgress * fProgress * (3f - 2f * fProgress));
    }
}
