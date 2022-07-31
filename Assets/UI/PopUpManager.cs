using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PopUpManager : MonoBehaviour
{
    public GameObject refObj;
    public GameObject GameManager;
    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.FindGameObjectWithTag("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void dismiss()
    {
        GameManager.GetComponent<GameManager>().panel.active = true;
        Destroy(refObj);   
    }
}
