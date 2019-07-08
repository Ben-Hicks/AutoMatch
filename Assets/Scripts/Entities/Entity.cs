using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public const int NUMABILSLOTS = 6;
    public enum ABILSLOT { PASS, MOVEMENT, ABIL1, ABIL2, ABIL3, ABIL4 };
    public List<Ability> lstAbilities;

    public int nMaxHealth;
    public int nCurHealth;

    public Tile tile;
    public AbilitySelector abilityselector;

    public void SetTile(Tile _tile) {
        Debug.Assert(_tile != null);

        tile = _tile;
    }

    public void SetAbilitySelector(AbilitySelector _abilityselector) {
        abilityselector = _abilityselector;
    }


    // Start is called before the first frame update
    void Start() {
        abilityselector = GetComponent<AbilitySelector>();
        GameController.Get().RegisterEntity(this);

        InitStandardAbilities();
    }

    public void InitStandardAbilities() {
        lstAbilities = new List<Ability>(NUMABILSLOTS);
        for (int i = 0; i < NUMABILSLOTS; i++) lstAbilities.Add(null);
        
        lstAbilities[(int)ABILSLOT.MOVEMENT] = new AbilityMove(this);
    }

}
