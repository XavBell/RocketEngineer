using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class FuelOrderManager : MonoBehaviour
{
    [SerializeField]private TMP_InputField quantityText;
    [SerializeField]private TMP_Dropdown substanceDropdown;
    [SerializeField]private Slider temperatureSlider;
    [SerializeField]private TMP_Text temperatureText;
    [SerializeField]private GameObject  liquidIcon;
    [SerializeField]private GameObject  gasIcon;
    [SerializeField]private GameObject  solidIcon;
    public GameObject selectedDestination;

    public string quantity;
    public string substance;
    public bool started;

    [SerializeField]private Substance kerosene;
    [SerializeField]private Substance LOX;

    // Start is called before the first frame update
    void Start()
    {
        updateTemperatureText();
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
                float substanceMolarMass = kerosene.MolarMass;
                float moles = float.Parse(quantityText.text.ToString())/substanceMolarMass * 1000;
                selectedDestination.GetComponent<container>().substance = kerosene;
                selectedDestination.GetComponent<container>().moles = moles;
                selectedDestination.GetComponent<container>().internalTemperature = temperatureSlider.value;
            }

            if(substanceDropdown.value == 1)
            {
                //Oxygen
                float substanceMolarMass = LOX.MolarMass;
                float moles = float.Parse(quantityText.text.ToString())/substanceMolarMass * 1000;
                selectedDestination.GetComponent<container>().substance = LOX;
                selectedDestination.GetComponent<container>().moles = moles;
                selectedDestination.GetComponent<container>().internalTemperature = temperatureSlider.value;
            }
        }
        selectedDestination = null;
    }

    public void updateTemperatureText()
    {
        temperatureText.text = temperatureSlider.value.ToString() + "K";

        if(substanceDropdown.value == 0)
        {
            //Kerosene
            if(temperatureSlider.value <= kerosene.SolidTemperature)
            {
                liquidIcon.SetActive(false);
                gasIcon.SetActive(false);
                solidIcon.SetActive(true);
            }

            if(temperatureSlider.value > kerosene.SolidTemperature && temperatureSlider.value < kerosene.GaseousTemperature)
            {
                liquidIcon.SetActive(true);
                gasIcon.SetActive(false);
                solidIcon.SetActive(false);
            }

            if(temperatureSlider.value >= kerosene.GaseousTemperature)
            {
                liquidIcon.SetActive(false);
                gasIcon.SetActive(true);
                solidIcon.SetActive(false);
            }
        }

        if(substanceDropdown.value == 1)
        {
            //Kerosene
            if(temperatureSlider.value <= LOX.SolidTemperature)
            {
                liquidIcon.SetActive(false);
                gasIcon.SetActive(false);
                solidIcon.SetActive(true);
            }

            if(temperatureSlider.value > LOX.SolidTemperature && temperatureSlider.value < LOX.GaseousTemperature)
            {
                liquidIcon.SetActive(true);
                gasIcon.SetActive(false);
                solidIcon.SetActive(false);
            }

            if(temperatureSlider.value >= LOX.GaseousTemperature)
            {
                liquidIcon.SetActive(false);
                gasIcon.SetActive(true);
                solidIcon.SetActive(false);
            }
        }
    }
}

