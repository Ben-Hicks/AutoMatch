using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to store all possible entity prefabs and to spawn desired enemy entities - acts as a factory

public class EntityController : Singleton<EntityController> {

    public GameObject pfHero;
    public List<GameObject> lstEnemyPrefabs;


    //Maybe make this into a coroutine with some spawning-in Coroutine
    public void PlaceEntity(GameObject pfEntity, Tile tile) {
        Debug.Assert(tile.HasEntity() == false);

        GameObject inst = Instantiate(pfEntity, tile.transform);

        Debug.Assert(inst.GetComponent<Entity>() != null);

        tile.SetEntity(inst.GetComponent<Entity>());
    }

    public override void Init() {
        
    }

}
