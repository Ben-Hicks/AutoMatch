using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : Property {

    public const int NUMABILSLOTS = 6;
    public enum ABILSLOT { PASS, MOVEMENT, ABIL1, ABIL2, ABIL3, ABIL4 };
    public List<Ability> lstAbilities;

    public int nMaxHealth;
    public int nCurHealth;
    
    public AbilitySelector abilityselector;

    private Collection privcollection;
    public Collection collection {
        get {
            if (privcollection == null) privcollection = GetComponent<Collection>();
            return privcollection;
        }
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

    public override void FlagForDeletion() {
        //Do nothing - entities shouldn't be deleted
    }

    public override void OnCollect(Collection collection) {
        Debug.Log("We collected an entity and should therefore do nothing");
    }
}
