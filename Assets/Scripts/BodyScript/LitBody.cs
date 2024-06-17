using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class LitBody : MonoBehaviour
{
    [SerializeField] Transform light;
    [SerializeField] Material mat;
    GameObject child;
    [SerializeField] Material atmoMat;
    [SerializeField] Vector3 scaledLightPos;
    [SerializeField] Vector3 roundedLightPos;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -transform.localScale.x/50);

        light = GameObject.Find("Sun").GetComponent<Transform>();
        
        mat = GetComponent<Renderer>().sharedMaterial;
        child = transform.GetChild(0).gameObject;

        if(child != null)
        {
            atmoMat = child.GetComponent<Renderer>().sharedMaterial;
            float atmoSize = (float)(GetComponent<EarthScript>().SolarSystemManager.earthRadius * 2 * 1.5 * 0.1); 
            child.transform.localScale = new Vector3(atmoSize, atmoSize, atmoSize);
        }
            
        else Debug.Log("Atmosphere not found. Ignoring...");
    }

    // Update is called once per frame
    void Update()
    {
        scaledLightPos = RoundVector3(light.position / 100000, 4); 
        roundedLightPos = RoundVector3(scaledLightPos, 4);

        mat.SetVector("_LightPos", -roundedLightPos);

        if (atmoMat != null)
            atmoMat.SetVector("_LightPos", -roundedLightPos);
        else Debug.Log("Atmosphere material not found. Ignoring...");
    }

    Vector3 RoundVector3(Vector3 v, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10, decimalPlaces);
        v.x = Mathf.Round(v.x * multiplier) / multiplier;
        v.y = Mathf.Round(v.y * multiplier) / multiplier;
        v.z = Mathf.Round(v.z * multiplier) / multiplier;
        return v;
    }
}
