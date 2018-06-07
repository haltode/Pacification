using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEconomy
{
    // NOTES:
    // Ville lv2 (donc colon et archer) dispo à partir du tour 50 sans amélioration
    // Ville lv3 (donc catapulte) dispo à partir du tour 160 sans amélioration

    public List<string[,]> upgradeCosts = new List<string[,]>
    {
        // [ (upgrade depuis level) X - 1 , ressource ID (cf Post-It ou commentaire) ]

        {new string[,] //Regular //science, food, steel, horse
            {
                {"5", "0", "0", "0"},       //lv1  -> 2
                {"20", "0", "0", "0"},      //lv2  -> 3
                {"50", "0", "0", "0"},      //lv3  -> 4
                {"100", "0", "0", "0"},     //lv4  -> 5 //Player can build more cities from this point on
                {"200", "0", "0", "0"},     //lv5  -> 6
                {"300", "0", "0", "0"},     //lv6  -> 7
                {"400", "0", "0", "0"},     //lv7  -> 8
                {"500", "0", "0", "0"},     //lv8  -> 9 
                {"650", "0", "0", "0"},     //lv9  -> 10
                {"1 000", "0", "0", "0"},   //lv10 -> 11 //(Era change !) Science: ~120 turns with a single non-upgraded city
                {"1 500", "0", "0", "0"},   //lv11 -> 12
                {"2 500", "0", "0", "0"},   //lv12 -> 13
                {"3 700", "0", "0", "0"},   //lv13 -> 14
                {"5 000", "0", "0", "0"},   //lv14 -> 15
                {"7 000", "0", "0", "0"},   //lv15 -> 16
                {"9 000", "0", "0", "0"},   //lv16 -> 17
                {"11 000", "0", "0", "0"},  //lv17 -> 18
                {"14 000", "0", "0", "0"},  //lv18 -> 19
                {"20 000", "0", "0", "0"},  //lv19 -> 20 //Science: ~400 turns with a single non-upgraded city
            }
        },
        {new string[,] //Ranged //science, wood, steel, gold
            {
                {"200", "0", "0", "0"},     //lv1 -> 2 //Player will have at least 122 science points before being able to build them or more cities. Turn ~50 minimum.
                {"500", "0", "0", "0"},     //lv2 -> 3
                {"800", "0", "0", "0"},     //lv3 -> 4
                {"1 200", "0", "0", "0"},   //lv4 -> 5
                {"1 800", "0", "0", "0"},   //lv5 -> 6
                {"2 500", "0", "0", "0"},   //lv6 -> 7
                {"3 500", "0", "0", "0"},   //lv7 -> 8
                {"5 000", "0", "0", "0"},   //lv8 -> 9
                {"7 000", "0", "0", "0"},   //lv9 -> 10
                {"10 000", "0", "0", "0"},  //lv10 -> 11 (Era change !) //Science: ~300 turns with a single non-upgraded city
                {"12 000", "0", "0", "0"},  //lv11 -> 12
                {"15 000", "0", "0", "0"},  //lv12 -> 13
                {"18 000", "0", "0", "0"},  //lv13 -> 14
                {"22 000", "0", "0", "0"},  //lv14 -> 15
                {"26 000", "0", "0", "0"},  //lv15 -> 16
                {"33 000", "0", "0", "0"},  //lv16 -> 17
                {"39 500", "0", "0", "0"},  //lv17 -> 18
                {"46 800", "0", "0", "0"},  //lv18 -> 19
                {"60 000", "0", "0", "0"},  //lv19 -> 20 //Science: ~600 turns with a single non-upgraded city
            }
        },
        {new string[,] //Heavy //science, wood, steel, diamond
            {
                {"3 000", "0", "0", "0"},   //lv1 -> 2 //Player will have at least 2140 science points before being able to build them. Turn ~160 minimum.
                {"4 200", "0", "0", "0"},   //lv2 -> 3
                {"5 800", "0", "0", "0"},   //lv3 -> 4
                {"7 500", "0", "0", "0"},   //lv4 -> 5
                {"10 000", "0", "0", "0"},  //lv5 -> 6
                {"13 000", "0", "0", "0"},  //lv6 -> 7
                {"17 000", "0", "0", "0"},  //lv7 -> 8
                {"22 000", "0", "0", "0"},  //lv8 -> 9
                {"28 000", "0", "0", "0"},  //lv9 -> 10
                {"35 000", "0", "0", "0"},  //lv10 -> 11 (Era change !) Science: ~500 turns with a single non-upgraded city
                {"47 000", "0", "0", "0"},  //lv11 -> 12
                {"60 000", "0", "0", "0"},  //lv12 -> 13
                {"80 000", "0", "0", "0"},  //lv13 -> 14
                {"105 000", "0", "0", "0"}, //lv14 -> 15
                {"136 000", "0", "0", "0"}, //lv15 -> 16
                {"175 000", "0", "0", "0"}, //lv16 -> 17
                {"240 000", "0", "0", "0"}, //lv17 -> 18
                {"340 000", "0", "0", "0"}, //lv18 -> 19
                {"500 000", "0", "0", "0"}, //lv19 -> 20 Science: ~1000 turns with a single non-upgraded city
            }
        }
    };
}
