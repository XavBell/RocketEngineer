using System.Numerics;
using System;
using System.Runtime.InteropServices.ComTypes;
using System.Data;
using System.Net.Mail;
using System.Globalization;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public GameObject core;
    public List<Stages> Stages = new List<Stages>();
    public int numberOfStages;

    public float rocketMass;
    public float throttle = 100f;
    public List<Engine> engines = new List<Engine>();
    public UnityEngine.Vector2 currentThrust;

    void Update()
    {

        
    }

    public void controlThrust()
    {
        updateActiveEngines();
        if(Input.GetKey(KeyCode.Z))
        {
            UnityEngine.Debug.Log(Stages.Count);
            List<UnityEngine.Vector2> totalThrust = new List<UnityEngine.Vector2>();  
            foreach(Engine engine in engines)
            {
                totalThrust.Add(engine.gameObject.transform.up.normalized * engine._thrust * throttle);
            }
            currentThrust = new UnityEngine.Vector2(0, 0);
            foreach(UnityEngine.Vector2 thrust in totalThrust)
            {
                currentThrust += thrust;
            }
            UnityEngine.Debug.Log(currentThrust);

        }
        if(!Input.GetKey(KeyCode.Z) && currentThrust != new UnityEngine.Vector2(0, 0))
        {
            currentThrust = new UnityEngine.Vector2(0,0);
        }
    }

    public void updateMass()
    {
        rocketMass = 0;
        foreach(Stages stage in Stages)
        {
            foreach(RocketPart part in stage.Parts)
            {
                rocketMass += part._partMass;
            }
        }
    }

    public void _orientation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, 0 , Time.deltaTime*50);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, 0, Time.deltaTime * -50);
        }
    }

    public void updateActiveEngines()
    {
        engines.Clear();
        foreach(Stages stage in Stages)
        {
            foreach(RocketPart part in stage.Parts)
            {
                if(part._partType == "engine")
                {
                    if(part.GetComponent<Engine>().active == true)
                    {
                        engines.Add(part.GetComponent<Engine>());
                    }
                }
                
            }
        }
    }

    void deleteReference(RocketPart rp, string reference)
    {
        RocketPart partAttached = new RocketPart();

        //Find decoupler reference in part
        if(reference == "top")
        {
            partAttached = rp.GetComponent<RocketPart>()._attachTop.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>();
        }

        if(reference == "bottom")
        {
            partAttached = rp.GetComponent<RocketPart>()._attachBottom.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>();
        }
            
            if(partAttached._attachTop != null)
            {
                if(partAttached._attachTop.GetComponent<AttachPointScript>().attachedBody == rp.gameObject)
                {
                    partAttached._attachTop.GetComponent<AttachPointScript>().attachedBody = null;
                }
                
            }

            if(partAttached._attachBottom != null)
            {
                if(partAttached._attachBottom.GetComponent<AttachPointScript>().attachedBody == rp.gameObject)
                {
                    partAttached._attachBottom.GetComponent<AttachPointScript>().attachedBody = null;
                }
            }

            if(partAttached._attachLeft != null)
            {
                if(partAttached._attachLeft.GetComponent<AttachPointScript>().attachedBody == rp.gameObject)
                {
                    partAttached._attachLeft.GetComponent<AttachPointScript>().attachedBody = null;
                }
            }

            if(partAttached._attachRight != null)
            {
                if(partAttached._attachRight.GetComponent<AttachPointScript>().attachedBody == rp.gameObject)
                {
                    partAttached._attachRight.GetComponent<AttachPointScript>().attachedBody = null;
                }
            }
    }

    public void updateRocketStaging()
    {
        //Find if there is a decoupled decoupler and its stage pos
        RocketPart rp = new RocketPart();
        int i = 0;
        int stagePos = 1000*1000;
        foreach(Stages stage in Stages)
        {
            foreach(RocketPart part in stage.Parts)
            {
                if(part._partType == "decoupler")
                {
                    if(part.GetComponent<Decoupler>().activated == true)
                    {
                        rp = part;
                        stagePos = i;
                        part.GetComponent<Decoupler>().activated = false;
                    }
                }
                
            }
            i++;
        }

        if(stagePos != 1000*1000)
        {
            RocketPart previousPartToo = rp._attachTop.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>();
            deleteReference(rp, "top");
            rp.GetComponent<RocketPart>()._attachTop.GetComponent<AttachPointScript>().attachedBody = null;

            List<RocketPart> inStage = new List<RocketPart>();
            List<int> inPos = new List<int>();
            inPos.Add(stagePos);
            int previousCount = 0;
            int currentCount = 1;

            bool coreIn = false;

            foreach(RocketPart part in Stages[stagePos].Parts)
            {
                inStage.Add(part);
            }

            while(currentCount != previousCount)
            {
                foreach(Stages stage in Stages)
                {
                    foreach(RocketPart part in stage.Parts)
                    {
                        if(part != rp)
                        {
                            if(part.GetComponent<RocketPart>()._attachTop != null)
                            {

                            
                                if(part.GetComponent<RocketPart>()._attachTop.GetComponent<AttachPointScript>().attachedBody != null)
                                {
                                    if(inStage.Contains(part.GetComponent<RocketPart>()))
                                    {
                                        int j = 0;
                                        foreach(Stages stage2 in Stages)
                                        {
                                            foreach(RocketPart part2 in stage2.Parts)
                                            {
                                                if(part2 == part.GetComponent<RocketPart>()._attachTop.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>())
                                                {
                                                    if(inPos.Contains(j) == false)
                                                    {
                                                        inPos.Add(j);
                                                    }

                                                    if(inStage.Contains(part2) == false)
                                                    {
                                                        inStage.Add(part2);
                                                    }
                                                }
                                            }
                                            j++;
                                        }
                                    }
                                }   

                            }

                            if(part.GetComponent<RocketPart>()._attachBottom != null)
                            {
                                if(part.GetComponent<RocketPart>()._attachBottom.GetComponent<AttachPointScript>().attachedBody != null)
                                {
                                    if(inStage.Contains(part.GetComponent<RocketPart>()))
                                    {
                                        int j = 0;
                                        foreach(Stages stage2 in Stages)
                                        {
                                            foreach(RocketPart part2 in stage2.Parts)
                                            {
                                                if(part2 == part.GetComponent<RocketPart>()._attachBottom.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>())
                                                {
                                                    if(inPos.Contains(j) == false)
                                                    {
                                                        inPos.Add(j);
                                                    }

                                                    if(inStage.Contains(part2) == false)
                                                    {
                                                        inStage.Add(part2);
                                                    }
                                                }
                                            }
                                            j++;
                                        }
                                    }
                                }
                            }

                            if(part.GetComponent<RocketPart>()._attachLeft != null)
                            {
                                if(part.GetComponent<RocketPart>()._attachLeft.GetComponent<AttachPointScript>().attachedBody != null)
                                {
                                    if(inStage.Contains(part.GetComponent<RocketPart>()))
                                    {
                                        int j = 0;
                                        foreach(Stages stage2 in Stages)
                                        {
                                            foreach(RocketPart part2 in stage2.Parts)
                                            {
                                                if(part2 == part.GetComponent<RocketPart>()._attachLeft.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>())
                                                {
                                                    if(inPos.Contains(j) == false)
                                                    {
                                                        inPos.Add(j);
                                                    }
                                                    if(inStage.Contains(part2) == false)
                                                    {
                                                        inStage.Add(part2);
                                                    }
                                                }
                                            }
                                            j++;
                                        }
                                    }
                                }
                            }

                            if(part.GetComponent<RocketPart>()._attachRight != null)
                            {
                                if(part.GetComponent<RocketPart>()._attachRight.GetComponent<AttachPointScript>().attachedBody != null)
                                {
                                    if(inStage.Contains(part.GetComponent<RocketPart>()))
                                    {
                                        int j = 0;
                                        foreach(Stages stage2 in Stages)
                                        {
                                            foreach(RocketPart part2 in stage2.Parts)
                                            {
                                                if(part2 == part.GetComponent<RocketPart>()._attachRight.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>())
                                                {
                                                    if(inPos.Contains(j) == false)
                                                    {
                                                        inPos.Add(j);
                                                    }

                                                    if(inStage.Contains(part2) == false)
                                                    {
                                                        inStage.Add(part2);
                                                    }
                                                }
                                            }
                                            j++;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }

                previousCount = currentCount;
                currentCount = inStage.Count;
                
            }

            UnityEngine.Debug.Log("Num pos" + inPos.Count);

            //Check if in other way
            foreach(int pos in inPos)
            {
                foreach(RocketPart part in Stages[pos].Parts)
                {
                    if(part.gameObject == core.gameObject)
                    {
                        coreIn = true;
                    }
                }
            }

            
            UnityEngine.Debug.Log(coreIn);

            if(coreIn == false)
            {
                rp.GetComponent<RocketPart>()._attachTop.GetComponent<AttachPointScript>().attachedBody = null;
                if(rp.GetComponent<Rigidbody2D>() == null)
                {
                    rp.gameObject.AddComponent<Rigidbody2D>();
                }
                rp.GetComponent<Rigidbody2D>().simulated = true;
                rp.GetComponent<Rigidbody2D>().freezeRotation = true;
            
                rp.gameObject.AddComponent<Rocket>();
                rp.gameObject.AddComponent<PlanetGravity>();
                rp.GetComponent<PlanetGravity>().initialized = true;
                rp.GetComponent<PlanetGravity>().possessed = false;
                rp.gameObject.GetComponent<Rocket>().core = rp.gameObject;
                rp.gameObject.GetComponent<PlanetGravity>().core = rp.gameObject;

                foreach(int pos in inPos)
                {
                    rp.GetComponent<Rocket>().Stages.Add(Stages[pos]);
                    this.Stages.RemoveAt(pos);
                    UnityEngine.Debug.Log("Pos " + pos);
                }

                foreach(RocketPart part in inStage)
                {
                    part.gameObject.transform.parent = rp.gameObject.transform;
                }

                rp.gameObject.transform.parent = null;
            }

            if(coreIn == true)
            {
                rp.GetComponent<RocketPart>()._attachTop.GetComponent<AttachPointScript>().attachedBody = previousPartToo.gameObject;
                if(previousPartToo._attachTop != null)
                {
                    if(previousPartToo._attachTop.GetComponent<AttachPointScript>().attachedBody == rp.gameObject)
                    {
                        previousPartToo._attachTop.GetComponent<AttachPointScript>().attachedBody = null;
                    }
                }

                if(previousPartToo._attachBottom != null)
                {
                    if(previousPartToo._attachBottom.GetComponent<AttachPointScript>().attachedBody == rp.gameObject)
                    {
                        previousPartToo._attachBottom.GetComponent<AttachPointScript>().attachedBody = null;
                    }
                }

                if(previousPartToo._attachRight != null)
                {
                    if(previousPartToo._attachRight.GetComponent<AttachPointScript>().attachedBody == rp.gameObject)
                    {
                        previousPartToo._attachRight.GetComponent<AttachPointScript>().attachedBody = null;
                    }
                }

                if(previousPartToo._attachLeft != null)
                {
                    if(previousPartToo._attachLeft.GetComponent<AttachPointScript>().attachedBody == rp.gameObject)
                    {
                        previousPartToo._attachLeft.GetComponent<AttachPointScript>().attachedBody = null;
                    }
                }

                rp.GetComponent<RocketPart>()._attachTop.GetComponent<AttachPointScript>().attachedBody = null;
                rp = previousPartToo;

                //Find stage pos
                int n = 0;
                int newPos = 1000;
                foreach(Stages stage in Stages)
                {
                    if(stage.Parts.Contains(rp))
                    {
                        newPos = n;
                    }
                    n++;
                }

                int newCurrentCount = -1;
                int newPreviousCount = 0;
                inStage = new List<RocketPart>();
                List<int> newInPos = new List<int>();
                newInPos.Add(newPos);

                foreach(RocketPart part in Stages[newPos].Parts)
                {
                    inStage.Add(part);
                }

                if(newPos != 1000)
                {
                    while(newCurrentCount != newPreviousCount)
                    {
                        foreach(Stages stage in Stages)
                        {
                            foreach(RocketPart part in stage.Parts)
                            {
                                if(part != rp)
                                {
                                    if(part.GetComponent<RocketPart>()._attachTop != null)
                                    {
                                        if(part.GetComponent<RocketPart>()._attachTop.GetComponent<AttachPointScript>().attachedBody != null)
                                        {
                                            if(inStage.Contains(part.GetComponent<RocketPart>()))
                                            {
                                                int j = 0;
                                                foreach(Stages stage2 in Stages)
                                                {
                                                    foreach(RocketPart part2 in stage2.Parts)
                                                    {
                                                        if(part2 == part.GetComponent<RocketPart>()._attachTop.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>())
                                                        {
                                                            if(newInPos.Contains(j) == false)
                                                            {
                                                                newInPos.Add(j);
                                                            }

                                                            if(inStage.Contains(part2) == false)
                                                            {
                                                                inStage.Add(part2);
                                                            }
                                                        }
                                                    }
                                                    j++;
                                                }
                                            }
                                        }
                                    }

                                    if(part.GetComponent<RocketPart>()._attachBottom != null)
                                    {
                                        if(part.GetComponent<RocketPart>()._attachBottom.GetComponent<AttachPointScript>().attachedBody != null)
                                        {
                                            if(inStage.Contains(part.GetComponent<RocketPart>()))
                                            {
                                                int j = 0;
                                                foreach(Stages stage2 in Stages)
                                                {
                                                    foreach(RocketPart part2 in stage2.Parts)
                                                    {
                                                        if(part2 == part.GetComponent<RocketPart>()._attachBottom.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>())
                                                        {
                                                            if(newInPos.Contains(j) == false)
                                                            {
                                                                newInPos.Add(j);
                                                            }

                                                            if(inStage.Contains(part2) == false)
                                                            {
                                                                inStage.Add(part2);
                                                            }
                                                        }
                                                    }
                                                    j++;
                                                }
                                            }
                                        }
                                    }

                                    if(part.GetComponent<RocketPart>()._attachLeft != null)
                                    {
                                        if(part.GetComponent<RocketPart>()._attachLeft.GetComponent<AttachPointScript>().attachedBody != null)
                                        {
                                            if(inStage.Contains(part.GetComponent<RocketPart>()))
                                            {
                                                int j = 0;
                                                foreach(Stages stage2 in Stages)
                                                {
                                                    foreach(RocketPart part2 in stage2.Parts)
                                                    {
                                                        if(part2 == part.GetComponent<RocketPart>()._attachLeft.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>())
                                                        {
                                                            if(newInPos.Contains(j) == false)
                                                            {
                                                                newInPos.Add(j);
                                                            }

                                                            if(inStage.Contains(part2) == false)
                                                            {
                                                                inStage.Add(part2);
                                                            }
                                                        }
                                                    }
                                                    j++;
                                                }
                                            }
                                        }
                                    }

                                    if(part.GetComponent<RocketPart>()._attachRight != null)
                                    {
                                        if(part.GetComponent<RocketPart>()._attachRight.GetComponent<AttachPointScript>().attachedBody != null)
                                        {
                                            if(inStage.Contains(part.GetComponent<RocketPart>()))
                                            {
                                                int j = 0;
                                                foreach(Stages stage2 in Stages)
                                                {
                                                    foreach(RocketPart part2 in stage2.Parts)
                                                    {
                                                        if(part2 == part.GetComponent<RocketPart>()._attachRight.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>())
                                                        {
                                                            if(newInPos.Contains(j) == false)
                                                            {
                                                                newInPos.Add(j);
                                                            }

                                                            if(inStage.Contains(part2) == false)
                                                            {
                                                                inStage.Add(part2);
                                                            }
                                                        }
                                                    }
                                                    j++;
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        newPreviousCount = newCurrentCount;
                        newCurrentCount = newInPos.Count;
                    }

                }

                rp.GetComponent<RocketPart>()._attachTop.GetComponent<AttachPointScript>().attachedBody = null;
                if(rp.GetComponent<Rigidbody2D>()== null)
                {
                    rp.gameObject.AddComponent<Rigidbody2D>();
                }
                rp.GetComponent<Rigidbody2D>().simulated = true;
                rp.GetComponent<Rigidbody2D>().freezeRotation = true;
            
                rp.gameObject.AddComponent<Rocket>();
                rp.gameObject.AddComponent<PlanetGravity>();
                rp.GetComponent<PlanetGravity>().initialized = true;
                rp.GetComponent<PlanetGravity>().possessed = false;
                rp.gameObject.GetComponent<Rocket>().core = rp.gameObject;
                rp.gameObject.GetComponent<PlanetGravity>().core = rp.gameObject;

                foreach(int pos in newInPos)
                {
                    rp.GetComponent<Rocket>().Stages.Add(Stages[pos]);
                }

                foreach(RocketPart part in inStage)
                {
                    part.gameObject.transform.parent = rp.gameObject.transform;
                }
                
                foreach(int pos in newInPos)
                {
                    this.Stages.RemoveAt(pos);
                }

                rp.gameObject.transform.parent = null;
            }
        }

    }

    public void scanRocket()
    {
        Stages.Clear();
        List<RocketPart> RocketParts = findRocketParts();
        List<RocketPart> Decouplers = filterPart(RocketParts, "decoupler");
        numberOfStages = Decouplers.Count + 1;
        CreateStage(RocketParts);
        addDecouplerStages(Decouplers);
        cleanStage();
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
                UnityEngine.Debug.Log(null);
                Stages Stage = new Stages();
                Stage.Parts.Add(_decoupler);
                Stages.Add(Stage);
            }
        }
    }

    void cleanStage()
    {
        for(int i = 0; i < Stages.Count; i++)
        {
            if(Stages[i].Parts.Count == 1 && Stages[i].Parts[0]._partType == "decoupler")
            {
                Stages.Remove(Stages[i]);
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
