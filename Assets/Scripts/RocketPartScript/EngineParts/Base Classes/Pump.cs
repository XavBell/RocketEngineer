using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pump", menuName = "ScriptableObjects/Pump")]
public class Pump:ScriptableObject
{
    public string pumpName = "default";
    public float thrust = 0f;
    public float mass = 0f;
    public float cost = 0f;
    public Sprite sprite;
}
