using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [field: SerializeField]
    public GameObject core {get; set;}
    public List<Stages> Stages = new List<Stages>();
    public List<RocketPart> RocketParts = new List<RocketPart>();

    public void scanRocket()
    {
        RocketParts = new List<RocketPart>();
        RocketPart[] rocketParts = FindObjectsOfType<RocketPart>();
        UnityEngine.Debug.Log(rocketParts.Length);
        foreach(RocketPart rp in rocketParts)
        {
            RocketParts.Add(rp);
        }
        
        List<RocketPart> Engines = filterEngine(RocketParts);
        UnityEngine.Debug.Log(Engines.Count);
    }

    public List<RocketPart> filterEngine(List<RocketPart> PartsToFilter)
    {
        List<RocketPart> Engines = new List<RocketPart>();
        foreach(RocketPart PotentialEngine in PartsToFilter)
        {
            if(PotentialEngine._partType == "engine")
            {
                Engines.Add(PotentialEngine);
            }
        }
        return Engines;
    }    
}
