﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to store all possible entity prefabs and to spawn desired enemy entities - acts as a factory

public class PropertyController : Singleton<PropertyController> {

    [System.Serializable]
    public struct PropertyEntry {
        public string sName;
        public GameObject goProperty;
    }

    public PropertyEntry[] arPropertyEntries;

    bool bDictInitialized;
    public Dictionary<string, GameObject> dictProperties;

    public void PlaceProperty(string sName, Tile tile) {
        PlaceProperty(GetPropertyPrefabByName(sName), tile);
    }

    //Maybe make this into a coroutine with some spawning-in Coroutine
    public void PlaceProperty(GameObject pfProperty, Tile tile) {

        if(tile.prop != null) {
            if (pfProperty.GetComponent<Property>().overwritePriority < tile.prop.overwritePriority) {
                //If the new property can't overwrite the existing property on this tile, then we shouldn't place anything here
                Debug.Log("Blocking a spawn since the new tile's priority isn't high enough since " + pfProperty.GetComponent<Property>().overwritePriority + " is less than " + tile.prop.overwritePriority);
                return;
            } else {

                //If this tile already have a previous property on it, then we'll have to clear out that property
                GameObject.Destroy(tile.prop.go);
            }
        }


        GameObject inst = Instantiate(pfProperty, tile.transform);

        Debug.Assert(inst.GetComponent<Property>() != null);
        
        tile.SetProperty(inst.GetComponent<Property>());

        //Make sure the property knows which GameObject it is attached to and representing it
        tile.prop.SetGameObject(inst);
    }

    void InitPropertiesDictionary() {
        bDictInitialized = true;

        dictProperties = new Dictionary<string, GameObject>();

        foreach (PropertyEntry entry in arPropertyEntries) {
            dictProperties.Add(entry.sName, entry.goProperty);
        }
    }

    public GameObject GetPropertyPrefabByName(string sName) {
        if (bDictInitialized == false) InitPropertiesDictionary();

        if(dictProperties.ContainsKey(sName) == false) {
            Debug.LogError("No property prefab exists for name: " + sName);
            return null;
        }

        return dictProperties[sName];
    }

    public void SpawnNewProperty(Tile tile) {
        //TODO:: Replace this with a class whose job is to only decide which type of tiles should be spawned given the circumstance of the game

        //Set the overwriting priority to the minimum so that any tile can be placed overtop of this
        if (tile.prop != null) {
            tile.prop.overwritePriority = Property.PRIORITY.NEG;
        }

        float fRand = Random.Range(0, 100);

        if(fRand < 3) {
            PlaceProperty("Stabber", tile);
        }else if(fRand < 70) {
            PlaceProperty("Gold", tile);
        } else {
            PlaceProperty("Blocker", tile);
        }


    }

    public override void Init() {
        if (bDictInitialized == false) InitPropertiesDictionary();
    }

}