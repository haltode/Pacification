using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : Unit
{
    protected int defaultHP;
    protected int maxHP;
    protected int upgradeHP;
    protected int defaultATK;
    protected int upgradeATK;
    protected int range;
    protected int level;

    private Dictionary<Unit.UnitType, float> dmgMult = new Dictionary<UnitType, float>();

    public int DefaultHP
    {
        get { return defaultHP; }
        set { defaultHP = value; }
    }

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

    // A voir avec l'équilibrage si on veut que l'attaquant prenne des dégats ou non
    public void Attack(ref Unit target)
    {
        float multiplier = 1f;
        dmgMult.TryGetValue(target.Type, out multiplier);
        target.Hp -= (int)((float)((defaultATK - upgradeATK) + upgradeATK * level) * multiplier);

        if (target.Hp <= 0)
        {
            target.Owner.RemoveUnit(target);
            Destroy(target);
        }
    }
    /*
    public void Attack(ref City target)
    {
        target.Hp -= ((defaultATK - upgradeATK) + upgradeATK * level);

        if (target.Hp <= 0)
        {
            target.Owner.RemoveCity(target);
            Destroy(target);
        }
    }
    */
}
