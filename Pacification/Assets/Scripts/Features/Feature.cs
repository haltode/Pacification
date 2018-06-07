using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feature
{
    public enum FeatureType
    {
        CITY,
        RESOURCE
    }

    public Player Owner { get; set; }

    public FeatureType Type { get; set; }

    public HexCell Location { get; set; }

    public int Hp { get; set; }

    public int MaxHp { get; set; }

    public string TypeToStr()
    {
        if(Type == FeatureType.CITY)
        {
            City city = (City)this;
            switch(city.Size)
            {
                case City.CitySize.CITY:
                    return "City";

                case City.CitySize.SETTLEMENT:
                    return "Settlement";

                case City.CitySize.MEGALOPOLIS:
                    return "Megalopolis";

                default:
                    return "";
            }
        }
        else
        {
            Resource resource = (Resource)this;

            switch(resource.RessourceType)
            {
                case Resource.ResourceType.WOOD:
                    return "Wood";

                case Resource.ResourceType.IRON:
                    return "Iron";

                case Resource.ResourceType.GOLD:
                    return "Gold";

                case Resource.ResourceType.DIAMOND:
                    return "Diamond";

                case Resource.ResourceType.HORSES:
                    return "Horses";

                case Resource.ResourceType.FOOD:
                    return "Food";

                default:
                    return "";
            }
        }
    }
}
