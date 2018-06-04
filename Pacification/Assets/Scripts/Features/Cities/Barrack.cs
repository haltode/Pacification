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
        if(city != null && !city.Location.HasUnit)
            city.Owner.AddUnit(Unit.StrToType(type), city.Location);
    }

    public GameObject GetBarrackObject
    {
        get { return barrackObject; }
    }
}
