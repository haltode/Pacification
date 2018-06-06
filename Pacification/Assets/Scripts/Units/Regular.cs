using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regular : Attacker
{
    public Regular(Player owner)
    {
        this.owner = owner;
        type = UnitType.REGULAR;
        hasMadeAction = false;
        this.level = owner.UnitLevel[0];

        upgradeHP = 20;
        hp = (isUpgraded() ? 420 : 150) - upgradeHP + ((level - (isUpgraded() ? 10 : 0)) * upgradeHP);
        maxHP = hp;

        upgradeATK = 25;
        defaultATK = (isUpgraded() ? 375 : 100) - upgradeATK + ((level - (isUpgraded() ? 10 : 0)) * upgradeATK);

        range = 1;
        mvtSPD = (isUpgraded() ? 3 : 2);

        dmgMultCity = (isUpgraded() ? 0.75f : 0.5f);
        dmgMult = new Dictionary<UnitType, float>()
        {
            { UnitType.SETTLER, 1f },
            { UnitType.WORKER, 1f },
            { UnitType.REGULAR, 1f },
            { UnitType.RANGED, (isUpgraded() ? 1.125f : 1.12f) },
            { UnitType.HEAVY, (isUpgraded() ? 1.5f : 1.25f) },
        };
    }

    public void LevelUp()
    {
        if (isMaxed())
            return;
        /*else if (level == 10)
        {
            this.SetGraphics(hexUnit.Grid.unitPrefab[(int)type + 4]);
            mvtSPD = 3;
            hp = 420;
            maxHP = 420;

            defaultATK = 375;

            dmgMultCity = 0.75f;
            dmgMult = new Dictionary<UnitType, float>()
            {
                { UnitType.SETTLER, 1f },
                { UnitType.WORKER, 1f },
                { UnitType.REGULAR, 1f },
                { UnitType.RANGED, 1.125f },
                { UnitType.HEAVY, 1.5f },
            };
        }*/
        else
        {
            maxHP += upgradeHP;
            hp += upgradeHP;
            defaultATK += upgradeATK;
        }

        level++;
    }
}