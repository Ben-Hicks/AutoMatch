using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphController : Singleton<TelegraphController> {

    public Ability abilDisplay;
    List<Telegraph.TeleTileInfo> lstHoverTeleInfo;

    public float fDelayEnemyTelegraph;
    public float fFadeoutTime;
    

    public IEnumerator AnimateEnemyTelegraph(Ability abilEnemy, Position posToTarget) {

        List<Telegraph.TeleTileInfo> lstTeleInfo = abilEnemy.TelegraphedTiles(posToTarget);

        //Initially telegraph all tiles as they are specified in the list of teletileinfo
        foreach(Telegraph.TeleTileInfo teleinfo in lstTeleInfo) {
            Board.Get().At(teleinfo.pos).telegraph.SetTelegraph(teleinfo);
        }

        float fStartTime = Time.timeSinceLevelLoad;

        //Keep displaying this ability until we've hit the desired display time
        while(Time.timeSinceLevelLoad - fStartTime > fDelayEnemyTelegraph) {
            yield return new WaitForSeconds(0.05f); 
        }

        //Now we should fadeout the telegraph over time
        float fFadeoutStartTime = Time.timeSinceLevelLoad;
        while (true) {

            float fElapsedTime = Time.timeSinceLevelLoad - fFadeoutStartTime;
            float fProgress = Mathf.Min(1f, fElapsedTime / fFadeoutTime);

            foreach (Telegraph.TeleTileInfo teleinfo in lstTeleInfo) {
                Board.Get().At(teleinfo.pos).telegraph.SetAlpha(1f - fProgress);
            }

            if (fProgress == 1f) break;
        }

        //Once the telegraph is done, clear the telegraphed info
        foreach (Telegraph.TeleTileInfo teleinfo in lstTeleInfo) {
            Board.Get().At(teleinfo.pos).telegraph.ClearTelegraph();
        }

    }

    public void ClearHoverTelegraph() {

        foreach(Telegraph.TeleTileInfo teleinfo in lstHoverTeleInfo) {
            Board.Get().At(teleinfo.pos).telegraph.ClearTelegraph();
        }

        lstHoverTeleInfo = null;
    }

    public void SetAbilityHoverTelegraph(Ability _abilDisplay, Position posToTarget) {
        ClearHoverTelegraph();

        abilDisplay = _abilDisplay;

        lstHoverTeleInfo = abilDisplay.TelegraphedTiles(posToTarget);

        foreach (Telegraph.TeleTileInfo teleinfo in lstHoverTeleInfo) {
            Board.Get().At(teleinfo.pos).telegraph.SetTelegraph(teleinfo);
        }
    }

    public override void Init() {
        
    }
}
