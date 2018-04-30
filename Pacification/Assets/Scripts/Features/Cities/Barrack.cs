using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrack : MonoBehaviour
{
    /*

    Explanation:

    The barrack is a building that exists by default in every City. You should access it by clicking on a "barrack" button in the City UI.
    It should spawn the Unit on the city, or in a neighbour cell depending on how much of a pain in the ass it is to code.
        (If on itself: it means units can move on cells where there is a friendly/neutral feature, but not an ennemy one. Which would be ideal)
        (If on neighbours: it means units can't access any cell where there is a feature, regardless of who owns it. Will be a problem for settlers/workers.)
    Barracks should only be able to generate one unit per turn, but the "security" of the turn-based mechanic will come later for both cities and units.

    */

    public GameObject BarrackObject;
    public Player Owner;
    public HexGrid hexGrid;

    // TODO : get the cell of the city this barrack is from, or a neighbour cell (see explanation above)
    HexCell cityLocation;

    void Start()
    {
        // TODO : link with the actual player, this is a placeholder + don't forget to take the EDITOR into account ==> the player is not in the Client
        Owner = new Player();
        hexGrid = FindObjectOfType<HexGrid>();
        cityLocation = hexGrid.GetCell(4, 2);
        BarrackObject.SetActive(false);
    }

    public void CreateUnitButton(string type)
    {
        Owner.CreateUnit(Unit.StrToType(type), cityLocation);
    }
}
