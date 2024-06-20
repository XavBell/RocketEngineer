using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowCreator : MonoBehaviour
{
    public GameObject UI;
    public GameObject spawnedUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRightClick()
    {
        if(spawnedUI != null)
        {
            Destroy(spawnedUI);
        }else{
            Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        Vector2 pos = Input.mousePosition;
        spawnedUI = Instantiate(UI, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        if(this.GetComponent<TankComponent>() != null)
        {
            spawnedUI.GetComponentInChildren<TankUIModule>().tank = this.GetComponent<container>();
        }
        spawnedUI.GetComponent<Canvas>().worldCamera = cam;
        spawnedUI.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        }
        
    }

}
