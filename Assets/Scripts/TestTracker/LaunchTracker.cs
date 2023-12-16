using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchTracker
{
    public List<float> times = new List<float>();
    public List<List<float>> Pressure = new List<List<float>>();
    public List<List<float>> Quantity = new List<List<float>>(); //Mass
    public List<List<float>> Volume = new List<List<float>>();
    public List<List<string>> state = new List<List<string>>(); //substance state

}
