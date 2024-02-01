using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class autoSpritePositionner : MonoBehaviour
{
    public GameObject topAttach;
    public GameObject bottomAttach;

    public GameObject turbine;
    public GameObject pump;
    public GameObject nozzle;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        float yMaxTurbine = turbine.GetComponent<SpriteRenderer>().bounds.max.y - turbine.GetComponent<SpriteRenderer>().bounds.min.y;
        turbine.transform.position = topAttach.transform.position - new Vector3(0, yMaxTurbine/2, 0);

        float yMaxPump = pump.GetComponent<SpriteRenderer>().bounds.max.y - pump.GetComponent<SpriteRenderer>().bounds.min.y;
        pump.transform.position = turbine.transform.position - new Vector3(0, yMaxPump, 0);

        float yMaxNozzle = nozzle.GetComponent<SpriteRenderer>().bounds.max.y - nozzle.GetComponent<SpriteRenderer>().bounds.min.y;
        nozzle.transform.position = pump.transform.position - new Vector3(0, yMaxNozzle/2 + yMaxPump/2, 0);

        bottomAttach.transform.position = nozzle.transform.position - new Vector3(0, yMaxNozzle/2, 0);
    }
}
