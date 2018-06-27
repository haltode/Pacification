
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamManager : MonoBehaviour
{

    public GameObject cameraOne;
    public GameObject cameraTwo;

    int cameraCount = -1;

    void Start()
    {
        CameraChangeCounter();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F12))
            CameraChangeCounter();
    }

    //Camera Counter
    void CameraChangeCounter()
    {
        //int cameraPositionCounter = PlayerPrefs.GetInt("CameraPosition"); //Will register wich cam was last chosen for the next game 
        cameraCount++;
        if(cameraCount > 1)
            cameraCount = 0;
        //PlayerPrefs.SetInt("CameraPosition", camPosition);

        if(cameraCount == 0)
        {
            cameraOne.SetActive(true);
            cameraTwo.SetActive(false);
        }
        else
        {
            cameraTwo.SetActive(true);
            cameraOne.SetActive(false);
        }
    }
}