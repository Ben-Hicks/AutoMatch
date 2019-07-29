using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : Singleton<AbilityController> {

    public enum ABIL { PASS, DASH, GEYSER, ICICLEWAVE, MOVE,
        REJUVENATE, STAB, DISENGAGE
    };

    public List<Ability> lstAbil = new List<Ability>();

    public override void Init() {
        //Keep in alphabetical order to make ordering easy - pass can come first though as a default value
        lstAbil.Add(new AbilityPass());
        lstAbil.Add(new AbilityDash());
        lstAbil.Add(new AbilityGeyser());
        lstAbil.Add(new AbilityIcicleWave());
        lstAbil.Add(new AbilityMove());
        lstAbil.Add(new AbilityRejuvenate());
        lstAbil.Add(new AbilityStab());
        lstAbil.Add(new AbilityDisengage());
    }

    //Return the single instance of the ability that is requested through an enum interface
    public Ability AbilityInstance(ABIL abil) {
        return lstAbil[(int)abil];
    }
}
