using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TankComponent : MonoBehaviour
{
    public string lineName;
    public Guid lineGuid;
    public float _volume;
    public float x_scale;
    public float y_scale;
    public float conductivity = 10;
    public bool tested;

    public void Start()
    {
        container container = this.GetComponent<container>();
        container.tankHeight = this.transform.localScale.y/this.transform.localScale.y;
        container.tankVolume = _volume;
        container.tankThermalConductivity = conductivity;
        container.tankSurfaceArea = x_scale*Mathf.PI*2 + (x_scale*Mathf.PI*y_scale);
        container.tested = tested;

        if(SceneManager.GetActiveScene().name == "Building")
        {
            this.gameObject.layer = 0;
        }
    }
}
