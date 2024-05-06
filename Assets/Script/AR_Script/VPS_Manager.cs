using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using System;

public class VPS_Manager : MonoBehaviour
{
    [SerializeField]
    private AREarthManager earthManager;

    [Serializable]
    public struct GeospatialObject{
        public GameObject ObjectPrefab;
        public EarthPosition EarthPosition;
    }


    [Serializable]
    public struct EarthPosition
    {
        public double Latitude;
        public double Longitude;
        public double Altitude;
    }

    [SerializeField]
    private ARAnchorManager aRAnchorManager;

    [SerializeField]
    private List<GeospatialObject> geospatial0bjects = new List<GeospatialObject>();
    
    void Start()
    {
        this.VerifyGeospatialSupport();
    }

    private void VerifyGeospatialSupport(){
        var result = this.earthManager. IsGeospatialModeSupported(GeospatialMode.Enabled);
        switch (result){
            case FeatureSupported.Supported:
                Debug.Log("Ready to use VPS");
                this.PlaceObjects();
                break;
            case FeatureSupported.Unknown:
                Debug.Log("Unknown");
                Invoke("VerifyGeospatialSupport",5.0f);
                break;
            case FeatureSupported. Unsupported:
                Debug.Log("VPS Unsupported");
                break;
        }   
    }

    private void PlaceObjects(){
        if (this.earthManager.EarthTrackingState == TrackingState.Tracking){
            var geospatialPose = this.earthManager.CameraGeospatialPose;

        foreach (var obj in this.geospatial0bjects){
                var earthPosition = obj.EarthPosition;
                var objAnchor = ARAnchorManagerExtensions.AddAnchor(this.aRAnchorManager, earthPosition.Latitude, 
                earthPosition.Longitude, earthPosition.Altitude, Quaternion.identity);
                Instantiate(obj.ObjectPrefab,objAnchor.transform);
            }
        }
        else if(this.earthManager.EarthTrackingState == TrackingState.None){
            Invoke("PlaceObjects",5.0f);
        }
    }
 
}
