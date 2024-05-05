using System.Collections.Generic;

[System.Serializable]
public class RecommendedLocationList
{
    public int ListID { get; set; }
    public string ListName { get; set; }
    public List<LocationPoint> Locations { get; set; }
}