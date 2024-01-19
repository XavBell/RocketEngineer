using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TVC", menuName = "ScriptableObjects/TVC")]
public class TVC:ScriptableObject
{
    public string TVCName = "default";
    public float maxAngle = 0.0f;
    public float speed = 0.0f;
    public float mass = 0f;
    public float cost = 0f;
}
