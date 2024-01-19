using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stages
{
    [field: SerializeField] public List<System.Guid> PartsID = new List<System.Guid>();
    [field: SerializeField] public List<RocketPart> Parts = new List<RocketPart>();

    public Vector2 thrust;

    public bool engineStarted = false;
    public float startTime = 0;
    public TimeManager MyTime;
    

    public void updateThrust(float thrustCoefficient)
    {
        if(engineStarted == false)
        {
            startTime = Time.time;
        }

        thrust = Vector2.zero;
        float oxidizerQty = GetQty("oxidizer");
        float fuelQty = GetQty("fuel");

        string oxidizerType = GetType("oxidizer").name;
        string fuelType = GetType("fuel").name;

        bool liquid = GetState();

        List<RocketPart> engines = GetEngines();
        float massFlowRate = totalMassFlowRate(engines) * thrustCoefficient;

        string propellantType = GetPropellant(oxidizerType, fuelType);
        float percentageFuel = 0f;
        float percentageOxidizer = 0f;
        if(propellantType != "none")
        {
            if(propellantType == "RP-1")
            {
                float ratio = 2.56f; //oxidizer to fuel ratio of RP-1
                //it means that for 2.56kg of oxidizer, 1 kg of fuel is consumed

                percentageOxidizer = ratio/(ratio + 1);
                percentageFuel = 1f/(ratio + 1);
            }
        }

        //Shouldn't need to use the MyTime because it's only ran in simulate mode
        float consumedFuel = Time.deltaTime * massFlowRate * percentageFuel;
        float consumedOxidizer = Time.deltaTime * massFlowRate * percentageOxidizer;
        if(fuelQty - consumedFuel >= 0 && oxidizerQty - consumedOxidizer >= 0 && consumedFuel != 0 && consumedOxidizer != 0 && liquid == true)
        {
            foreach(RocketPart engine in engines)
            {

                //Fix to have the right start time
                bool fail;
                float rawThrust = engine.GetComponent<Engine>().CalculateOutputThrust(Time.time - startTime, out fail);
                if(fail == false && engine.GetComponent<Engine>().operational == true)
                {
                    thrust += thrustCoefficient * new Vector2(engine.gameObject.transform.up.x, engine.gameObject.transform.up.y) * rawThrust;
                }

                if(fail == true)
                {
                    engine.GetComponent<Engine>().operational = false;
                }
            }

            foreach(RocketPart tank in Parts)
            {
                if(tank._partType == "tank")
                {
                    if(tank.GetComponent<outputInputManager>().circuit == "oxidizer")
                    {
                        if(tank.GetComponent<outputInputManager>().moles - tank.GetComponent<outputInputManager>().mass/oxidizerQty*consumedOxidizer*1000f/tank.GetComponent<outputInputManager>().substance.MolarMass < 0)
                        {
                            tank.GetComponent<outputInputManager>().moles = 0;
                        }else{
                            tank.GetComponent<outputInputManager>().moles -= tank.GetComponent<outputInputManager>().mass/oxidizerQty*consumedOxidizer*1000f/tank.GetComponent<outputInputManager>().substance.MolarMass;
                        }
                        
                    }

                    if(tank.GetComponent<outputInputManager>().circuit == "fuel")
                    {
                        if(tank.GetComponent<outputInputManager>().moles - tank.GetComponent<outputInputManager>().mass/fuelQty*consumedFuel*1000f/tank.GetComponent<outputInputManager>().substance.MolarMass < 0)
                        {
                            tank.GetComponent<outputInputManager>().moles = 0;
                        }else{
                            tank.GetComponent<outputInputManager>().moles -= tank.GetComponent<outputInputManager>().mass/fuelQty*consumedFuel*1000f/tank.GetComponent<outputInputManager>().substance.MolarMass;
                        }
                        
                    }
                }
            }
        }
        engineStarted = true;  
    }

    string GetPropellant(string oxidizer, string fuel)
    {
        if(oxidizer == "LOX" && fuel == "kerosene")
        {
            return "RP-1";
        }else{
            return "none";
        }
    }
    
    bool GetState()
    {
        foreach(RocketPart part in Parts)
        {
            if(part._partType == "tank")
            {
                if(part.GetComponent<outputInputManager>().state != "liquid")
                {
                    return false;
                }
            }
        }

        return true;
    }

    float GetQty(string type)
    {
        float qty = 0f;
        foreach(RocketPart part in Parts)
        {
            if(part._partType == "tank")
            {
                if(part.GetComponent<outputInputManager>().circuit == type)
                {
                    qty += part.GetComponent<outputInputManager>().mass;
                }
            }
        }

        return qty;
    }

    Substance GetType(string type)
    {
        Substance substance = null;
        foreach(RocketPart part in Parts)
        {
            if(part._partType == "tank")
            {
                if(part.GetComponent<outputInputManager>().circuit == type)
                {
                    substance = part.GetComponent<outputInputManager>().substance;
                }
            }
        }

        return substance;
    }

    List<RocketPart> GetEngines()
    {
        List<RocketPart> engines = new List<RocketPart>();
        foreach(RocketPart part in Parts)
        {
            if(part._partType == "engine")
            {
                if(part.GetComponent<Engine>().active == true)
                {
                    engines.Add(part);
                }
            }
        }
        return engines;
    }

    float totalMassFlowRate(List<RocketPart> Engines)
    {
        float massFlowRate = 0f;
        foreach(RocketPart engine in Engines)
        {
            massFlowRate += engine.GetComponent<Engine>()._rate;
        }
        return massFlowRate;
    }

}
