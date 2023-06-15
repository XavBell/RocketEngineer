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
    public int numberOfStages;

    public void scanRocket()
    {
        Stages.Clear();
        List<RocketPart> RocketParts = findRocketParts();
        List<RocketPart> Decouplers = filterPart(RocketParts, "decoupler");
        numberOfStages = Decouplers.Count + 1;
        CreateStage(RocketParts);
        addDecouplerStages(Decouplers);
        //addDecouplerStages();
        scanStage();
    }

    public List<RocketPart> findRocketParts()
    {
        List<RocketPart> RocketParts = new List<RocketPart>();
        RocketPart[] rocketParts = FindObjectsOfType<RocketPart>();
        foreach(RocketPart rp in rocketParts)
        {
            RocketParts.Add(rp);
        }
        return RocketParts;
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
        AttachPoints = GroupAttachPoints(AttachPoints);
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

    public List<AttachPointScript> GroupAttachPoints(List<AttachPointScript> AttachToGroup)
    {
        List<AttachPointScript> AttachToRemove = new List<AttachPointScript>();
        foreach (AttachPointScript Attach in AttachToGroup)
        {
            if(Attach.referenceBody.GetComponent<RocketPart>()._partType == "decoupler" || Attach.attachedBody.GetComponent<RocketPart>()._partType == "decoupler")
            {
                AttachToRemove.Add(Attach);
            }
        }

        foreach (AttachPointScript removeAtt in AttachToRemove)
        {
            AttachToGroup.Remove(removeAtt);
        }

        return AttachToGroup;
    }

    public List<RocketPart> RemoveDecouplersFromList(List<RocketPart> PartsToFilter)
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


    public void CreateStage(List<RocketPart> RocketParts)
    {
        List<AttachPointScript> GroupedAttach = findAttachPoints();
        
        List<RocketPart> FilteredRocketParts = RemoveDecouplersFromList(RocketParts);

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

    public void addDecouplerStages(List<RocketPart> Decouplers)
    {
        foreach (RocketPart _decoupler in Decouplers)
        {
            if(_decoupler._attachBottom.GetComponent<AttachPointScript>().attachedBody != null)
            {
                RocketPart BottomPart = _decoupler._attachBottom.GetComponent<AttachPointScript>().attachedBody.gameObject.GetComponent<RocketPart>();

                if (BottomPart != null)
                {
                    foreach(Stages Stage in Stages)
                    {
                        List<RocketPart> PartsToAdd = new List<RocketPart>(); 
                        foreach(RocketPart Part in Stage.Parts)
                        {
                            if(Part == BottomPart)
                            {
                                PartsToAdd.Add(_decoupler);
                            }
                        }
    
                        foreach (RocketPart rp in PartsToAdd)
                        {
                            Stage.Parts.Add(rp);   
                        }
                    }
                }
            }

            if(_decoupler._attachBottom.GetComponent<AttachPointScript>().attachedBody == null)
            {
                Stages Stage = new Stages();
                Stage.Parts.Add(_decoupler);
                Stages.Add(Stage);
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
