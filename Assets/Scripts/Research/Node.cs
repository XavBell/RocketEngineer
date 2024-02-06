using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Node", menuName = "ScriptableObjects/Node")]
public class Node : ScriptableObject
{
    public string NodeName;
    public List<Node> Dependencies;

    public List<Turbine> unlockedTurbine;
    public List<Pump> unlockedPump;
    public List<Nozzle> unlockedNozzle;
    public List<TVC> unlockedTVC;
}
