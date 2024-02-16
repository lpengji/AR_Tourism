using System.Collections;
using System.Collections.Generic;
using Mapbox.Examples;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using GeoCoordinatePortable;
using UnityEngine;


public class LocationPointInformation : MonoBehaviour
{
    [SerializeField]
    private Vector2d actualCoordinate;

    public void setActualCoordinate(Vector2d actualCoordinate){
        this.actualCoordinate = actualCoordinate;
    }

    public Vector2d getActualCoordinate(){
        return this.actualCoordinate;
    }

    public double getDistanceBetweenPlayerAndLocation(){
        LocationStatus playerLocation = GameObject.Find("Canvas").GetComponent<LocationStatus>();
        GeoCoordinate currentPlayerLocation = new GeoCoordinate(playerLocation.getLatitude(),playerLocation.getLongitude());
        GeoCoordinate currentLocationPointLocation = new GeoCoordinate(actualCoordinate[0],actualCoordinate[1]);

        double distance = currentPlayerLocation.GetDistanceTo(currentLocationPointLocation);
        return distance;
    }

    
}
