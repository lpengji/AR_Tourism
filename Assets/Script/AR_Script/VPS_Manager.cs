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
    [SerializeField]
    private GameObject loadingAnimation;

    [SerializeField]
    private ARAnchorManager arAnchorManager;

    [SerializeField]
    private GameObject geospatialObjectsPrefab;
    private bool hasStarted = false;

    public GeospatialPose geospatialPose;
    private List<ARLocationInformation> aRLocationInformations;
    [SerializeField]
    private ARInforDDBBManagement aRInforDDBBManagement;

    void Start()
    {
        loadingAnimation.SetActive(true);
        if (hasStarted)
        {
            return;
        }
        earthManager = GetComponent<AREarthManager>();
        aRLocationInformations = aRInforDDBBManagement.arLocationInformations;
        // Ensure location permissions are granted
        StartCoroutine(CheckLocationPermission());

        hasStarted = true;
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
        PlaceObjects(aRLocationInformations);
    }

    public void PlaceObjects(List<ARLocationInformation> arLocationInformations)
    {
        this.aRLocationInformations = arLocationInformations;

        Debug.Log("Attempting to place objects.");

        if (earthManager.EarthTrackingState == TrackingState.Tracking)
        {
            Debug.Log("Earth tracking is active. Placing objects...");

            // Camera's geospatial pose en WGS84 
            this.geospatialPose = earthManager.CameraGeospatialPose;
            Debug.Log($"Camera Pose: Lat: {geospatialPose.Latitude}, Lon: {geospatialPose.Longitude}, Alt: {geospatialPose.Altitude}");

            StartCoroutine(PlaceObjectsCoroutine(arLocationInformations));
            Debug.Log("Coroutine started");
        }
        else
        {
            Debug.Log("Earth tracking not active. Retrying...");
            Invoke("PlaceObjects", 5.0f);
        }
    }



    private IEnumerator PlaceObjectsCoroutine(List<ARLocationInformation> arLocationInformations)
    {
        Debug.Log("Starting PlaceObjectsCoroutine...");

        foreach (var info in arLocationInformations)
        {
            // Usar altitud de geospatialPose si info.Altitud es 0
            double altitudeToUse = info.Altitud != 0 ? info.Altitud : geospatialPose.Altitude;

            var objAnchor = ARAnchorManagerExtensions.AddAnchor(arAnchorManager,
                info.Latitud, info.Longitud, altitudeToUse, Quaternion.identity);

            if (objAnchor != null)
            {
                var instantiatedObject = Instantiate(geospatialObjectsPrefab, objAnchor.transform);
                var arLocationInformationObj = instantiatedObject.GetComponent<ARLocationInformationObj>();

                if (arLocationInformationObj != null)
                {
                    arLocationInformationObj.Id = info.Id;
                    arLocationInformationObj.Latitud = info.Latitud;
                    arLocationInformationObj.Longitud = info.Longitud;
                    arLocationInformationObj.Altitud = info.Altitud;
                    arLocationInformationObj.Information = info.Information;
                    Debug.Log("OBJECTO AR CREADO CORRECTAMENTE");
                }
                else
                {
                    Debug.LogError("ARLocationInformationObj script is missing on the AR GameObject.");
                }

                Debug.Log("Instantiated object at: " + objAnchor.transform.position + " with local position: " + instantiatedObject.transform.localPosition);

                instantiatedObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Failed to create anchor for object at " +
                    $"Lat: {info.Latitud}, Lon: {info.Longitud}, Alt: {info.Altitud}");
            }

            yield return null;
        }

        Debug.Log("PlaceObjectsCoroutine completed.");
        // Después de completar la colocación de objetos, desactivar la animación de carga
        loadingAnimation.SetActive(false);
    }

}
