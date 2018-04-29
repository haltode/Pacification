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

    public GameObject HexUnitPrefab;
    public GameObject BarrackObject;
    public Player Owner;

    private HexCell cell; // TODO : get the cell of the city this barrack is from, or a neighbour cell (see explanation above)
    private HexMapEditor hme;

    private int counter; // TODO : maybe find a better name, the main purpose of this is the security for the HexMapEditor recovery. Also used to generate unit positions during tests.

    public GameObject SettlerPrefab;
    public GameObject WorkerPrefab;
    public GameObject RegularEra1Prefab;
    public GameObject RegularEra2Prefab;
    public GameObject RangedEra1Prefab;
    public GameObject RangedEra2Prefab;
    public GameObject HeavyEra1Prefab;
    public GameObject HeavyEra2Prefab;

    private Dictionary<string, GameObject> UnitFromString = new Dictionary<string, GameObject>();

    void Start()
    {
        Owner = new Player(); // TODO : link with the actual player, this is a placeholder + don't forget to take the EDITOR into account ==> the player is not in the Client
        BarrackObject.SetActive(false);
        PopulateDictionary();
        counter = 0;
    }
	
	void Update()
    {
        if (counter == 0)
        {
            hme = GameObject.FindObjectOfType<HexMapEditor>().GetComponent<HexMapEditor>();
            counter++;
        }
    }

    void PopulateDictionary()
    {
        UnitFromString["Settler"] = SettlerPrefab;
        UnitFromString["Worker"] = WorkerPrefab;
        UnitFromString["RegularEra1"] = RegularEra1Prefab;
        UnitFromString["RegularEra2"] = RegularEra2Prefab;
        UnitFromString["RangedEra1"] = RangedEra1Prefab;
        UnitFromString["RangedEra2"] = RangedEra2Prefab;
        UnitFromString["HeavyEra1"] = HeavyEra1Prefab;
        UnitFromString["HeavyEra2"] = HeavyEra2Prefab;
    }

    public void CreateUnitButton(string unitName)
    {
        Unit u = null;
        switch (unitName)
        {
            case "Settler":
                u = new Settler(Owner);
                break;
            case "Worker":
                u = new Worker(Owner);
                break;
            case "Regular":
                u = new Regular(Owner);
                unitName = (Owner.UnitLevel > 10 ? "RegularEra2" : "RegularEra1");
                break;
            case "Ranged":
                u = new Ranged(Owner);
                unitName = (Owner.UnitLevel > 10 ? "RangedEra2" : "RangedEra1");
                break;
            case "Heavy":
                u = new Heavy(Owner);
                unitName = (Owner.UnitLevel > 10 ? "HeavyEra2" : "HeavyEra1");
                break;
            default:
                Debug.Log("SpawnUnit::CreateUnit::Invalid unitType string passed as parameter.");
                break;
        }

        u.hexUnitGameObect = Instantiate(HexUnitPrefab);
        u.HexUnit = u.hexUnitGameObect.GetComponent<HexUnit>();
        u.HexUnit.Unit = u;
        UnitGraphics.SetGraphics(u.hexUnitGameObect, UnitFromString[unitName]);
        float orientation = UnityEngine.Random.Range(0f, 360f);

        // Debug.Log(hme.hexGrid == null); // TODO : dans Editor ça marche pas aled

        hme.hexGrid.AddUnit(u.HexUnit, hme.hexGrid.cells[counter], orientation); // TODO : get the position of the cell where the barrack is (the city you spawn from)
        counter++; // TODO : see above
    }
}
