using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : Attacker
{
    public Ranged(Player owner)
    {
        this.owner = owner;
        type = UnitType.REGULAR;
        this.level = owner.UnitLevel;

        upgradeHP = 25;
        hp = (isUpgraded() ? 400 : 100) - upgradeHP + (level * upgradeHP);
        maxHP = hp;

        upgradeATK = 20;
        defaultATK = (isUpgraded() ? 300 : 80) - upgradeATK + (level * upgradeATK);

        range = 2;
        mvtSPD = 2;

        dmgMultCity = (isUpgraded() ? 0.5f : 0.25f);
        dmgMult = new Dictionary<UnitType, float>()
        {
            { UnitType.SETTLER, (isUpgraded() ? 2f : 1.5f) },
            { UnitType.WORKER, (isUpgraded() ? 2f : 1.5f) },
            { UnitType.REGULAR, (isUpgraded() ? 2f : 1.5f) },
            { UnitType.RANGED, (isUpgraded() ? 2f : 1.5f) },
            { UnitType.HEAVY, (isUpgraded() ? 0.5f : 0.25f) },
        };

        id = owner.AddUnit(this);
    }

    public void LevelUp()
    {
        if (isMaxed())
        {
            return;
        }
        else if (level == 10)
        {
            hp = 400;
            maxHP = 400;

            defaultATK = 300;

            dmgMultCity = 0.5f;
            dmgMult = new Dictionary<UnitType, float>()
            {
                { UnitType.SETTLER, 2f },
                { UnitType.WORKER, 2f },
                { UnitType.REGULAR, 2f },
                { UnitType.RANGED, 2f },
                { UnitType.HEAVY, 0.5f },
            };
        }
        else
        {
            maxHP += upgradeHP;
            hp += upgradeHP;
            defaultATK += upgradeATK;
        }

        level++;
    }
}