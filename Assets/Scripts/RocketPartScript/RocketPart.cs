using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPart : MonoBehaviour
{
    [field: SerializeField] public string _partType{get; protected set;}

    public int _partID{get; set;}

    public float _partMass{get; set;}

    public UnityEngine.Vector2 _position{get; set;}

    [field: SerializeField] public GameObject _attachTop{get; set;}
    [field: SerializeField] public GameObject _attachBottom{get; set;}
    [field: SerializeField] public GameObject _attachRight{get; set;}
    [field: SerializeField] public GameObject _attachLeft{get; set;}
}
