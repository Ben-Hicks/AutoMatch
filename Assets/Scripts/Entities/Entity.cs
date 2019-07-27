using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : Property {

    public const int NUMABILSLOTS = 6;
    public enum ABILSLOT { PASS, MOVEMENT, ABIL1, ABIL2, ABIL3, ABIL4 };

    [System.Serializable]
    public struct AbilUsing {
        public AbilityController.ABIL abil;
        public int nPriority;
    }

    public AbilUsing[] arAbilUsed = new AbilUsing[NUMABILSLOTS];
    public Ability[] arAbilities = new Ability[NUMABILSLOTS];
    public int[] arPriorities = new int[NUMABILSLOTS];

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
        UpdateAllAbilities();
    }

    public void InitStandardAbilities() {
        arAbilUsed[(int)ABILSLOT.MOVEMENT] = new AbilUsing { abil = AbilityController.ABIL.MOVE, nPriority = 0 };
        arAbilUsed[(int)ABILSLOT.PASS] = new AbilUsing { abil = AbilityController.ABIL.PASS, nPriority = 0 };
    }
    
    public void UpdateAbility(ABILSLOT abilslot) {
        //Using the selection information in arAbilUsed, fetch the instance of that type from AbilityController
        arAbilities[(int)abilslot] = AbilityController.Get().AbilityInstance(arAbilUsed[(int)abilslot].abil);
        arPriorities[(int)abilslot] = arAbilUsed[(int)abilslot].nPriority;
    }

    public void UpdateAllAbilities() {
        for(int i=0; i<NUMABILSLOTS; i++) {
            UpdateAbility((ABILSLOT)i);
        }
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

    public override void OnCollect(Collection collection) {
        //TODO:: currently don't have any collection stuff done
    }

    private void OnDestroy() {
        GameController.Get().UnregisterEntity(this);
    }

    public void Update() {
        tile.SetDebugText(nCurHealth.ToString() + "/" + nMaxHealth.ToString());
    }

}
