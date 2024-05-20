using System.Collections.Generic;

[System.Serializable]
public class RecommendedLocationList
{
    public int listID;
    public string listName;
    public List<LocationPoint> locations;

    // Constructor
    public RecommendedLocationList(int listID, string listName, List<LocationPoint> locations)
    {
        this.listID = listID;
        this.listName = listName;
        this.locations = locations;
    }

    public int ListID
    {
        get { return listID; }
        set { listID = value; }
    }

    public string ListName
    {
        get { return listName; }
        set { listName = value; }
    }

    public List<LocationPoint> Locations
    {
        get { return locations; }
        set { locations = value; }
    }
}