using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using UnityEngine.Android;
using TMPro;

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

    private List<GameObject> instantiatedObjects = new List<GameObject>();  // Lista para mantener los objetos instanciados

    public void Instantiate()
    {
        loadingAnimation.SetActive(true);
        aRLocationInformations = aRInforDDBBManagement.arLocationInformations;
        if (hasStarted)
        {
            PlaceObjects();
        }
        else
        {
            earthManager = GetComponent<AREarthManager>();
            hasStarted = true;
            StartCoroutine(CheckLocationPermission());
        }
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
                Invoke(nameof(VerifyGeospatialSupport), 5.0f);
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

    public void PlaceObjects()
    {
        Debug.Log("Attempting to place objects.");
        if (earthManager.EarthTrackingState == TrackingState.Tracking)
        {
            Debug.Log("Earth tracking is active. Placing objects...");
            geospatialPose = earthManager.CameraGeospatialPose;
            Debug.Log($"Camera Pose: Lat: {geospatialPose.Latitude}, Lon: {geospatialPose.Longitude}, Alt: {geospatialPose.Altitude}");
            StartCoroutine(PlaceObjectsCoroutine(aRLocationInformations));
        }
        else
        {
            Debug.Log("Earth tracking not active. Retrying...");
            Invoke(nameof(PlaceObjects), 5.0f);
        }
    }

    private IEnumerator PlaceObjectsCoroutine(List<ARLocationInformation> arLocationInformations)
    {
        // Destroy previous objects
        foreach (var obj in instantiatedObjects)
        {
            Destroy(obj);
        }
        instantiatedObjects.Clear();

        foreach (var info in arLocationInformations)
        {
            Debug.Log($"Processing AR location with ID: {info.Id}");

            double altitudeToUse = info.Altitud != 0 ? info.Altitud : geospatialPose.Altitude;
            altitudeToUse += 0.5;

            var objAnchor = ARAnchorManagerExtensions.AddAnchor(arAnchorManager,
                info.Latitud, info.Longitud, altitudeToUse, Quaternion.identity);

            if (objAnchor != null)
            {
                Debug.Log($"Anchor created successfully for ID: {info.Id}");
                var instantiatedObject = Instantiate(geospatialObjectsPrefab, objAnchor.transform);
                var arLocationInformationObj = instantiatedObject.GetComponent<ARLocationInformationObj>();

                if (arLocationInformationObj != null)
                {
                    arLocationInformationObj.Id = info.Id;
                    arLocationInformationObj.Latitud = info.Latitud;
                    arLocationInformationObj.Longitud = info.Longitud;
                    arLocationInformationObj.Altitud = info.Altitud;
                    arLocationInformationObj.Information = info.Information;
                    Debug.Log("AR object created successfully.");

                    // Find the TMP_Text component by name
                    var textTMP = FindTextComponentByName(instantiatedObject.transform, "Text (TMP)");
                    if (textTMP != null)
                    {
                        textTMP.text = info.Information;
                        Debug.Log("TMP_Text component updated successfully.");
                    }
                    else
                    {
                        Debug.LogError("TMP_Text component is missing on the AR GameObject.");
                        PrintHierarchy(instantiatedObject.transform); // Print hierarchy for debugging
                    }
                }
                else
                {
                    Debug.LogError("ARLocationInformationObj script is missing on the AR GameObject.");
                }

                instantiatedObjects.Add(instantiatedObject);  // Add the instantiated object to the list
                instantiatedObject.SetActive(true);
            }
            else
            {
                Debug.LogError($"Failed to create anchor for object at Lat: {info.Latitud}, Lon: {info.Longitud}, Alt: {info.Altitud}");
            }

            yield return null;
        }

        loadingAnimation.SetActive(false);
    }


    // Función recursiva para encontrar el componente TextMeshProUGUI por nombre
    private TMP_Text FindTextComponentByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                // Check for both TMP_Text and TextMeshProUGUI
                var tmpTextComponent = child.GetComponent<TMP_Text>();
                if (tmpTextComponent != null)
                {
                    Debug.Log($"Found TMP_Text component on GameObject: {child.name}");
                    return tmpTextComponent;
                }
                else
                {
                    Debug.LogError($"TMP_Text component not found on GameObject: {child.name}");
                }
            }
            var result = FindTextComponentByName(child, name);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }



    // Función para imprimir la jerarquía del objeto instanciado
    private void PrintHierarchy(Transform parent, string indent = "")
    {
        Debug.Log($"{indent}{parent.name} ({parent.GetType()})");
        foreach (Transform child in parent)
        {
            PrintHierarchy(child, indent + "  ");
        }
    }


}
