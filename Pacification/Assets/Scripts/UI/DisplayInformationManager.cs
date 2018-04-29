using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInformationManager : MonoBehaviour {


    public Text money;
    public Text science;
    public Text production;
    public Text happiness;

    void Start()
    {
        money.text = "Money: 0";
        science.text = "Science: 0";
        production.text = "Production: 0";
        happiness.text = "Happiness: 0";
    }

    public void UpdateMoneyDisplay(int value)
    {
        money.text = "Money: " + value;
    }

    public void UpdateScienceDisplay(int value)
    {
        science.text = "Science: " + value;
    }

    public void UpdateProductionDisplay(int value)
    {
        production.text = "Production: " + value;
    }

    public void UpdateHappinessDisplay(int value)
    {
        happiness.text = "Happiness: " + value;
    }
}
