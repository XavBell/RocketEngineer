using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : RocketPart
{ 
    public List<Nozzle> nozzleReferences = new List<Nozzle>();
    public List<Turbine> turbineReferences = new List<Turbine>();
    public List<Pump> pumpReferences = new List<Pump>();
    public float _thrust;
    public float _rate;
    public float _tvcSpeed;
    public float _maxAngle;

    public string _tvcName;
    public string _nozzleName;
    public string _pumpName;
    public string _turbineName;
    public int stageNumber;
    //Reliability is between 0 and 1
    public float reliability;
    public float maxTime;
    public bool active = false;
    public bool operational = true;

    public bool willFail = false;
    public float timeOfFail;
    public float outReadThrust;
    public bool willExplode = false;

    public void activate()
    {
        active = true;
    }

    void Start()
    {
        InitializeSprite();
        InitializeFail();
    }

    public void InitializeSprite()
    {
        foreach(Nozzle nozzle in nozzleReferences)
        {
            if(nozzle.nozzleName == _nozzleName)
            {
                this.GetComponentInChildren<autoSpritePositionner>().nozzle.GetComponent<SpriteRenderer>().sprite = nozzle.sprite;
            }
        }

        foreach(Pump pump in pumpReferences)
        {
            if(pump.pumpName == _pumpName)
            {
                this.GetComponentInChildren<autoSpritePositionner>().pump.GetComponent<SpriteRenderer>().sprite = pump.sprite;
            }
        }

        foreach(Turbine turbine in turbineReferences)
        {
            if(turbine.turbineName == _turbineName)
            {
                this.GetComponentInChildren<autoSpritePositionner>().turbine.GetComponent<SpriteRenderer>().sprite = turbine.sprite;
            }
        }

          
    }

    public void InitializeFail()
    {
        float percentageOfThrust = Random.Range(reliability, 2-reliability);
        float outThrust = _thrust * percentageOfThrust;
        float minThrust = _thrust * 0.8f;
        float maxThrust = _thrust * 1.2f;
        if(outThrust < minThrust || outThrust > maxThrust)
        {
            willFail = true;
            if(percentageOfThrust < 0.5f || percentageOfThrust > 1.5f)
            {
                willExplode = true;
            }
            timeOfFail = maxTime * percentageOfThrust;
            if(timeOfFail < 2)
            {
                timeOfFail = 2;
            }
        }else{
            willFail = false;
        }
    }

    public float CalculateOutputThrust(float time, out bool fail)
    {
        float percentageOfThrust = Random.Range(0.99f, 1.01f);
        float outThrust = _thrust * percentageOfThrust;
        if(willFail == true && timeOfFail <= time)
        {
            outThrust = 0;
            fail = true;
            if(willExplode == true)
            {
                if(this.gameObject.transform.parent.GetComponent<staticFireStandManager>() != null)
                {
                    this.gameObject.transform.parent.GetComponent<staticFireStandManager>().failed = false;
                    this.gameObject.transform.parent.GetComponent<staticFireStandManager>().started = false;
                    this.gameObject.transform.parent.GetComponent<staticFireStandManager>().stopped = true;
                    
                }
                if(this.gameObject.transform.parent.gameObject.GetComponent<PlanetGravity>() != null)
                {
                    GameObject toDestroy = this.gameObject.transform.parent.gameObject;
                    FindObjectOfType<DestroyPopUpManager>().ShowDestroyPopUp("Destroyed due to engine failure");
                    Destroy(toDestroy);
                }else if(explosion != null){
                    explosion.transform.parent = null;
                    explosion.Play();
                    FindObjectOfType<DestroyPopUpManager>().ShowDestroyPopUp("Destroyed due to engine failure");
                    Destroy(this.gameObject);
                }
            }
        }else if(willFail == false){
            fail = false;
        }else if(willFail == true && timeOfFail >= time){
            fail = false;
        }else{
            fail = false;
        }
        outReadThrust = outThrust;
        return outThrust;
    }
}
