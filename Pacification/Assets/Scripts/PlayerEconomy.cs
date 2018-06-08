using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEconomy
{
    // NOTES:
    // Ville lv2 (donc colon et archer) dispo à partir du tour 50 sans amélioration
    // Ville lv3 (donc catapulte) dispo à partir du tour 160 sans amélioration

    public static List<string[,]> upgradeCosts = new List<string[,]>
    {
        // [ unit level - 1 , resource ID (cf Post-It ou commentaire) ]

        {new string[,] //Regular //science, food, steel, horse
            {
                {"5", "250", "50", "0"},       //lv1  -> 2
                {"20", "250", "50", "0"},      //lv2  -> 3
                {"50", "250", "50", "0"},      //lv3  -> 4
                {"100", "250", "100", "0"},     //lv4  -> 5 //Player can build more cities from this point on
                {"200", "500", "100", "0"},     //lv5  -> 6
                {"300", "500", "100", "0"},     //lv6  -> 7
                {"400", "500", "200", "0"},     //lv7  -> 8
                {"500", "500", "200", "0"},     //lv8  -> 9 
                {"650", "750", "300", "0"},     //lv9  -> 10
                {"1000", "2500", "1000", "75"}, //lv10 -> 11 //(Era change !) Science: ~120 turns with a single non-upgraded city
                {"1500", "2000", "500", "12"},  //lv11 -> 12
                {"2500", "2000", "500", "13"},  //lv12 -> 13
                {"3700", "2000", "500", "14"}, //lv13 -> 14
                {"5000", "2000", "500", "15"}, //lv14 -> 15
                {"7000", "2500", "500", "16"}, //lv15 -> 16
                {"9000", "2500", "1000", "17"}, //lv16 -> 17
                {"11000", "2500", "1000", "18"},//lv17 -> 18
                {"14000", "2500", "1000", "19"},//lv18 -> 19
                {"20000", "5000", "2500", "20"},//lv19 -> 20 //Science: ~400 turns with a single non-upgraded city
                {"0", "0", "0", "0"}            //All upgrades finished
            }
        },
        {new string[,] //Ranged //science, wood, steel, gold
            {
                {"200", "250", "250", "0"},         //lv1 -> 2 //Player will have at least 122 science points before being able to build them or more cities. Turn ~50 minimum.
                {"500", "250", "250", "0"},         //lv2 -> 3
                {"800", "250", "250", "0"},         //lv3 -> 4
                {"1200", "250", "250", "0"},        //lv4 -> 5
                {"1800", "500", "250", "0"},        //lv5 -> 6
                {"2500", "500", "500", "0"},        //lv6 -> 7
                {"3500", "500", "500", "0"},        //lv7 -> 8
                {"5000", "500", "500", "0"},        //lv8 -> 9
                {"7000", "500", "500", "0"},        //lv9 -> 10
                {"10000", "3000", "2500", "10000"}, //lv10 -> 11 (Era change !) //Science: ~300 turns with a single non-upgraded city
                {"12000", "1000", "500", "1200"},   //lv11 -> 12
                {"15000", "1000", "1000", "1300"},  //lv12 -> 13
                {"18000", "1000", "1000", "1400"},  //lv13 -> 14
                {"22000", "1000", "1000", "1500"},  //lv14 -> 15
                {"26000", "2500", "1000", "1600"},  //lv15 -> 16
                {"33000", "1000", "1000", "1700"},  //lv16 -> 17
                {"39500", "1000", "1000", "1800"},  //lv17 -> 18
                {"46800", "1000", "1000", "1900"},  //lv18 -> 19
                {"60000", "6000", "1000", "2000"},  //lv19 -> 20 //Science: ~600 turns with a single non-upgraded city
                {"0", "0", "0", "0"}                //All upgrades finished
            }
        },
        {new string[,] //Heavy //science, wood, steel, diamond
            {
                {"3000", "500", "250", "0"},        //lv1 -> 2 //Player will have at least 2140 science points before being able to build them. Turn ~160 minimum.
                {"4200", "500", "250", "0"},        //lv2 -> 3
                {"5800", "500", "250", "0"},        //lv3 -> 4
                {"7500", "500", "250", "0"},        //lv4 -> 5
                {"10000", "500", "500", "0"},       //lv5 -> 6
                {"13000", "1000", "500", "0"},      //lv6 -> 7
                {"17000", "1000", "500", "0"},      //lv7 -> 8
                {"22000", "1000", "500", "0"},      //lv8 -> 9
                {"28000", "1000", "500", "0"},      //lv9 -> 10
                {"35000", "5000", "3500", "300"},   //lv10 -> 11 (Era change !) Science: ~500 turns with a single non-upgraded city
                {"47000", "1000", "1000", "20"},    //lv11 -> 12
                {"60000", "1000", "1000", "30"},    //lv12 -> 13
                {"80000", "1000", "1000", "40"},    //lv13 -> 14
                {"105000", "1000", "1000", "50"},   //lv14 -> 15
                {"136000", "2500", "1000", "60"},   //lv15 -> 16
                {"175000", "2500", "1000", "70"},   //lv16 -> 17
                {"240000", "2500", "1000", "80"},   //lv17 -> 18
                {"340000", "2500", "1000", "90"},   //lv18 -> 19
                {"500000", "7500", "1000", "100"},  //lv19 -> 20 Science: ~1000 turns with a single non-upgraded city
                {"0", "0", "0", "0"}                //All upgrades finished
            }
        }
    };



    public static List<string[,]> unitCosts = new List<string[,]>
    {
        // [ unit level - 1, resource ID (cf Post-It ou commentaire) ]

        {new string[,] //Regular //money, food, steel, horse
            {
                {"100", "50", "25", "0"},   //lv1
                {"105", "50", "25", "0"},   //lv2
                {"110", "50", "25", "0"},   //lv3
                {"115", "50", "25", "0"},   //lv4
                {"120", "50", "25", "0"},   //lv5
                {"125", "50", "50", "0"},   //lv6
                {"130", "50", "50", "0"},   //lv7
                {"135", "50", "50", "0"},   //lv8 
                {"140", "50", "50", "0"},   //lv9
                {"150", "50", "50", "0"},   //lv10
                {"200", "100", "100", "1"}, //lv11
                {"210", "100", "100", "2"}, //lv12
                {"220", "100", "100", "3"}, //lv13
                {"230", "100", "100", "3"}, //lv14
                {"240", "100", "100", "3"}, //lv15
                {"250", "100", "150", "5"}, //lv16
                {"270", "100", "150", "7"}, //lv17
                {"290", "100", "150", "9"}, //lv18
                {"310", "100", "150", "12"},//lv19
                {"350", "150", "150", "15"},//lv20
            }
        },
        {new string[,] //Ranged //money, wood, steel, gold
            {
                {"200", "180", "40", "0"},   //lv1
                {"205", "180", "40", "0"},   //lv2
                {"210", "180", "40", "0"},   //lv3
                {"215", "180", "40", "0"},   //lv4
                {"220", "180", "40", "0"},   //lv5
                {"225", "200", "70", "0"},   //lv6
                {"230", "200", "70", "0"},   //lv7
                {"235", "200", "70", "0"},   //lv8 
                {"240", "200", "70", "0"},   //lv9
                {"250", "200", "70", "0"},   //lv10
                {"280", "230", "110", "75"}, //lv11
                {"290", "230", "110", "80"}, //lv12
                {"300", "230", "110", "85"}, //lv13
                {"310", "230", "110", "90"}, //lv14
                {"320", "230", "110", "95"}, //lv15
                {"330", "250", "170", "100"},//lv16
                {"340", "250", "170", "110"},//lv17
                {"350", "250", "170", "120"},//lv18
                {"360", "250", "170", "130"},//lv19
                {"400", "250", "170", "150"},//lv20
            }
        },
        {new string[,] //Heavy //money, wood, steel, diamond
            {
                {"310", "200", "25", "0"},  //lv1
                {"320", "200", "25", "0"},  //lv2
                {"330", "200", "25", "0"},  //lv3
                {"340", "200", "25", "0"},  //lv4
                {"350", "200", "25", "0"},  //lv5
                {"360", "300", "50", "0"},  //lv6
                {"370", "300", "50", "0"},  //lv7
                {"380", "300", "50", "0"},  //lv8 
                {"390", "300", "50", "0"},  //lv9
                {"400", "300", "50", "0"},  //lv10
                {"600", "100", "200", "5"}, //lv11
                {"610", "100", "200", "5"}, //lv12
                {"620", "100", "200", "5"}, //lv13
                {"630", "100", "200", "7"}, //lv14
                {"640", "100", "200", "7"}, //lv15
                {"650", "150", "300", "7"}, //lv16
                {"666", "150", "300", "9"}, //lv17
                {"680", "150", "300", "9"}, //lv18
                {"700", "150", "300", "9"}, //lv19   
                {"750", "200", "300", "10"},//lv20
            }
        },
    };



    public static bool canUpgrade(Unit.UnitType type, Player owner)
    {
        bool levelCheck = true;
        bool resourceCheck = true;

        if (type == Unit.UnitType.REGULAR)
        {
            levelCheck = owner.unitLevel[0] < 20;

            resourceCheck = (owner.science >= int.Parse((upgradeCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 0])) &&
                            (owner.resources[5] >= int.Parse((upgradeCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 1])) &&
                            (owner.resources[0] >= int.Parse((upgradeCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 2])) &&
                            (owner.resources[3] >= int.Parse((upgradeCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 3]));
        }
        else if (type == Unit.UnitType.RANGED)
        {
            levelCheck = owner.unitLevel[1] < 20;

            resourceCheck = (owner.science >= int.Parse((upgradeCosts[0])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 0])) &&
                            (owner.resources[4] >= int.Parse((upgradeCosts[1])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 1])) &&
                            (owner.resources[0] >= int.Parse((upgradeCosts[1])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 2])) &&
                            (owner.resources[1] >= int.Parse((upgradeCosts[1])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 3]));
        }
        else if (type == Unit.UnitType.HEAVY)
        {
            levelCheck = owner.unitLevel[2] < 20;

            resourceCheck = (owner.science >= int.Parse((upgradeCosts[0])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 0])) &&
                            (owner.resources[4] >= int.Parse((upgradeCosts[2])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 1])) &&
                            (owner.resources[0] >= int.Parse((upgradeCosts[2])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 2])) &&
                            (owner.resources[2] >= int.Parse((upgradeCosts[2])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 3]));
        }
        else
        {
            Debug.Log("canUpgrade : Unknown or non-attacker unit type");
        }

        return levelCheck && resourceCheck;
    }



    public static bool canSpawn(Unit.UnitType type, Player owner, City city)
    {
        bool cityCheck = true;
        bool resourceCheck = true;

        if (type == Unit.UnitType.REGULAR)
        {
            cityCheck = (city.spawncount) < (city.prodRate);
            resourceCheck = (owner.money >= int.Parse((unitCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 0])) &&
                            (owner.resources[5] >= int.Parse((unitCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 1])) &&
                            (owner.resources[0] >= int.Parse((unitCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 2])) &&
                            (owner.resources[3] >= int.Parse((unitCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 3]));
        }
        else if (type == Unit.UnitType.RANGED)
        {
            cityCheck = ((city.spawncount) < (city.prodRate)) && (city.Size == City.CitySize.CITY || city.Size == City.CitySize.MEGALOPOLIS);
            resourceCheck = (owner.money >= int.Parse((unitCosts[0])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 0])) &&
                            (owner.resources[4] >= int.Parse((unitCosts[1])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 1])) &&
                            (owner.resources[0] >= int.Parse((unitCosts[1])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 2])) &&
                            (owner.resources[1] >= int.Parse((unitCosts[1])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 3]));
        }
        else if (type == Unit.UnitType.HEAVY)
        {
            cityCheck = ((city.spawncount) < (city.prodRate)) && (city.Size == City.CitySize.MEGALOPOLIS);
            resourceCheck = (owner.money >= int.Parse((unitCosts[0])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 0])) &&
                            (owner.resources[4] >= int.Parse((unitCosts[2])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 1])) &&
                            (owner.resources[0] >= int.Parse((unitCosts[2])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 2])) &&
                            (owner.resources[2] >= int.Parse((unitCosts[2])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 3]));
        }
        else if (type == Unit.UnitType.WORKER)
        {
            cityCheck = (city.spawncount) < (city.prodRate);
            resourceCheck = (owner.money >= 120) &&
                            (owner.resources[5] >= 200) &&
                            (owner.resources[4] >= 200) &&
                            (owner.resources[0] >= 69);
        }
        else if (type == Unit.UnitType.SETTLER)
        {
            cityCheck = ((city.spawncount) < (city.prodRate)) && (city.Size == City.CitySize.CITY || city.Size == City.CitySize.MEGALOPOLIS);
            resourceCheck = (owner.money >= 300) &&
                            (owner.resources[5] >= 420) &&
                            (owner.resources[4] >= 500) &&
                            (owner.resources[2] >= 10);
        }
        else
        {
            Debug.Log("canSpawn : Unknown unit type");
        }

        return cityCheck && resourceCheck;
    }



    public static void MakeUpgrade(Unit.UnitType type, Player owner)
    {
        if (type == Unit.UnitType.REGULAR)
        {
            owner.resources[5] -= int.Parse((upgradeCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 1]);
            owner.resources[0] -= int.Parse((upgradeCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 2]);
            owner.resources[3] -= int.Parse((upgradeCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 3]);
        }
        else if (type == Unit.UnitType.RANGED)
        {
            owner.resources[4] -= int.Parse((upgradeCosts[1])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 1]);
            owner.resources[0] -= int.Parse((upgradeCosts[1])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 2]);
            owner.resources[1] -= int.Parse((upgradeCosts[1])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 3]);
        }
        else if (type == Unit.UnitType.HEAVY)
        {
            owner.resources[4] -= int.Parse((upgradeCosts[2])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 1]);
            owner.resources[0] -= int.Parse((upgradeCosts[2])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 2]);
            owner.resources[2] -= int.Parse((upgradeCosts[2])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 3]);
        }
        else
        {
            Debug.Log("canUpgrade : Unknown or non-attacker unit type");
        }
    }
    
    

    public static void MakePurshase(Unit.UnitType type, Player owner)
    {
        if (type == Unit.UnitType.REGULAR)
        {
            owner.money -= int.Parse((unitCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 0]);
            owner.resources[5] -= int.Parse((unitCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 1]);
            owner.resources[0] -= int.Parse((unitCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 2]);
            owner.resources[3] -= int.Parse((unitCosts[0])[owner.GetUnitLevel(Unit.UnitType.REGULAR) - 1, 3]);
        }
        else if (type == Unit.UnitType.RANGED)
        {
            owner.money -= int.Parse((unitCosts[0])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 0]);
            owner.resources[4] -= int.Parse((unitCosts[1])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 1]);
            owner.resources[0] -= int.Parse((unitCosts[1])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 2]);
            owner.resources[1] -= int.Parse((unitCosts[1])[owner.GetUnitLevel(Unit.UnitType.RANGED) - 1, 3]);
        }
        else if (type == Unit.UnitType.HEAVY)
        {
            owner.money -= int.Parse((unitCosts[0])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 0]);
            owner.resources[4] -= int.Parse((unitCosts[2])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 1]);
            owner.resources[0] -= int.Parse((unitCosts[2])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 2]);
            owner.resources[2] -= int.Parse((unitCosts[2])[owner.GetUnitLevel(Unit.UnitType.HEAVY) - 1, 3]);
        }
        else if (type == Unit.UnitType.WORKER)
        {
            owner.money -= 120;
            owner.resources[5] -= 200;
            owner.resources[4] -= 200;
            owner.resources[0] -= 69;
        }
        else if (type == Unit.UnitType.SETTLER)
        {
            owner.money -= 300;
            owner.resources[5] -= 420;
            owner.resources[4] -= 500;
            owner.resources[2] -= 5;
        }
        else
        {
            Debug.Log("canSpawn : Unknown unit type");
        }
    }
}
