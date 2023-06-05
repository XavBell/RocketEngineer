using System.Data;
using System.Net.Mail;
using System.Globalization;
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
    public int numberOfStages;

    public void scanRocket()
    {
        Stages.Clear();
        findRocketParts();
        List<RocketPart> Engines = filterPart(RocketParts, "engine");
        List<RocketPart> Decouplers = filterPart(RocketParts, "decoupler");
        numberOfStages = Decouplers.Count + 1;
        List<AttachPointScript> Attachs = findAttachPoints();
        GroupAttachPoints(Attachs);
        scanStage();
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

    public List<RocketPart> filterPart(List<RocketPart> PartsToFilter, string type)
    {
        List<RocketPart> FilteredParts = new List<RocketPart>();
        foreach(RocketPart PotentialPart in PartsToFilter)
        {
            if(PotentialPart._partType == type)
            {
                FilteredParts.Add(PotentialPart);
            }
        }
        return FilteredParts;
    }

    public List<AttachPointScript> findAttachPoints()
    {
        List<AttachPointScript> AttachPoints = new List<AttachPointScript>();
        AttachPointScript[] Attachs = FindObjectsOfType<AttachPointScript>();
        foreach(AttachPointScript attach in Attachs)
        {
            AttachPoints.Add(attach);
        }
        AttachPoints = filterAttachPoints(AttachPoints);
        AttachPoints = GroupAttachPointsAgain(AttachPoints);
        
        return AttachPoints;
    }

    public List<AttachPointScript> filterAttachPoints(List<AttachPointScript> AttachsToFilter)
    {
        List<AttachPointScript> FilteredAttachPoints = new List<AttachPointScript>();
        foreach (AttachPointScript attach in AttachsToFilter)
        {
            if(attach.attachedBody != null)
            {
                FilteredAttachPoints.Add(attach);
            }   
        }
        return FilteredAttachPoints;
    }

    public List<AttachPointScript> GroupAttachPointsAgain(List<AttachPointScript> AttachToGroup)
    {
        List<AttachPointScript> GroupedAttach = new List<AttachPointScript>();
        GroupedAttach = AttachToGroup;
        for (int i = 0; i < AttachToGroup.Count; i++)
        {
            if(AttachToGroup[i].referenceBody.GetComponent<RocketPart>()._partType == "decoupler" || AttachToGroup[i].attachedBody.GetComponent<RocketPart>()._partType == "decoupler")
            {
                GroupedAttach.Remove(AttachToGroup[i]);
            }
        }
        return GroupedAttach;
    }

    public List<RocketPart> FilterRocketParts(List<RocketPart> PartsToFilter)
    {
        List<RocketPart> FilteredRocketParts = new List<RocketPart>();
        FilteredRocketParts = PartsToFilter;
        for (int i = 0; i < PartsToFilter.Count; i++)
        {
            if(PartsToFilter[i]._partType == "decoupler")
            {
                FilteredRocketParts.Remove(PartsToFilter[i]);
            }
        }

        return FilteredRocketParts;
    }


    public void GroupAttachPoints(List<AttachPointScript> AttachToGroup)
    {
        List<AttachPointScript> GroupedAttach = GroupAttachPointsAgain(AttachToGroup);
        
        List<RocketPart> FilteredRocketParts = FilterRocketParts(RocketParts);

        List<RocketPart> PartsPlaced = new List<RocketPart>();
        for (int x = 0; x < numberOfStages; x++)
        {
            foreach(RocketPart RP in FilteredRocketParts)
            {
                Stages Stage = new Stages();
                List<RocketPart> PartsInStage = new List<RocketPart>();

                if(!PartsPlaced.Contains(RP))
                {
                    PartsPlaced.Add(RP);
                    PartsInStage.Add(RP);

                    for(int i = 0; i < GroupedAttach.Count; i++)
                    {
                        if(PartsInStage.Contains(GroupedAttach[i].attachedBody.GetComponent<RocketPart>()) && !PartsPlaced.Contains(GroupedAttach[i].referenceBody.GetComponent<RocketPart>()))
                        {
                            PartsInStage.Add(GroupedAttach[i].referenceBody.GetComponent<RocketPart>());
                            PartsPlaced.Add(GroupedAttach[i].referenceBody.GetComponent<RocketPart>());
                            i = 0;
                        }
                    }

                    foreach (RocketPart RPA in PartsInStage)
                    {
                        Stage.Parts.Add(RPA);   
                    }
                    Stages.Add(Stage);
                }
            }
        }
    }

    public void scanStage()
    {
        int StageNumber = 0;
        foreach(Stages Stage in Stages)
        {
            UnityEngine.Debug.Log("Stage " + StageNumber + " infos: " + "Number of parts: " + Stage.Parts.Count);
            int PartNumber = 0;
            foreach (RocketPart RP in Stage.Parts)
            {
                UnityEngine.Debug.Log("Stage " + StageNumber + " part " + PartNumber + " Guid: " + RP._partID + " Type: " + RP._partType);
                PartNumber++;
            }
            StageNumber++;
        }
    }

}
