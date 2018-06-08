using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrack : MonoBehaviour
{
    GameObject barrackObject;
    HexGameUI gameUI;

    void Start()
    {
        gameUI = FindObjectOfType<HexGameUI>();
        barrackObject = gameObject;
        barrackObject.SetActive(true);
    }

    public void CreateUnitButton(string type)
    {
        City city = gameUI.selectedCity;

        PlayerEconomy.MakePurshase(Unit.StrToType(type), city.Owner);
        city.Owner.displayer.UpdateUpgradePannel();
        city.Owner.displayer.UpdateInformationPannels();
        
        if(city != null && !city.Location.HasUnit)
            city.Owner.AddUnit(Unit.StrToType(type), city.Location);
    }

    public void UpgradeBuilding(string type)
    {
        City city = gameUI.selectedCity;

        CityBuilding.MakeBuild(type, city);
        city.Owner.displayer.UpdateUpgradePannel();
        city.Owner.displayer.UpdateInformationPannels();

        if (city != null)
            city.Build(type);
    }

    public GameObject GetBarrackObject
    {
        get { return barrackObject; }
    }
}
