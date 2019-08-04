using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : MonoBehaviour {

    public Colour.Col col;
    public GameObject goManaImage;

    public float fMinManaY;
    public float fMaxManaY;

    public float fBobAmount;
    public float fBobPeriod;

    public float fAdjustTime;

    public int nCurMana;
    public int nMaxMana;//Move this somewhere else to the collections maybe

    public Vector2 v2StartPos;
    public Vector2 v2CurPos;
    public Vector2 v2TargetPos;

    public void UpdateMana() {
        //Insert mana amount fetching here
        nCurMana = nCurMana;

        v2StartPos = goManaImage.transform.position;
        v2TargetPos = new Vector2(0f, Mathf.Lerp(fMinManaY, fMaxManaY, nCurMana / nMaxMana));
    }

    public IEnumerator CalculateManaLevel() {

        while (true) {
            //If we're already reflecting the right position for the mana, 
            //  then just sleep and wait for a new target position
            if (v2CurPos == v2TargetPos) {
                yield return new WaitForSeconds(0.05f);
            } else {
                //Otherwise, we should move our current position to our target
                float fStartTime = Time.timeSinceLevelLoad;

                float fProgress = 0f;
                while (fProgress < 1f) {

                    v2TargetPos = Library.LerpSmoothStep(v2StartPos, v2TargetPos, fProgress);

                    yield return new WaitForSeconds(0.05f);
                    fProgress = Mathf.Min(1, (Time.timeSinceLevelLoad - fStartTime) / fAdjustTime);
                }
            }
        }
    }

    public IEnumerator ShowManaLevel() {
        while (true) {
            float fCurBob = fBobAmount * Mathf.Sin(Time.timeSinceLevelLoad / fBobPeriod);

            goManaImage.transform.position = new Vector3(v2CurPos.x,
                v2CurPos.y + fCurBob, 0f);

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void Start() {
        StartCoroutine(CalculateManaLevel());
        StartCoroutine(ShowManaLevel());
    }



}
