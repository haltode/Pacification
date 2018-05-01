using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrack : MonoBehaviour
{
    GameObject BarrackObject;
    HexGameUI gameUI;

    void Start()
    {
        gameUI = FindObjectOfType<HexGameUI>();
        //BarrackObject.SetActive(false);
    }

    public void CreateUnitButton(string type)
    {
        City city = gameUI.selectedCity;
        if(city != null)
            city.Owner.AddUnit(Unit.StrToType(type), city.Position);
    }
}
