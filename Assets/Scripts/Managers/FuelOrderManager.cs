using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using DG.Tweening;

public class FuelOrderManager : MonoBehaviour
{
    [SerializeField]private TMP_InputField quantityText;
    [SerializeField]private TMP_Dropdown substanceDropdown;
    public GameObject selectedDestination;

    public string quantity;
    public string substance;
    public bool started;

    [SerializeField]private Substance kerosene;
    [SerializeField]private Substance LOX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PanelFadeIn(GameObject panel)
    {
        panel.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        panel.GetComponent<RectTransform>().transform.DOScale(1, 0.1f);
    }

    public void selectDestination()
    {
        buildingType[] tanks = FindObjectsOfType<buildingType>();
        foreach(buildingType tank in tanks)
        {
            if(tank.type == "GSEtank")
            {
                tank.selectUI.SetActive(true);
                PanelFadeIn(tank.selectUI);
            }
        } 
    }

    public void addFuel()
    {
        if(quantity != null)
        {
            if(substanceDropdown.value == 0)
            {
                //Kerosene
                float substanceMolarMass = 170f;
                float moles = float.Parse(quantityText.text.ToString())/substanceMolarMass * 1000;
                selectedDestination.GetComponent<container>().substance = kerosene;
                selectedDestination.GetComponent<container>().moles = moles;
                selectedDestination.GetComponent<container>().internalTemperature = selectedDestination.GetComponent<container>().externalTemperature;
            }

            if(substanceDropdown.value == 1)
            {
                //Oxygen
                float substanceMolarMass = 32f;
                float moles = float.Parse(quantityText.text.ToString())/substanceMolarMass * 1000;
                selectedDestination.GetComponent<container>().substance = LOX;
                selectedDestination.GetComponent<container>().moles = moles;
                selectedDestination.GetComponent<container>().internalTemperature = selectedDestination.GetComponent<container>().externalTemperature;
            }
        }
        selectedDestination = null;
    }
}
