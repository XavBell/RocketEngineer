using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saveWorldRocket
{
    public List<saveStage> stages = new List<saveStage>();
    public List<KeplerParams> keplerParams = new List<KeplerParams>();
    public List<string> planetName = new List<string>();
    public List<string> state = new List<string>();
    public List<double> x_pos = new List<double>();
    public List<double> y_pos = new List<double>();
    public List<double> curr_X = new List<double>();
    public List<double> curr_Y = new List<double>();
    public List<double> prev_X = new List<double>();
    public List<double> prev_Y = new List<double>();
    public List<double> v_x = new List<double>();
    public List<double> v_y =  new List<double>();
    public double Ho;
    public double H;
    public double Mo;
    public double n;
    public double e;
    public double a;
    public double i;
    public double lastUpdatedTime;
    public double startTime;
    public Guid coreID;
    
}
