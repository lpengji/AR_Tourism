using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using System;
using UnityEngine.Android;

public class VPS_Manager : MonoBehaviour
{
    [SerializeField]
    private AREarthManager earthManager;

    [Serializable]
    public struct GeospatialObject
    {
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
    private ARAnchorManager arAnchorManager;

    [SerializeField]
    private List<GeospatialObject> geospatialObjects;

    void Start()
    {
        if (earthManager == null)
        {
            earthManager = GetComponent<AREarthManager>();
        }

        // Ensure location permissions are granted
        StartCoroutine(CheckLocationPermission());
    }

    private IEnumerator CheckLocationPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(1f);
        }

        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Debug.Log("Waiting for location permissions...");
            yield return new WaitForSeconds(1f);
        }

        VerifyGeospatialSupport();
    }

    private void VerifyGeospatialSupport()
    {
        var result = earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
        switch (result)
        {
            case FeatureSupported.Supported:
                Debug.Log("Ready to use VPS");
                StartCoroutine(CheckARSessionState());
                break;
            case FeatureSupported.Unknown:
                Debug.Log("Geospatial support status unknown. Retrying...");
                Invoke("VerifyGeospatialSupport", 5.0f);
                break;
            case FeatureSupported.Unsupported:
                Debug.Log("VPS Unsupported on this device.");
                break;
        }
    }

    private IEnumerator CheckARSessionState()
    {
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            Debug.Log("Waiting for AR session to be ready...");
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("AR session is ready");
        PlaceObjects();
    }

    private void PlaceObjects()
    {
        Debug.Log("Attempting to place objects.");

        if (earthManager.EarthTrackingState == TrackingState.Tracking)
        {
            Debug.Log("Earth tracking is active. Placing objects...");

            // Camera's geospatial pose en WGS84 
            var geospatialPose = earthManager.CameraGeospatialPose;
            Debug.Log($"Camera Pose: Lat: {geospatialPose.Latitude}, Lon: {geospatialPose.Longitude}, Alt: {geospatialPose.Altitude}");

            foreach (var obj in geospatialObjects)
            {
                var earthPosition = obj.EarthPosition;
                // Debug.Log($"Creating anchor at Lat: {earthPosition.Latitude}, Lon: {earthPosition.Longitude}, Alt: {earthPosition.Altitude}");

                var objAnchor = ARAnchorManagerExtensions.AddAnchor(arAnchorManager,
                    geospatialPose.Latitude, geospatialPose.Longitude, geospatialPose.Altitude + 1, Quaternion.identity);

                if (objAnchor != null)
                {
                    var instantiatedObject = Instantiate(obj.ObjectPrefab, objAnchor.transform);
                    Debug.Log("Instantiated object at: " + objAnchor.transform.position + " with local position: " + instantiatedObject.transform.localPosition);

                    instantiatedObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("Failed to create anchor for object at " +
                        $"Lat: {earthPosition.Latitude}, Lon: {earthPosition.Longitude}, Alt: {earthPosition.Altitude}");
                }
            }
        }
        else
        {
            Debug.Log("Earth tracking not active. Retrying...");
            Invoke("PlaceObjects", 5.0f);
        }
    }
}
