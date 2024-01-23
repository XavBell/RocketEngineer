using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flowControllerStaticFire : MonoBehaviour
{
    container oxidizerContainer;
    container fuelContainer;
    TimeManager MyTime;
    staticFireStandManager staticFireStandManager;

    // Start is called before the first frame update
    void Start()
    {
        MyTime = FindObjectOfType<TimeManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        CalculateFlowStaticFireEngine(staticFireStandManager.ConnectedEngine.GetComponent<Engine>()._rate, staticFireStandManager.started, staticFireStandManager);
    }

    //Flow of fuel for static fires are managed in this component
    private void CalculateFlowStaticFireEngine(float massFlowRateEngine, bool started, staticFireStandManager sFSM)
    {
        float ratio = 0;
        if(oxidizerContainer.substance.Name == "LOX" && fuelContainer.substance.Name == "kerosene")
        {
            ratio = 2.56f; //RP-1
        }
        //Ratio is always oxidizer/fuel
        if (started == true)
        {
            float percentageOxidizer = ratio / (ratio + 1);
            float oxidizerRate = percentageOxidizer * massFlowRateEngine;
            //Static fire will be able to be ran at timewarp
            float consumedOxidizer = oxidizerRate * MyTime.deltaTime;
            if (oxidizerContainer.mass - consumedOxidizer >= 0)
            {
                sFSM.oxidizerSufficient = true;
                //Multiply by 1000 bcs engine rate is kg
                float consumedMoles = consumedOxidizer * 1000 / oxidizerContainer.substance.MolarMass;
                oxidizerContainer.moles -= consumedMoles;
            }
            else
            {
                sFSM.oxidizerSufficient = false;
            }



            float percentageFuel = 1f / (ratio + 1);
            float fuelRate = percentageFuel * massFlowRateEngine;
            //Static fire will be able to be ran at timewarp
            float consumedFuel = fuelRate * MyTime.deltaTime;
            if (fuelContainer.mass - consumedFuel >= 0)
            {
                sFSM.fuelSufficient = true;
                float consumedMoles = consumedFuel * 1000 / fuelContainer.substance.MolarMass;
                fuelContainer.moles -= consumedMoles;
            }
            else
            {
                sFSM.fuelSufficient = false;
            }

        }
    }
}
