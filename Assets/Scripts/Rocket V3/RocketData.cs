using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketData
{
    public string rocketName;
    public List<string> lineNames = new List<string>();
    public List<Guid> lineGuids = new List<Guid>();
    public string state;
    public float x_pos;
    public float y_pos;
    public float z_rot;
    public double v_x;
    public double v_y;
    public double curr_X;
    public double curr_Y;
    public double prev_X;
    public double prev_Y;
    public KeplerParams keplerParams = new KeplerParams();
    public double Ho;
    public double H;
    public double Mo;
    public double n;
    public double e;
    public double a;
    public double i;
    public double lastUpdatedTime;
    public double startTime;
    public string planetName;
    public PartData rootPart;
    public stageData stageData = new stageData();
}
