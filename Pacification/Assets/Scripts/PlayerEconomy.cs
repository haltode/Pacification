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
                {"5", "250", "250", "0"},      //lv1  -> 2
                {"20", "250", "250", "0"},     //lv2  -> 3
                {"50", "250", "250", "0"},     //lv3  -> 4
                {"100", "250", "250", "0"},    //lv4  -> 5 //Player can build more cities from this point on
                {"200", "500", "250", "0"},    //lv5  -> 6
                {"300", "500", "500", "0"},    //lv6  -> 7
                {"400", "500", "500", "0"},    //lv7  -> 8
                {"500", "500", "500", "0"},    //lv8  -> 9 
                {"650", "500", "500", "0"},    //lv9  -> 10
                {"1000", "2500", "2500", "75"},   //lv10 -> 11 //(Era change !) Science: ~120 turns with a single non-upgraded city
                {"1500", "1000", "500", "12"},   //lv11 -> 12
                {"2500", "1000", "500", "13"},   //lv12 -> 13
                {"3700", "1000", "1000", "14"},   //lv13 -> 14
                {"5000", "1000", "1000", "15"},   //lv14 -> 15
                {"7000", "1000", "1000", "16"},   //lv15 -> 16
                {"9000", "1000", "1000", "17"},   //lv16 -> 17
                {"11000", "1000", "1000", "18"},  //lv17 -> 18
                {"14000", "1000", "1000", "19"},  //lv18 -> 19
                {"20000", "5000", "2500", "20"},  //lv19 -> 20 //Science: ~400 turns with a single non-upgraded city
            }
        },
        {new string[,] //Ranged //science, wood, steel, gold
            {
                {"200", "250", "250", "0"},    //lv1 -> 2 //Player will have at least 122 science points before being able to build them or more cities. Turn ~50 minimum.
                {"500", "250", "250", "0"},    //lv2 -> 3
                {"800", "250", "250", "0"},    //lv3 -> 4
                {"1200", "250", "250", "0"},   //lv4 -> 5
                {"1800", "500", "250", "0"},   //lv5 -> 6
                {"2500", "500", "500", "0"},   //lv6 -> 7
                {"3500", "500", "500", "0"},   //lv7 -> 8
                {"5000", "500", "500", "0"},   //lv8 -> 9
                {"7000", "500", "500", "0"},   //lv9 -> 10
                {"10000", "3000", "2500", "10000"},  //lv10 -> 11 (Era change !) //Science: ~300 turns with a single non-upgraded city
                {"12000", "1000", "500", "1200"},  //lv11 -> 12
                {"15000", "1000", "1000", "1300"},  //lv12 -> 13
                {"18000", "1000", "1000", "1400"},  //lv13 -> 14
                {"22000", "1000", "1000", "1500"},  //lv14 -> 15
                {"26000", "2500", "1000", "1600"},  //lv15 -> 16
                {"33000", "1000", "1000", "1700"},  //lv16 -> 17
                {"39500", "1000", "1000", "1800"},  //lv17 -> 18
                {"46800", "1000", "1000", "1900"},  //lv18 -> 19
                {"60000", "6000", "1000", "2000"},  //lv19 -> 20 //Science: ~600 turns with a single non-upgraded city
            }
        },
        {new string[,] //Heavy //science, wood, steel, diamond
            {
                {"3000", "500", "250", "0"},   //lv1 -> 2 //Player will have at least 2140 science points before being able to build them. Turn ~160 minimum.
                {"4200", "500", "250", "0"},   //lv2 -> 3
                {"5800", "500", "250", "0"},   //lv3 -> 4
                {"7500", "500", "250", "0"},   //lv4 -> 5
                {"10000", "500", "500", "0"},  //lv5 -> 6
                {"13000", "1000", "500", "0"},  //lv6 -> 7
                {"17000", "1000", "500", "0"},  //lv7 -> 8
                {"22000", "1000", "500", "0"},  //lv8 -> 9
                {"28000", "1000", "500", "0"},  //lv9 -> 10
                {"35000", "5000", "3500", "300"},  //lv10 -> 11 (Era change !) Science: ~500 turns with a single non-upgraded city
                {"47000", "1000", "1000", "20"},  //lv11 -> 12
                {"60000", "1000", "1000", "30"},  //lv12 -> 13
                {"80000", "1000", "1000", "40"},  //lv13 -> 14
                {"105000", "1000", "1000", "50"}, //lv14 -> 15
                {"136000", "2500", "1000", "60"}, //lv15 -> 16
                {"175000", "2500", "1000", "70"}, //lv16 -> 17
                {"240000", "2500", "1000", "80"}, //lv17 -> 18
                {"340000", "2500", "1000", "90"}, //lv18 -> 19
                {"500000", "7500", "1000", "100"}, //lv19 -> 20 Science: ~1000 turns with a single non-upgraded city
            }
        }
    };
}
