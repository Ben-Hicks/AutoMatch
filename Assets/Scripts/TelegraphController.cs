﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphController : Singleton<TelegraphController> {

    public Ability abilDisplay;
    List<Telegraph.TeleTileInfo> lstHoverTeleInfo;

    public float fDelayEnemyTelegraph;
    public float fFadeoutTime;
    
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
    

    public void ClearHoverTelegraph() {

        ClearList(lstHoverTeleInfo);

        lstHoverTeleInfo = null;
    }

    public void SetAbilityHoverTelegraph(Ability _abilDisplay, Entity owner, Position posToTarget) {
        ClearHoverTelegraph();

        abilDisplay = _abilDisplay;

        lstHoverTeleInfo = abilDisplay.TelegraphedTiles(owner, posToTarget);

        foreach (Telegraph.TeleTileInfo teleinfo in lstHoverTeleInfo) {
            Board.Get().At(teleinfo.pos).telegraph.SetTelegraph(teleinfo);
        }
    }

    public void TelegraphAllEnemies(List<SelectorEnemy> lstSelectorEnemy) {
        //Clear all previous telegraphs
        ClearAllTelegraphs();

        //Then set the telegraphs for all enemies' planned abilities
        foreach (SelectorEnemy selector in lstSelectorEnemy) {
            //Debug.Log("Asking for telegraph for " + selector + " that wants tile at " + selector.intended.GetIntended().pos.ToString());
            TelegraphList(selector.intended.abil.TelegraphedTiles(selector.owner, selector.intended.GetIntendedPos()));
        }
    }

    public override void Init() {

    }
}
