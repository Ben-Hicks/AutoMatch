using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphController : Singleton<TelegraphController> {

    public Ability abilDisplay;
    List<Telegraph.TeleTileInfo> lstHoverTeleInfo;

    public float fDelayEnemyTelegraph;
    public float fFadeoutTime;

    public float fTimeLastStableSnapshot;
    
    public void TelegraphList(List<Telegraph.TeleTileInfo> _lstTeleInfo) {
        foreach (Telegraph.TeleTileInfo teleinfo in _lstTeleInfo) {
            Board.Get().At(teleinfo.pos).telegraph.SetTelegraph(teleinfo);
        }
    }

    public void ClearList(List<Telegraph.TeleTileInfo> _lstTeleInfo) {
        foreach (Telegraph.TeleTileInfo teleinfo in _lstTeleInfo) {
            Board.Get().At(teleinfo.pos).telegraph.ClearTelegraph();
        }
    }

    public void ClearAllTelegraphs() {
        foreach (Tile tile in Board.Get().lstAllTiles) {
            tile.telegraph.ClearTelegraph();
        }
    }

    public IEnumerator AnimateEnemyTelegraph(SelectorEnemy.Intended intended) {

        float fLocalTimeLastStableSnapshot = fTimeLastStableSnapshot;
        List<Telegraph.TeleTileInfo> lstTeleInfo = intended.abil.TelegraphedTiles(intended.GetIntended().pos);

        //Initially telegraph all tiles as they are specified in the list of teletileinfo
        TelegraphList(lstTeleInfo);

        float fStartTime = Time.timeSinceLevelLoad;

        //Keep displaying this ability until we've hit the desired display time
        while(Time.timeSinceLevelLoad - fStartTime < fDelayEnemyTelegraph) {

            //If there's been an update to the board recently, then clear out any previous telegraphs, update our lstTeleInfo, and re-telegraph
            if(fLocalTimeLastStableSnapshot != fTimeLastStableSnapshot) {
                fLocalTimeLastStableSnapshot = fTimeLastStableSnapshot;
                
                //Recalculate lstteleinfo
                lstTeleInfo = intended.abil.TelegraphedTiles(intended.GetIntended().pos);

                //Then reapply our telegraph to the new applicable tiles
                TelegraphList(lstTeleInfo);
            }

            yield return null;
        }

        //Now we should fadeout the telegraph over time
        float fFadeoutStartTime = Time.timeSinceLevelLoad;
        while (true) {

            float fElapsedTime = Time.timeSinceLevelLoad - fFadeoutStartTime;
            float fProgress = Mathf.Min(1f, fElapsedTime / fFadeoutTime);


            //If there's been an update to the board recently, then clear out any previous telegraphs, update our lstTeleInfo, and re-telegraph
            if (fLocalTimeLastStableSnapshot != fTimeLastStableSnapshot) {
                fLocalTimeLastStableSnapshot = fTimeLastStableSnapshot;

                //Recalculate lstteleinfo
                lstTeleInfo = intended.abil.TelegraphedTiles(intended.GetIntended().pos);

                //Then reapply our telegraph to the new applicable tiles
                TelegraphList(lstTeleInfo);
            }

            foreach (Telegraph.TeleTileInfo teleinfo in lstTeleInfo) {
                Board.Get().At(teleinfo.pos).telegraph.SetAlpha(1f - fProgress);
            }

            if (fProgress == 1f) break;
            yield return null;
        }

        //Once the telegraph is done, clear the telegraphed info
        ClearList(lstTeleInfo);

    }

    public void ClearHoverTelegraph() {

        ClearList(lstHoverTeleInfo);

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

    public void cbOnBoardChange(Object target, params object[] args) {
        //If the board changed at all, we can update our snapshot of the last time the board was stable
        fTimeLastStableSnapshot = Time.timeSinceLevelLoad;

        ClearAllTelegraphs();
    }

    public override void Init() {

        Board.Get().subBoardChanged.Subscribe(cbOnBoardChange);

    }
}
