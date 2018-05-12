using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInformationManager : MonoBehaviour {


    public Text money;
    public Text science;
    public Text production;
    public Text happiness;
    public Text nbRound;

    public Transform downPanel;
    public Transform upEditPanel;
    public Transform upGamePanel;

    public GameObject editorLeftPanel;
    public GameObject editorRightPanel;
    public GameObject loadingPanel;

    void Start()
    {
        money.text = "Money: 0";
        science.text = "Science: 0";
        production.text = "Production: 0";
        happiness.text = "Happiness: 0";

        if(GameManager.Instance.gamemode == GameManager.Gamemode.EDITOR)
        {
            foreach(Transform t in downPanel)
                t.gameObject.SetActive(false);

            foreach(Transform t in upGamePanel)
                t.gameObject.SetActive(false);

            editorLeftPanel.SetActive(true);
            editorRightPanel.SetActive(true);

            Shader.EnableKeyword("HEX_MAP_EDITOR");
        }
        else
        {
            foreach(Transform t in upEditPanel)
                t.gameObject.SetActive(false);

            editorLeftPanel.SetActive(false);
            editorRightPanel.SetActive(false);

            Shader.DisableKeyword("HEX_MAP_EDITOR");
        }
    }

    public void KillLoading()
    {
        loadingPanel.SetActive(false);
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

    public void UpdateRoundDisplay(int value)
    {
        nbRound.text = "Round  " + value;
    }
}
