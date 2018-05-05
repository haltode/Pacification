using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : Unit
{
    protected int maxHP;
    protected int upgradeHP;
    protected int defaultATK;
    protected int upgradeATK;
    protected int range;
    protected int level;

    protected Dictionary<Unit.UnitType, float> dmgMult;
    protected float dmgMultCity;

    public int MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }

    public int UpgradeHP
    {
        get { return upgradeHP; }
        set { upgradeHP = value; }
    }

    public int DefaultATK
    {
        get { return defaultATK; }
        set { defaultATK = value; }
    }

    public int UpgradeATK
    {
        get { return upgradeATK; }
        set { upgradeATK = value; }
    }

    public int Range
    {
        get { return range; }
        set { range = value; }
    }

    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public bool isUpgraded()
    {
        return level > 10;
    }

    public bool isMaxed()
    {
        return level == 20;
    }

    public bool IsInRangeToAttack(HexCell target)
    {
        return HexUnit.location.coordinates.DistanceTo(target.coordinates) <= range;
    }

    // A voir avec l'équilibrage si on veut que l'attaquant prenne des dégats ou non
    public void Attack(Unit target)
    {
        float multiplier = 1f;
        dmgMult.TryGetValue(target.Type, out multiplier);
        int damage = (int)((float)((defaultATK - upgradeATK) + upgradeATK * level) * multiplier);

        owner.client.Send("CUNM|UTD|" + target.HexUnit.location.coordinates.X + "#" + target.HexUnit.location.coordinates.Z + "#" + damage + "|" + target.owner.name);
    }

    public void Attack(City target)
    {
        int damage = 100; //FIX THIS

        owner.client.Send("CUNM|CTD|" + target.position.coordinates.X + "#" + target.position.coordinates.Z + "#" + damage + "|" + target.owner.name);
    }
    
}