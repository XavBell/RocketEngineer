using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class substanceProperty
{
    public void AssignProperty(string substance, out float substanceDensity, out float substanceLiquidTemperature, out float substanceGaseousTemperature, out float substanceSolidTemperature, out float substanceMolarMass, out float substanceSpecificHeatCapacity)
    {
        if(substance == "kerosene")
        {
            substanceDensity = 800f;
            substanceLiquidTemperature = 226f; //up to 424
            substanceGaseousTemperature = 424f; //and more
            substanceSolidTemperature = 226f; //and below
            substanceMolarMass = 170f;
            substanceSpecificHeatCapacity = 2010f;
            return;
        }

        if(substance == "LOX")
        {
            substanceDensity = 1141f;
            substanceLiquidTemperature = 56f; //up to 91
            substanceGaseousTemperature = 91f; //and more
            substanceSolidTemperature = 56f; //and below
            substanceMolarMass = 32f;
            substanceSpecificHeatCapacity = 2010f;
            return;
        }

        //else cry
        substanceDensity = 1141f;
        substanceLiquidTemperature = 56f; //up to 91
        substanceGaseousTemperature = 91f; //and more
        substanceSolidTemperature = 56f; //and below
        substanceMolarMass = 32f;
        substanceSpecificHeatCapacity = 2010f;
    }
}
