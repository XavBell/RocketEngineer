using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//ACTUALLY Occluder
public class occulterManager : MonoBehaviour
{
    public string partType;
    public AttachPointScript refAttachPoint;
    public GameObject occulter;

    public AttachPointScript topAttach;
    public AttachPointScript bottomAttach;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(partType == "tank")
        {
            hideTank();
        }

        if(partType == "engine")
        {
            hideEngine();
        }
    }

    void hideTank()
    {
        if(refAttachPoint.attachedBody != null)
        {
            if((refAttachPoint.attachedBody.GetComponent<RocketPart>()._partType == "tank" && refAttachPoint.attachedBody.GetComponent<SpriteRenderer>().size.x >= refAttachPoint.referenceBody.GetComponent<SpriteRenderer>().size.x) || (refAttachPoint.attachedBody.GetComponent<RocketPart>()._partType == "engine" && refAttachPoint.attachedBody.GetComponent<RocketPart>()._attachBottom.GetComponent<AttachPointScript>().attachedBody != null))
            {
                occulter.SetActive(true);
            }else{
                occulter.SetActive(false);
            }
        }else{
            occulter.SetActive(false);
        }
    }

    void hideEngine()
    {
        if(bottomAttach.attachedBody != null && topAttach.attachedBody != null)
        {
            occulter.SetActive(true);
            if(topAttach.attachedBody.GetComponent<RocketPart>()._partType == "tank")
            {
                float xSize = 0;
                if(SceneManager.GetActiveScene().name == "Building")
                {
                    xSize = topAttach.attachedBody.GetComponent<SpriteRenderer>().size.x;
                }

                if(SceneManager.GetActiveScene().name == "SampleScene")
                {
                    xSize = topAttach.attachedBody.transform.localScale.x;
                }
                occulter.transform.localScale = new Vector2(2*xSize, occulter.transform.localScale.y);
            }
        }else{
            occulter.SetActive(false);
        }
    }
}
