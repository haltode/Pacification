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

    private void Update()
    {
        warningMessage.SetActive(SlidermapSize.value >= 3);
    }
}
