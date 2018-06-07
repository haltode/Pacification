using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInformationManager : MonoBehaviour {

    public Text nbRound;

    public Text money;
    public Text science;
    public Text food;
    public Text wood;
    public Text horse;
    public Text iron;
    public Text gold;
    public Text diamond;

    public GameObject townResources;
    public Text productivity;
    public Text population;
    public Text happiness;


    public Transform downPanel;
    public Transform upEditPanel;
    public Transform upGamePanel;

    public GameObject editorLeftPanel;
    public GameObject editorRightPanel;
    public GameObject loadingPanel;

    public Player player;

    void Start()
    {
        InitiateResources();

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

    private void Update()
    {
        if(player != null && player.canPlay)
            DisplayResources();
    }

    public void UpdateRoundDisplay(int rounds)
    {
        nbRound.text = "" + rounds;
    }

    public void DisplayResources()
    {
        money.text = "" + player.money;
        science.text = "" + player.science;
        food.text = "" + player.resources[5];
        wood.text = "" + player.resources[4];
        horse.text = "" + player.resources[3];
        iron.text = "" + player.resources[0];
        gold.text = "" + player.resources[1];
        diamond.text = "" + player.resources[2];
    }

    public void DisplayTownResources(string productivityC, string populationC, string happinessC)
    {
        productivity.text = productivityC;
        population.text = populationC;
        happiness.text = happinessC;
        townResources.SetActive(true);
    }

    public void HideTownResources()
    {
        townResources.SetActive(false);
    }

    void InitiateResources()
    {
        money.text = "0";
        science.text = "0";
        food.text = "0";
        wood.text = "0";
        horse.text = "0";
        iron.text = "0";
        gold.text = "0";
        diamond.text = "0";

        townResources.SetActive(false);
        productivity.text = "0";
        population.text = "0";
        happiness.text = "0";
}

    public void KillLoading()
    {
        loadingPanel.SetActive(false);
    }
}
