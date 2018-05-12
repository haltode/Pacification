using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGeneratorPanel : MonoBehaviour {

    public GameObject warningMessage;

    public Text Textseed;
    public Slider SlidermapSize;
    public Slider SliderjitterProbability;
    public Slider SliderchunkSizeMin;
    public Slider SliderchunkSizeMax;
    public Slider SliderlandPercentage;
    public Slider SliderelevationMaximum;
    public Slider SliderregionBorder;
    public Slider SliderregionCount;
    public Slider SlidererosionPercentage;

    public GameObject randomPanel;
    public GameObject loadPanel;
    public Text button;

    public string path = "";
    bool modeRandom = true;

    private void Update()
    {
        warningMessage.SetActive(SlidermapSize.value >= 3);
    }

    public void ChangeGenMode()
    {
        loadPanel.SetActive(modeRandom);
        modeRandom = !modeRandom;
        randomPanel.SetActive(modeRandom);

        if(modeRandom)
        {
            button.text = "RANDOM";
            path = "";
        }
        else
            button.text = "LOAD";
    }
}
