using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour {

    public Entity entity;

    public int nGold;//To be changed later;

    // Start is called before the first frame update
    void Start() {
        entity = GetComponent<Entity>();
    }
    
}
