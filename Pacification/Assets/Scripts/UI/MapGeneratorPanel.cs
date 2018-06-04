using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGeneratorPanel : MonoBehaviour {

    public GameObject warningMessageS;
    public GameObject warningMessageM;

    public Text TextseedS;
    public Slider SlidermapSizeS;
    public Slider SliderResourcesS;
    public Slider SliderjitterProbabilityS;
    public Slider SliderchunkSizeMinS;
    public Slider SliderchunkSizeMaxS;
    public Slider SliderlandPercentageS;
    public Slider SliderelevationMaximumS;
    public Slider SliderregionBorderS;
    public Slider SliderregionCountS;
    public Slider SlidererosionPercentageS;

    public Text TextseedM;
    public Slider SlidermapSizeM;
    public Slider SliderResourcesM;
    public Slider SliderjitterProbabilityM;
    public Slider SliderchunkSizeMinM;
    public Slider SliderchunkSizeMaxM;
    public Slider SliderlandPercentageM;
    public Slider SliderelevationMaximumM;
    public Slider SliderregionBorderM;
    public Slider SliderregionCountM;
    public Slider SlidererosionPercentageM;

    public GameObject randomPanel;
    public GameObject loadPanel;
    public Text button;

    public string path = "";
    bool modeRandom = true;

    private void Update()
    {
        warningMessageS.SetActive(SlidermapSizeS.value >= 3);
        warningMessageM.SetActive(SlidermapSizeM.value >= 3);
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
