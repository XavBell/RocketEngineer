using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Prediction : MonoBehaviour
{
    public PlanetGravity planetGravity;
    public GameObject WorldSaveManager;
    public MasterManager MasterManager;
    public GameObject MastRef;
    public LineRenderer line;

    public GameObject planet;
    public Rigidbody2D rb;
    public float G;
    public float rocketMass;
    public float Mass;
    
    // Start is called before the first frame update
    void Start()
    {
        WorldSaveManager = GameObject.FindGameObjectWithTag("WorldSaveManager");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(MasterManager == null)
        {
            GameObject MastRef = GameObject.FindGameObjectWithTag("MasterManager");
            if(MastRef)
            {
                MasterManager = MastRef.GetComponent<MasterManager>();
            }
        }

        if(MasterManager != null && MasterManager.ActiveRocket != null)
        {
            planetGravity = MasterManager.ActiveRocket.GetComponent<PlanetGravity>();
            rb = planetGravity.rb;
            G = planetGravity.G;
            rocketMass = planetGravity.rocketMass;
        }

        if(planetGravity != null)
        {
            planet = planetGravity.planet;
            Mass = planetGravity.Mass;
            Vector2 currentPos = rb.position;
            Vector2 prevPos = currentPos;
            Vector2 currentVelocity = rb.velocity;
            Vector2 planetCords = planet.transform.position;
            int stepCount = 10000;
            line.positionCount = stepCount;
            List<float> distances = new List<float>();
            float min = Mathf.Infinity;
            line.SetPosition(0, currentPos);
            for (int i = 1; i < stepCount; i++)
            {
                float distance = Vector2.Distance(currentPos, planet.transform.position);
                Vector2 forceDir = (planetCords - currentPos).normalized;
                Vector2 ForceVector =  (forceDir * (G * Mass * rocketMass/ (distance * distance)));
                Vector2 acceleration = ForceVector/rb.mass * Time.fixedDeltaTime;
                currentVelocity += acceleration * Time.fixedDeltaTime * 5f;
                currentPos += currentVelocity * Time.fixedDeltaTime * 5f;
                prevPos = currentPos;
                line.SetPosition(i, prevPos);
            }
            
        }


    }
}