using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntHero : Entity {

    public override void OnCollect(Collection collection) {

    }

    public override void SetDefaultColour() {
        colour.SetCol(Colour.Col.WILD);
    }

    public override void InitSpecialAbilities() {

        lstAbilities[(int)ABILSLOT.ABIL1] = new AbilityIcicleWave(this);
        lstAbilities[(int)ABILSLOT.ABIL2] = new AbilityDash(this);
        lstAbilities[(int)ABILSLOT.ABIL3] = new AbilityStab(this);

    }

}
