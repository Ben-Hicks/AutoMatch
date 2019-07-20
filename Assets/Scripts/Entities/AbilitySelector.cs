using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilitySelector : MonoBehaviour {

    public Entity owner;
    public Entity entTarget;

    public abstract IEnumerator SelectAndUseAbility();

    public void SetTarget(Entity _entTarget) {
        entTarget = _entTarget;
    }

    //Use whatever base logic to decide who to attack - typically the player
    public virtual void AcquireTarget() {

        SetTarget(GameController.Get().entHero);

    }

    public virtual void Start() {
        owner = GetComponent<Entity>();

        AcquireTarget();
    }



    
}
