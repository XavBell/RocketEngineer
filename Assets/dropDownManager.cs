using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dropDownManager : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public GameObject tank;
    
    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Building")
        {
            dropdown.gameObject.SetActive(true);
            if(tank.GetComponentInChildren<Tank>().propellantCategory == "fuel")
            {
                dropdown.value = 0;
            }

            if(tank.GetComponentInChildren<Tank>().propellantCategory == "oxidizer")
            {
                dropdown.value = 1;
            }
        }else
        {
            dropdown.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnValueChanged()
    {
        if(dropdown.value == 0)
        {
            tank.GetComponentInChildren<Tank>().propellantCategory = "fuel";
        }

        if(dropdown.value == 1)
        {
            tank.GetComponentInChildren<Tank>().propellantCategory = "oxidizer";
        }

        this.gameObject.SetActive(false);
    }
}
