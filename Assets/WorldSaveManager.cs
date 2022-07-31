using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class WorldSaveManager : MonoBehaviour
{
    public void saveWorld()
    {
        public GameObject[] rockets = GameObject.FindGameObjectsWithTag<"capsule">();
        foreach(GameObject rocket in rockets)
        {
            saveWorld saveWorld = new saveWorld();
            saveWorld.types = new List<string>();
        }
    }
}
