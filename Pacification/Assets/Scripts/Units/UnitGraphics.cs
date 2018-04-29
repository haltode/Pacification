using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGraphics : MonoBehaviour
{
    public static void SetGraphics(GameObject unit, GameObject obj)
    {
        GameObject graphicsObject = null;

        for (int i = 0; i < unit.transform.childCount; i++)
        {
            if (unit.transform.GetChild(i).name == "Graphics")
                graphicsObject = unit.transform.GetChild(i).gameObject;
        }

        if (graphicsObject != null)
            Destroy(graphicsObject);

        GameObject instantiated = Instantiate(obj, unit.transform.position + new Vector3(0, 2, 0), Quaternion.identity);

        instantiated.name = "Graphics";

        instantiated.transform.parent = unit.transform;
    }
}
