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
    public GameObject rightGamePanel;



    //UPGRADE
    //////////////////////
    public GameObject upgradePannel;
    public GameObject upgradePannelClose;

    public Text regularLvl;
    public Text regularScience;
    public Text regularRes1;
    public Text regularRes2;
    public Text regularRes3;
    public GameObject regularUpgrade;

    public Text rangedLvl;
    public Text rangedScience;
    public Text rangedRes1;
    public Text rangedRes2;
    public Text rangedRes3;
    public GameObject rangedUpgrade;

    public Text heavyLvl;
    public Text heavyScience;
    public Text heavyRes1;
    public Text heavyRes2;
    public Text heavyRes3;
    public GameObject heavyUpgrade;
    //////////////////////


    //INFORMATION
    //////////////////////
    public GameObject openInformationPannel;
    public GameObject closeInformationPannel;
    public GameObject infoIsActive;
    public Text infoIsEnemy;
    public Text infoType;
    public Text infoLevel;
    public Text infoOwner;
    public Text infoHealth;
    public RectTransform healthUnit;
    public GameObject canAct;
    public GameObject canMove;

    public GameObject openInformation2Pannel;
    public GameObject closeInformation2Pannel;
    public GameObject info2IsActive;
    public Text info2IsEnemy;
    public Text info2Type;
    public Text info2Owner;
    public Text info2Health;
    public RectTransform healthCity;
    //////////////////////


    //CITY
    //////////////////////
    public GameObject cityPanel;
    public GameObject seeMore;
    public GameObject seeLess;

    public GameObject settler;
    public GameObject worker;
    public GameObject regular;
    public GameObject ranged;
    public GameObject heavy;

    public GameObject settlerMORE;
    public GameObject workerMORE;
    public GameObject regularMORE;
    public GameObject rangedMORE;
    public GameObject heavyMORE;

    public GameObject moneyUp;
    public GameObject scienceUp;
    public GameObject productionUp;
    public GameObject happinessUp;

    public GameObject moneyUpMORE;
    public GameObject scienceUpMORE;
    public GameObject productionUpMORE;
    public GameObject happinessUpMORE;

    ///////
    public Text settlerR1;
    public Text settlerR2;
    public Text settlerR3;
    public Text settlerR4;
    public Text workerR1;
    public Text workerR2;
    public Text workerR3;
    public Text workerR4;
    public Text regularR1;
    public Text regularR2;
    public Text regularR3;
    public Text regularR4;
    public Text rangedR1;
    public Text rangedR2;
    public Text rangedR3;
    public Text rangedR4;
    public Text heavyR1;
    public Text heavyR2;
    public Text heavyR3;
    public Text heavyR4;
    public Text moneyR1;
    public Text moneyR2;
    public Text scienceR1;
    public Text scienceR2;
    public Text prodR1;
    public Text prodR2;
    public Text happiR1;
    public Text happiR2;
    //////////////////////

    public Player player;
    HexCell currentcell;

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
            rightGamePanel.SetActive(false);
            closeInformation2Pannel.SetActive(false);
            closeInformationPannel.SetActive(false);

            Shader.EnableKeyword("HEX_MAP_EDITOR");
        }
        else
        {
            foreach(Transform t in upEditPanel)
                t.gameObject.SetActive(false);

            editorLeftPanel.SetActive(false);
            editorRightPanel.SetActive(false);
            rightGamePanel.SetActive(true);
            closeInformation2Pannel.SetActive(true);
            closeInformationPannel.SetActive(true);

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

    public void LevelUp(string type)
    {
        player.LevelUp(Unit.StrToType(type));
        UpdateUpgradePannel();
    }

    //Upgrade pannel
    ////////////////////
    public void OpenUpgradePannel()
    {
        upgradePannelClose.SetActive(false);
        UpdateUpgradePannel();
        upgradePannel.SetActive(true);
    }
    public void CloseUpgradePannel()
    {
        upgradePannel.SetActive(false);
        upgradePannelClose.SetActive(true);
    }
    public void UpdateUpgradePannel()
    {
        regularLvl.text = "" + player.GetUnitLevel(Unit.UnitType.REGULAR) + "/20";
        regularScience.text = "" + (PlayerEconomy.upgradeCosts[0])[player.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 0];
        regularRes1.text = "" + (PlayerEconomy.upgradeCosts[0])[player.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 1]; //Food
        regularRes2.text = "" + (PlayerEconomy.upgradeCosts[0])[player.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 2]; //Iron
        regularRes3.text = "" + (PlayerEconomy.upgradeCosts[0])[player.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 3]; //Horse
        regularUpgrade.SetActive(player.science < 0); //Change the 0 (condition must be false in order to buy)

        rangedLvl.text = "" + player.GetUnitLevel(Unit.UnitType.RANGED) + "/20";
        rangedScience.text = "" + (PlayerEconomy.upgradeCosts[1])[player.GetUnitLevel(Unit.UnitType.RANGED) - 1, 0];
        rangedRes1.text = "" + (PlayerEconomy.upgradeCosts[1])[player.GetUnitLevel(Unit.UnitType.RANGED) - 1, 1]; //Wood
        rangedRes2.text = "" + (PlayerEconomy.upgradeCosts[1])[player.GetUnitLevel(Unit.UnitType.RANGED) - 1, 2]; //Iron
        rangedRes3.text = "" + (PlayerEconomy.upgradeCosts[1])[player.GetUnitLevel(Unit.UnitType.RANGED) - 1, 3]; //Gold
        rangedUpgrade.SetActive(player.science < 0); //Change the 0 (condition must be false in order to buy)

        heavyLvl.text = "" + player.GetUnitLevel(Unit.UnitType.HEAVY) + "/20";
        heavyScience.text = "" + (PlayerEconomy.upgradeCosts[2])[player.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 0];
        heavyRes1.text = "" + (PlayerEconomy.upgradeCosts[2])[player.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 1]; //Wood
        heavyRes2.text = "" + (PlayerEconomy.upgradeCosts[2])[player.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 2]; //Iron
        heavyRes3.text = "" + (PlayerEconomy.upgradeCosts[2])[player.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 3]; //Diams
        heavyUpgrade.SetActive(player.science < 0); //Change the 0 (condition must be false in order to buy)
    }


    //INFORMATION PANNEL
    ////////////////////
    public void OpenInformationPannel()
    {
        closeInformationPannel.SetActive(false);
        openInformationPannel.SetActive(true);
    }
    public void CloseInformationPannel()
    {
        openInformationPannel.SetActive(false);
        closeInformationPannel.SetActive(true);
    }
    public void OpenInformation2Pannel()
    {
        closeInformation2Pannel.SetActive(false);
        openInformation2Pannel.SetActive(true);
    }
    public void CloseInformation2Pannel()
    {
        openInformation2Pannel.SetActive(false);
        closeInformation2Pannel.SetActive(true);
    }

    public void UpdateInformationPannels()
    {
        UpdateInformationPannels(currentcell);
    }

    public void UpdateInformationPannels(HexCell cell)
    {
        currentcell = cell;

        if(cell == null)
            return;

        if(cell.HasUnit)
        {
            Unit unit = cell.Unit.Unit;
            infoIsActive.SetActive(true);
            if(unit.owner == player)
            {
                infoIsEnemy.text = "";
                infoLevel.text = unit.Level + "/" + unit.maxLevel;
                canAct.SetActive(!unit.hasMadeAction);
                //canMove;
            }
            else
            {
                infoIsEnemy.text = "Enemy";
                infoLevel.text = "??/??";
                canAct.SetActive(false);
                canMove.SetActive(false);
            }

            infoType.text = unit.TypeToStr();
            infoOwner.text = unit.owner.name;
            infoHealth.text = unit.Hp + " / " + unit.MaxHP;
            healthUnit.sizeDelta = new Vector2(((float)unit.Hp / (float)unit.MaxHP) * 60f, healthCity.sizeDelta.y);
        }
        else
            infoIsActive.SetActive(false);

        if(cell.HasCity || cell.FeatureIndex > 9)
        {
            Feature feature = cell.Feature;
            info2IsActive.SetActive(true);
            if(feature.Owner == player)
            {
                info2IsEnemy.text = "";
                info2Owner.text = player.name;
            }
            else
            {
                info2IsEnemy.text = "Enemy";
                info2Owner.text = feature.Owner.name;
            }

            info2Type.text = feature.TypeToStr();
            info2Health.text = feature.Hp + " / " + feature.MaxHp;
            healthCity.sizeDelta = new Vector2(((float)feature.Hp / (float)feature.MaxHp) * 60f, healthCity.sizeDelta.y);
        }
        else
            info2IsActive.SetActive(false);


        if(cell.HasCity && cell.Feature.Owner == player)
        {
            City city = (City)cell.Feature;
            productivity.text = "" + city.prodLevel;
            population.text = "" + city.pop;
            happiness.text = "" + (int)city.happiness;
            townResources.SetActive(true);
            cityPanel.SetActive(true);

            bool canbuySettler = true;      //CHANGER TOUT CA EN FONCTION DES PRIX
            bool canbuyWorker = true;       //
            bool canbuyRegular = true;      //
            bool canbuyRanged = true;        //
            bool canbuyHeavy = true;        //
            bool canUpgradeMoney = true;    //
            bool canUpgradeScience = true;  //
            bool canUpgradeProduction = true;//
            bool canUpgradeHapiness = true; //


            settler.SetActive(canbuySettler);
            worker.SetActive(canbuyWorker); ;
            regular.SetActive(canbuyRegular);
            ranged.SetActive(canbuyRanged);
            heavy.SetActive(canbuyHeavy);
            moneyUp.SetActive(canUpgradeMoney);
            scienceUp.SetActive(canUpgradeScience);
            productionUp.SetActive(canUpgradeProduction);
            happinessUp.SetActive(canUpgradeHapiness);
            settlerMORE.SetActive(canbuySettler);
            workerMORE.SetActive(canbuyWorker);
            regularMORE.SetActive(canbuyRegular);
            rangedMORE.SetActive(canbuyRanged);
            heavyMORE.SetActive(canbuyHeavy);
            moneyUpMORE.SetActive(canUpgradeMoney);
            scienceUpMORE.SetActive(canUpgradeScience);
            productionUpMORE.SetActive(canUpgradeProduction);
            happinessUpMORE.SetActive(canUpgradeHapiness);


            settlerR1.text = "0";   //REMPLIR ça en fonction des prix d'achat et d'upgrade batiment
            settlerR2.text = "0";   //R1, R2, R3 les ressources -> R4 money
            settlerR3.text = "0";
            settlerR4.text = "0";
            workerR1.text = "0";
            workerR2.text = "0";
            workerR3.text = "0";
            workerR4.text = "0";
            regularR1.text = "0";
            regularR2.text = "0";
            regularR3.text = "0";
            regularR4.text = "0";
            rangedR1.text = "0";
            rangedR2.text = "0";
            rangedR3.text = "0";
            rangedR4.text = "0";
            heavyR1.text = "0";
            heavyR2.text = "0";
            heavyR3.text = "0";
            heavyR4.text = "0";
            moneyR1.text = "0";
            moneyR2.text = "0";
            scienceR1.text = "0";
            scienceR2.text = "0";
            prodR1.text = "0";
            prodR2.text = "0";
            happiR1.text = "0";
            happiR2.text = "0";

        }
        else
        {
            townResources.SetActive(false);
            cityPanel.SetActive(false);
        }
    }

    public void SeeMoreCityPanel()
    {
        seeLess.SetActive(false);
        seeMore.SetActive(true);
    }
    public void SeeLessCityPanel()
    {
        seeMore.SetActive(false);
        seeLess.SetActive(true);
    }

}
