using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsPart : MonoBehaviour
{
    public string type;
    public float mass;
    public bool CanHaveChildren;
    //For snapping purpose
    public BoxCollider2D boxCollider;

}
