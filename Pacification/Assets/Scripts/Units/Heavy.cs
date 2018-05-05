using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heavy : Attacker
{
    public Heavy(Player owner, int id)
    {
        this.owner = owner;
        this.id = id;
        type = UnitType.REGULAR;
        this.level = owner.UnitLevel;

        upgradeHP = 20;
        hp = (isUpgraded() ? 350 : 120) - upgradeHP + (level * upgradeHP);
        maxHP = hp;

        upgradeATK = 10;
        defaultATK = (isUpgraded() ? 210 : 100) - upgradeATK + (level * upgradeATK);

        range = (isUpgraded() ? 2 : 1);
        mvtSPD = 1;

        dmgMultCity = (isUpgraded() ? 2f : 1.75f);
        dmgMult = new Dictionary<UnitType, float>()
        {
            { UnitType.SETTLER, 0.5f },
            { UnitType.WORKER, 0.5f },
            { UnitType.REGULAR, 0.5f },
            { UnitType.RANGED, 0.5f },
            { UnitType.HEAVY, 1f },
        };
    }

    public void LevelUp()
    {
        if (isMaxed())
            return;
        else if (level == 10)
        {
            range = 2;
            hp = 350;
            maxHP = 350;

            defaultATK = 210;

            dmgMultCity = 2f;
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