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
        if (engineStarted == false)
        {
            startTime = Time.time;
        }

        thrust = Vector2.zero;
        float oxidizerQty = GetQty("oxidizer");
        float fuelQty = GetQty("fuel");

        string oxidizerType = null;
        string fuelType = null;
        if (fuelQty > 0 && oxidizerQty > 0)
        {
            oxidizerType = GetType("oxidizer").name;
            fuelType = GetType("fuel").name;
        }


        bool liquid = GetState();

        List<RocketPart> engines = GetEngines();
        float massFlowRate = totalMassFlowRate(engines) * thrustCoefficient;

        string propellantType = GetPropellant(oxidizerType, fuelType);
        float percentageFuel = 0f;
        float percentageOxidizer = 0f;
        if (propellantType != "none")
        {
            if (propellantType == "RP-1")
            {
                float ratio = 2.56f; //oxidizer to fuel ratio of RP-1
                //it means that for 2.56kg of oxidizer, 1 kg of fuel is consumed

                percentageOxidizer = ratio / (ratio + 1);
                percentageFuel = 1f / (ratio + 1);
            }
        }

        //Shouldn't need to use the MyTime because it's only ran in simulate mode
        float consumedFuel = Time.deltaTime * massFlowRate * percentageFuel;
        float consumedOxidizer = Time.deltaTime * massFlowRate * percentageOxidizer;
        if (fuelQty - consumedFuel >= 0 && oxidizerQty - consumedOxidizer >= 0 && consumedFuel != 0 && consumedOxidizer != 0 && liquid == true)
        {
            foreach (RocketPart engine in engines)
            {
                //Fix to have the right start time
                bool fail;
                float rawThrust = engine.GetComponent<Engine>().CalculateOutputThrust(Time.time - startTime, out fail);
                if (fail == false && engine.GetComponent<Engine>().operational == true)
                {
                    thrust += thrustCoefficient * new Vector2(engine.gameObject.transform.up.x, engine.gameObject.transform.up.y) * rawThrust;
                    foreach (RocketPart tank in Parts)
                    {
                        if (tank._partType == "tank")
                        {
                            if (tank.GetComponent<container>().substance != null)
                            {
                                if (tank.GetComponent<Tank>().propellantCategory == "oxidizer")
                                {
                                    if (tank.GetComponent<container>().moles - tank.GetComponent<container>().mass / oxidizerQty * consumedOxidizer * 1000f / tank.GetComponent<container>().substance.MolarMass <= 0)
                                    {
                                        tank.GetComponent<container>().moles = 0;
                                        engine.GetComponent<Engine>().lackFuel = true;
                                        thrust = Vector2.zero;
                                    }
                                    else
                                    {
                                        tank.GetComponent<container>().moles -= tank.GetComponent<container>().mass / oxidizerQty * consumedOxidizer * 1000f / tank.GetComponent<container>().substance.MolarMass;
                                        engine.GetComponent<Engine>().lackFuel = false;
                                    }

                                }

                                if (tank.GetComponent<Tank>().propellantCategory == "fuel")
                                {
                                    if (tank.GetComponent<container>().moles - tank.GetComponent<container>().mass / fuelQty * consumedFuel * 1000f / tank.GetComponent<container>().substance.MolarMass <= 0)
                                    {
                                        tank.GetComponent<container>().moles = 0;
                                        engine.GetComponent<Engine>().lackFuel = true;
                                        thrust = Vector2.zero;
                                    }
                                    else
                                    {
                                        tank.GetComponent<container>().moles -= tank.GetComponent<container>().mass / fuelQty * consumedFuel * 1000f / tank.GetComponent<container>().substance.MolarMass;
                                        engine.GetComponent<Engine>().lackFuel = false;
                                    }

                                }
                            }
                        }

                    }
                }

                if (fail == true)
                {
                    engine.GetComponent<Engine>().operational = false;
                }
            }


        }
        engineStarted = true;
    }

    string GetPropellant(string oxidizer, string fuel)
    {
        if (oxidizer == "LOX" && fuel == "kerosene")
        {
            return "RP-1";
        }
        else
        {
            return "none";
        }
    }

    bool GetState()
    {
        foreach (RocketPart part in Parts)
        {
            if (part._partType == "tank")
            {
                if (part.GetComponent<container>().state != "liquid" && part.GetComponent<container>().substance != null)
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
        foreach (RocketPart part in Parts)
        {
            if (part._partType == "tank")
            {
                if (part.GetComponent<Tank>().propellantCategory == type)
                {
                    qty += part.GetComponent<container>().mass;
                }
            }
        }

        return qty;
    }

    Substance GetType(string type)
    {
        Substance substance = null;
        foreach (RocketPart part in Parts)
        {
            if (part._partType == "tank")
            {
                if (part.GetComponent<Tank>().propellantCategory == type && part.GetComponent<container>().substance != null)
                {
                    substance = part.GetComponent<container>().substance;
                    return substance;
                }
            }
        }
        return null;
    }

    List<RocketPart> GetEngines()
    {
        List<RocketPart> engines = new List<RocketPart>();
        foreach (RocketPart part in Parts)
        {
            if (part._partType == "engine")
            {
                if (part.GetComponent<Engine>().active == true)
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
        foreach (RocketPart engine in Engines)
        {
            massFlowRate += engine.GetComponent<Engine>()._rate;
        }
        return massFlowRate;
    }

}
