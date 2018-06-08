using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public const int ScienceMinLevelEmbark = 150;

    protected HexUnit hexUnit;
    public GameObject hexGameObject;
    public UnitsAnimator anim;
    protected int level = 1;
    public int maxLevel = 1;

    public enum UnitType
    {
        SETTLER,
        WORKER,
        REGULAR,
        RANGED,
        HEAVY
    }

    public Player Owner;
    protected UnitType type;
    protected int mvtSPD;
    protected int hp;
    protected int maxHp;
    public bool embark;
    public bool hasMadeAction;
    public int currMVT;

    public UnitType Type
    {
        get { return type; }
    }

    public int MaxHP
    {
        get { return maxHp; }
        set { maxHp = value; }
    }

    public int MvtSPD
    {
        get { return mvtSPD; }
        set { mvtSPD = value; }
    }

    public int Hp
    {
        get { return hp; }
        set { hp = value; }
    }

    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public HexUnit HexUnit
    {
        get { return hexUnit; }
        set { hexUnit = value; }
    }

    public bool CanEmbark
    {
        get { return Owner.science >= ScienceMinLevelEmbark; }
    }

    public static bool CanAttack(UnitType type)
    {
        return (type == UnitType.REGULAR || type == UnitType.RANGED || type == UnitType.HEAVY);
    }

    public static UnitType StrToType(string type)
    {
        if(type == "settler")
            return UnitType.SETTLER;
        else if(type == "worker")
            return UnitType.WORKER;
        else if(type == "regular")
            return UnitType.REGULAR;
        else if(type == "ranged")
            return UnitType.RANGED;
        else
            return UnitType.HEAVY;
    }

    public string TypeToStr()
    {
        if(type == UnitType.SETTLER)
            return "Settler";
        else if(type == UnitType.WORKER)
            return "Worker";
        else if(type == UnitType.REGULAR)
            return "Regular";
        else if(type == UnitType.RANGED)
            return "Ranged";
        else
            return "Heavy";
    }

    public void SetGraphics(GameObject prefab)
    {
        float prevOrientation = 0f;
        if(hexGameObject != null)
            prevOrientation = hexUnit.Orientation;

        GameObject graphicsObject = null;

        for(int i = 0; i < hexGameObject.transform.childCount; ++i)
            if(hexGameObject.transform.GetChild(i).name == "Graphics")
                graphicsObject = hexGameObject.transform.GetChild(i).gameObject;

        if(graphicsObject != null)
            Object.Destroy(graphicsObject);

        Vector3 spawnPos = hexGameObject.transform.position + new Vector3(0, 1, 0);
        Quaternion orientation = Quaternion.Euler(0f, prevOrientation, 0f);

        GameObject instantiated = Object.Instantiate(prefab, spawnPos, orientation);
        instantiated.name = "Graphics";
        instantiated.transform.parent = hexGameObject.transform;

        anim = hexGameObject.AddComponent<UnitsAnimator>();
        Object.FindObjectOfType<UnitsParticlesColor>().SetColor(Owner.color);
    }

    public void Update()
    {
        hasMadeAction = false;
        currMVT = 0;
    }
}