using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heavy : Attacker
{
    public Heavy(Player owner)
    {
        this.owner = owner;
        type = UnitType.HEAVY;
        hasMadeAction = false;
        this.level = owner.UnitLevel[2];
        maxLevel = 20;

        upgradeHP = 20;
        hp = (IsUpgraded() ? 350 : 120) - upgradeHP + ((level - (IsUpgraded() ? 10 : 0)) * upgradeHP);
        MaxHP = hp;

        upgradeATK = 10;
        defaultATK = (IsUpgraded() ? 210 : 100) - upgradeATK + ((level - (IsUpgraded() ? 10 : 0)) * upgradeATK);

        range = (IsUpgraded() ? 3 : 2);
        mvtSPD = 1;
        currMVT = 0;

        dmgMultCity = (IsUpgraded() ? 2f : 1.75f);
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
        if (IsMaxed())
            return;
        /*else if (level == 10)
        {
            this.SetGraphics(hexUnit.Grid.unitPrefab[(int)type + 4]);
            range = 2;
            hp = 350;
            maxHP = 350;

            defaultATK = 210;

            dmgMultCity = 2f;
        }*/
        else
        {
            MaxHP += upgradeHP;
            hp += upgradeHP;
            defaultATK += upgradeATK;
        }

        level++;
    }
}