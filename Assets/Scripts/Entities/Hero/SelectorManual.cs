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

                yield return owner.arAbilities[(int)Entity.ABILSLOT.ABIL1].AttemptManualUse(owner);
            } else if (Input.GetKeyUp(KeyCode.Alpha2)) {
                Debug.Log("Manually using ability 2");

                //Clear all enemy telegraphs so that it's clear where your abilities are telegraphing themselves
                TelegraphController.Get().ClearAllTelegraphs();


                yield return owner.arAbilities[(int)Entity.ABILSLOT.ABIL2].AttemptManualUse(owner);
            } else if (Input.GetKeyUp(KeyCode.Alpha3)) {
                Debug.Log("Manually using ability 3");

                //Clear all enemy telegraphs so that it's clear where your abilities are telegraphing themselves
                TelegraphController.Get().ClearAllTelegraphs();


                yield return owner.arAbilities[(int)Entity.ABILSLOT.ABIL3].AttemptManualUse(owner);
            } else if (Input.GetKeyUp(KeyCode.Alpha4)) {
                Debug.Log("Manually using ability 4");

                //Clear all enemy telegraphs so that it's clear where your abilities are telegraphing themselves
                TelegraphController.Get().ClearAllTelegraphs();
                
                yield return owner.arAbilities[(int)Entity.ABILSLOT.ABIL4].AttemptManualUse(owner);
            } else {
                yield return owner.arAbilities[(int)Entity.ABILSLOT.MOVEMENT].AttemptManualUse(owner);
            }

            if (bUsedAbility) {
                TelegraphController.Get().ClearAllTelegraphs();
                break;
            }

            //Not yielding here since each of the previous things above will yeild themselves if they fail to get input
            
        }

    }

}
