using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mapbox.Examples;
using System.Linq;
using System.IO;
using System;
using Mapbox.Utils;

public class LocationPointDDBBManagement : MonoBehaviour
{
    private string locationPointsPersistenceFileName = "locationPointDDBB.json";
    private List<LocationPoint> locationPoints;
    private User loggedUser;

    void Start()
    {
        string userJson = PlayerPrefs.GetString("AuthenticatedUser");
        this.loggedUser = JsonUtility.FromJson<User>(userJson);

        Debug.Log("loggedUser: " + loggedUser.userID + " , " + loggedUser.userName);

        // Cargar la información de los puntos de ubicación desde la base de datos
        LoadLocationPoints();
    }

    public void LoadLocationPoints()
    {
        string locationPointsURL = Path.Combine(Application.streamingAssetsPath, locationPointsPersistenceFileName);

#if UNITY_ANDROID && !UNITY_EDITOR
        string filePath = Path.Combine(Application.persistentDataPath, locationPointsPersistenceFileName);
#else
        string filePath = locationPointsURL;
#endif

        Debug.Log("File path: " + filePath);

        if (Application.platform == RuntimePlatform.Android && File.Exists(filePath))
        {
            LoadFile(filePath);
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(LoadFileFromStreamingAssets(locationPointsURL));
        }
        else
        {
            LoadFile(filePath);
        }
    }

    IEnumerator LoadFileFromStreamingAssets(string filePath)
    {
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al cargar el archivo: " + www.error);
        }
        else
        {
            string locationPointsInformation = www.downloadHandler.text;
            ProcessLocationPoints(locationPointsInformation);
        }
    }

    void LoadFile(string filePath)
    {
        string locationPointsInformation = File.ReadAllText(filePath);
        ProcessLocationPoints(locationPointsInformation);
    }

    void ProcessLocationPoints(string locationPointsInformation)
    {
        locationPoints = JsonUtility.FromJson<LocationPointsWrapper>(locationPointsInformation).locationPoints;
    }

    public void AddARInformation(int locationPointId, int arInformationId)
    {
        var locationPoint = locationPoints.Find(point => point.Id == locationPointId);
        if (locationPoint != null)
        {
            List<int> arInformationIds = new List<int>(locationPoint.ArInformationId);
            arInformationIds.Add(arInformationId);
            locationPoint.ArInformationId = arInformationIds.ToArray();
            SaveLocationPointsToFile();
        }
        else
        {
            Debug.LogError("LocationPoint not found with ID: " + locationPointId);
        }
    }

    public void RemoveARInformation(int locationPointId, int arInformationId)
    {
        var locationPoint = locationPoints.Find(point => point.Id == locationPointId);
        if (locationPoint != null)
        {
            List<int> arInformationIds = new List<int>(locationPoint.ArInformationId);
            arInformationIds.Remove(arInformationId);
            Debug.Log("longitud de la nueva lista creada en LOCATIONDDBB: " + arInformationIds.Count);
            locationPoint.ArInformationId = arInformationIds.ToArray();
            Debug.Log("#longitud de la lista original: " + locationPoint.ArInformationId.Length);

            SaveLocationPointsToFile();
        }
        else
        {
            Debug.LogError("LocationPoint not found with ID: " + locationPointId);
        }
    }

    void SaveLocationPointsToFile()
    {
        string json = JsonUtility.ToJson(new LocationPointsWrapper(locationPoints));
        string filePath;

#if UNITY_ANDROID && !UNITY_EDITOR
        filePath = Path.Combine(Application.persistentDataPath, locationPointsPersistenceFileName);
#else
        filePath = Path.Combine(Application.streamingAssetsPath, locationPointsPersistenceFileName);
#endif

        Debug.Log("File path: " + filePath);

        try
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(filePath, json);
            Debug.Log("Puntos de ubicación actualizados guardados en el archivo JSON.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al guardar el archivo: " + ex.Message);
        }
    }
}