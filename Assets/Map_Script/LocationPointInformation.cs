using System.Collections;
using System.Collections.Generic;
using Mapbox.Examples;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using GeoCoordinatePortable;
using UnityEngine;


public class LocationPointInformation : MonoBehaviour
{
    private Vector2d actualCoordinate;
    private int id;
    private float altitud;
    private int createdByUserID;
    private int informationId;

    public Vector2d ActualCoordinate
    {
        get { return actualCoordinate; }
        set { actualCoordinate = value; }
    }

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public float Altitud
    {
        get { return altitud; }
        set { altitud = value; }
    }

    public int CreatedByUserID
    {
        get { return createdByUserID; }
        set { createdByUserID = value; }
    }

    public int InformationId
    {
        get { return informationId; }
        set { informationId = value; }
    }

    public double getDistanceBetweenPlayerAndLocation()
    {
        LocationStatus playerLocation = GameObject.Find("LocationCoordinatesCanvas").GetComponent<LocationStatus>();
        GeoCoordinate currentPlayerLocation = new GeoCoordinate(playerLocation.getLatitude(), playerLocation.getLongitude());
        GeoCoordinate currentLocationPointLocation = new GeoCoordinate(actualCoordinate[0], actualCoordinate[1]);

        double distance = currentPlayerLocation.GetDistanceTo(currentLocationPointLocation);
        return distance;
    }


}
