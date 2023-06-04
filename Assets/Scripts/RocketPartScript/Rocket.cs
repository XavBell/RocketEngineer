using System.Diagnostics;
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
        findRocketParts();
        List<RocketPart> Engines = filterEngine(RocketParts);
        createStage(Engines);
    }

    public void findRocketParts()
    {
        RocketParts = new List<RocketPart>();
        RocketPart[] rocketParts = FindObjectsOfType<RocketPart>();
        foreach(RocketPart rp in rocketParts)
        {
            RocketParts.Add(rp);
        }
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

    public void createStage(List<RocketPart> Engines)
    {
        foreach(RocketPart Engine in Engines)
        {
            Stages Stage = new Stages();
            AttachPointScript TopAttach = Engine.GetComponent<RocketPart>()._attachTop.GetComponent<AttachPointScript>();
            while(TopAttach.attachedBody != null)
            {
                RocketPart NextPart = TopAttach.attachedBody.GetComponent<RocketPart>();
                if(NextPart._partType == "tank")
                {
                    
                }
            }
        }
    }    
}
