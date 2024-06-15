using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dropDownManager : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public TankComponent tank;
    public RocketController rocketController;
    
    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Building")
        {
            dropdown.gameObject.SetActive(true);
            fetchLines();
            
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
        tank.lineName = dropdown.options[dropdown.value].text;
        tank.lineGuid = rocketController.lineGuids[rocketController.lineNames.IndexOf(tank.lineName)];
        this.gameObject.SetActive(false);
    }

    public void fetchLines()
    {
        dropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (var line in rocketController.lineNames)
        {
            options.Add(line);
        }
        dropdown.AddOptions(options);
    }
}
