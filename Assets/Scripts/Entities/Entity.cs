using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public List<Ability> lstActions;//TO UPDATE
    public int nMaxHealth;
    public int nCurHealth;

    public Tile tile;
    public AbilitySelector abilityselector;

    public void SetTile(Tile _tile) {
        Debug.Assert(_tile != null);

        tile = _tile;
    }

    public void SetAbilitySelector(AbilitySelector _abilityselector) {
        abilityselector = _abilityselector;
    }


    // Start is called before the first frame update
    void Start() {
        abilityselector = GetComponent<AbilitySelector>();
    }

    // Update is called once per frame
    void Update() {

    }
}
