using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController> {

    public Queue<Entity> queueEntities;
    public HashSet<Entity> setActiveEntities; //keep a set of all active entities so that we can ignore inactive ones

    public float fDelayBetweenClears;

    public EntHero entHero;

    public override void Init() {
        queueEntities = new Queue<Entity>();
        setActiveEntities = new HashSet<Entity>();
    }

    public void RegisterEntity(Entity entity) {

        queueEntities.Enqueue(entity);
        setActiveEntities.Add(entity);

    }

    public void UnregisterEntity(Entity entity) {

        setActiveEntities.Remove(entity);

    }

    public Entity GetNextActingEntity() {

        Entity entityNext = queueEntities.Dequeue();

       
        if (setActiveEntities.Contains(entityNext)) {
            //If the entity at the beginning of the queue exists, we should re-add it to the end of the queue, then return it
            queueEntities.Enqueue(entityNext);

            return entityNext;
        } else {
            //If the entity that was at the beginning was already unregistered, we shouldn't re-add it to the back of the queue
            //and should instead recurse to see if the next entity is active

            Debug.Log("SKIPPING SINCE NON-ACTIVE");
            return GetNextActingEntity();
        }
    }

    //TODO:: Consider if this can be initialized by this class rather than the board, but in a way that 
    //       could ensure the board gets a chance to finish initiallizing itself
    public IEnumerator GameLoop() {

        //Initially pause for a frame to ensure all initializations can happen before moving forward with the game loop
        yield return null;

        Board.Get().UpdatePathDistsFromPlayer();

        while (true) {
            

            //Initially ensure that our starting matches all get cleaned up
            while (true) {
                int nFlagged = Board.Get().FlagMatches();

                //If there were no flagged matches, we can leave this loop
                if (nFlagged == 0) break;

                yield return new WaitForSeconds(fDelayBetweenClears);

                //If there are flagged matches, keep cleaning them up
                yield return Board.Get().CleanupMatches();

                
            }

            Board.Get().UpdatePathDistsFromPlayer();
            yield return GetNextActingEntity().abilityselector.SelectAndUseAbility();

            yield return new WaitForSeconds(0.1f);
             
            //After performing the entity's actions, ensure the player is moved to the center of the board
            Board.Get().RealignPlayer();

            yield return Board.Get().AnimateMovingTiles(Board.Get().fBoardScrollTime);
            
        }
    }


}
