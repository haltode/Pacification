using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInformationManager : MonoBehaviour {


    public Text money;
    public Text science;
    public Text production;
    public Text happiness;

    public Transform downPannel;
    public Transform upEditPannel;
    public Transform upGamePannel;

    public GameObject editorPannel1;
    public GameObject editorPannel2;

    void Start()
    {
        money.text = "Money: 0";
        science.text = "Science: 0";
        production.text = "Production: 0";
        happiness.text = "Happiness: 0";

        if(GameManager.Instance.gamemode == GameManager.Gamemode.EDITOR)
        {
            foreach(Transform t in downPannel)
                t.gameObject.SetActive(false);

            foreach(Transform t in upGamePannel)
                t.gameObject.SetActive(false);

            editorPannel1.SetActive(true);
            editorPannel2.SetActive(true);

            Shader.EnableKeyword("HEX_MAP_EDITOR");
        }
        else
        {
            foreach(Transform t in upEditPannel)
                t.gameObject.SetActive(false);

            editorPannel1.SetActive(false);
            editorPannel2.SetActive(false);

            Shader.DisableKeyword("HEX_MAP_EDITOR");
        }
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
