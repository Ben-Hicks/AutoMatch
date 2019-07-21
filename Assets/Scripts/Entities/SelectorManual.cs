using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorManual : AbilitySelector {

    public bool bUsedAbility;

    public override IEnumerator SelectAndUseAbility() {

        bUsedAbility = false;

        while (bUsedAbility == false) {

            //Should just select an ability first in this loop, then move to a loop where it gets the target
            if (Input.GetKeyUp(KeyCode.Alpha1)) {
                Debug.Log("Manually using ability 1");

                //Clear all enemy telegraphs so that it's clear where your abilities are telegraphing themselves
                TelegraphController.Get().ClearAllTelegraphs();

                yield return owner.lstAbilities[(int)Entity.ABILSLOT.ABIL1].AttemptManualUse();
            } else if (Input.GetKeyUp(KeyCode.Alpha2)) {
                Debug.Log("Manually using ability 2");

                //Clear all enemy telegraphs so that it's clear where your abilities are telegraphing themselves
                TelegraphController.Get().ClearAllTelegraphs();


                yield return owner.lstAbilities[(int)Entity.ABILSLOT.ABIL2].AttemptManualUse();
            } else if (Input.GetKeyUp(KeyCode.Alpha3)) {
                Debug.Log("Would enter ability selection 3 at this point");

                //Clear all enemy telegraphs so that it's clear where your abilities are telegraphing themselves
                TelegraphController.Get().ClearAllTelegraphs();


                bUsedAbility = true;
                //yield return owner.lstAbilities[(int)Entity.ABILSLOT.ABIL3].AttemptManualUse();
            } else if (Input.GetKeyUp(KeyCode.Alpha4)) {
                Debug.Log("Would enter ability selection 4 at this point");

                //Clear all enemy telegraphs so that it's clear where your abilities are telegraphing themselves
                TelegraphController.Get().ClearAllTelegraphs();


                bUsedAbility = true;
                //yield return owner.lstAbilities[(int)Entity.ABILSLOT.ABIL4].AttemptManualUse();
            } else {
                yield return owner.lstAbilities[(int)Entity.ABILSLOT.MOVEMENT].AttemptManualUse();
            }

            if (bUsedAbility) {
                TelegraphController.Get().ClearAllTelegraphs();
                break;
            }

            //Not yielding here since each of the previous things above will yeild themselves if they fail to get input
            
        }

    }

}
