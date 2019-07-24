using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : Property {

    public const int NUMABILSLOTS = 6;
    public enum ABILSLOT { PASS, MOVEMENT, ABIL1, ABIL2, ABIL3, ABIL4 };
    public List<Ability> lstAbilities;

    public int nMaxHealth;
    public int nCurHealth;

    public PRIORITY collectionPriority;

    public AbilitySelector abilityselector;

    private Collection privcollection;
    public Collection collection {
        get {
            if (privcollection == null) privcollection = GetComponent<Collection>();
            return privcollection;
        }
    }

    public Entity() {
        InitStandardAbilities();
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
        lstAbilities[(int)ABILSLOT.PASS] = new AbilityPass(this);
    }
    

    public override void TakeHealing(int nAmount=1) {
        Debug.Assert(nAmount > 0);
        nCurHealth = Mathf.Min(nMaxHealth, nCurHealth + nAmount);
    }

    public override void TakeDamage(int nAmount=1) {
        Debug.Assert(nAmount > 0);
        nCurHealth = Mathf.Max(0, nCurHealth - nAmount);

        if(nCurHealth == 0) {
            OnNoHealth();
        }
    }

    public virtual void OnNoHealth() {
        bCanBeCollected = true;
    }

    public float GetAnimTime(float fProposedAnimTime) {
        if (Board.Get().InfoAt(tile.pos).bActive) {
            return fProposedAnimTime;
        } else {
            return 0.00001f;
        }
    }

    private void OnDestroy() {
        GameController.Get().UnregisterEntity(this);
    }

    public void Update() {
        tile.SetDebugText(nCurHealth.ToString() + "/" + nMaxHealth.ToString());
    }

}
