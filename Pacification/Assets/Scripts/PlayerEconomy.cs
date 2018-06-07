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
        // [ (upgrade depuis level) X - 1 , ressource ID (cf Post-It ou commentaire) ]

        {new string[,] //Regular //science, food, steel, horse
            {
                {"5", "0", "0", "0"},      //lv1  -> 2
                {"20", "0", "0", "0"},     //lv2  -> 3
                {"50", "0", "0", "0"},     //lv3  -> 4
                {"100", "0", "0", "0"},    //lv4  -> 5 //Player can build more cities from this point on
                {"200", "0", "0", "0"},    //lv5  -> 6
                {"300", "0", "0", "0"},    //lv6  -> 7
                {"400", "0", "0", "0"},    //lv7  -> 8
                {"500", "0", "0", "0"},    //lv8  -> 9 
                {"650", "0", "0", "0"},    //lv9  -> 10
                {"1000", "0", "0", "0"},   //lv10 -> 11 //(Era change !) Science: ~120 turns with a single non-upgraded city
                {"1500", "0", "0", "0"},   //lv11 -> 12
                {"2500", "0", "0", "0"},   //lv12 -> 13
                {"3700", "0", "0", "0"},   //lv13 -> 14
                {"5000", "0", "0", "0"},   //lv14 -> 15
                {"7000", "0", "0", "0"},   //lv15 -> 16
                {"9000", "0", "0", "0"},   //lv16 -> 17
                {"11000", "0", "0", "0"},  //lv17 -> 18
                {"14000", "0", "0", "0"},  //lv18 -> 19
                {"20000", "0", "0", "0"},  //lv19 -> 20 //Science: ~400 turns with a single non-upgraded city
            }
        },
        {new string[,] //Ranged //science, wood, steel, gold
            {
                {"200", "0", "0", "0"},    //lv1 -> 2 //Player will have at least 122 science points before being able to build them or more cities. Turn ~50 minimum.
                {"500", "0", "0", "0"},    //lv2 -> 3
                {"800", "0", "0", "0"},    //lv3 -> 4
                {"1200", "0", "0", "0"},   //lv4 -> 5
                {"1800", "0", "0", "0"},   //lv5 -> 6
                {"2500", "0", "0", "0"},   //lv6 -> 7
                {"3500", "0", "0", "0"},   //lv7 -> 8
                {"5000", "0", "0", "0"},   //lv8 -> 9
                {"7000", "0", "0", "0"},   //lv9 -> 10
                {"10000", "0", "0", "0"},  //lv10 -> 11 (Era change !) //Science: ~300 turns with a single non-upgraded city
                {"12000", "0", "0", "0"},  //lv11 -> 12
                {"15000", "0", "0", "0"},  //lv12 -> 13
                {"18000", "0", "0", "0"},  //lv13 -> 14
                {"22000", "0", "0", "0"},  //lv14 -> 15
                {"26000", "0", "0", "0"},  //lv15 -> 16
                {"33000", "0", "0", "0"},  //lv16 -> 17
                {"39500", "0", "0", "0"},  //lv17 -> 18
                {"46800", "0", "0", "0"},  //lv18 -> 19
                {"60000", "0", "0", "0"},  //lv19 -> 20 //Science: ~600 turns with a single non-upgraded city
            }
        },
        {new string[,] //Heavy //science, wood, steel, diamond
            {
                {"3000", "0", "0", "0"},   //lv1 -> 2 //Player will have at least 2140 science points before being able to build them. Turn ~160 minimum.
                {"4200", "0", "0", "0"},   //lv2 -> 3
                {"5800", "0", "0", "0"},   //lv3 -> 4
                {"7500", "0", "0", "0"},   //lv4 -> 5
                {"10000", "0", "0", "0"},  //lv5 -> 6
                {"13000", "0", "0", "0"},  //lv6 -> 7
                {"17000", "0", "0", "0"},  //lv7 -> 8
                {"22000", "0", "0", "0"},  //lv8 -> 9
                {"28000", "0", "0", "0"},  //lv9 -> 10
                {"35000", "0", "0", "0"},  //lv10 -> 11 (Era change !) Science: ~500 turns with a single non-upgraded city
                {"47000", "0", "0", "0"},  //lv11 -> 12
                {"60000", "0", "0", "0"},  //lv12 -> 13
                {"80000", "0", "0", "0"},  //lv13 -> 14
                {"105000", "0", "0", "0"}, //lv14 -> 15
                {"136000", "0", "0", "0"}, //lv15 -> 16
                {"175000", "0", "0", "0"}, //lv16 -> 17
                {"240000", "0", "0", "0"}, //lv17 -> 18
                {"340000", "0", "0", "0"}, //lv18 -> 19
                {"500000", "0", "0", "0"}, //lv19 -> 20 Science: ~1000 turns with a single non-upgraded city
            }
        }
    };
}
