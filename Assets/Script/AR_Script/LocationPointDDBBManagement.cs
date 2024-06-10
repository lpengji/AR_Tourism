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
        // Construir la ruta completa al archivo JSON en StreamingAssets
        string locationPointsURL = Path.Combine(Application.streamingAssetsPath, locationPointsPersistenceFileName);

#if UNITY_ANDROID && !UNITY_EDITOR
        // Si es Android, usar la ruta persistente en el sistema de archivos específica de Android
        string filePath = Path.Combine(Application.persistentDataPath, locationPointsPersistenceFileName);
#else
        // Si no es Android, usar la ruta en la carpeta de streamingAssets
        string filePath = locationPointsURL;
#endif

        Debug.Log("File path: " + filePath);

        // Verificar si la plataforma es Android y si el archivo existe
        if (Application.platform == RuntimePlatform.Android && File.Exists(filePath))
        {
            // Si es Android y el archivo existe, cargar el archivo directamente desde el sistema de archivos
            LoadFile(filePath);
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            // Si es Android y el archivo no existe en persistentDataPath, intentar cargar desde streamingAssets
            StartCoroutine(LoadFileFromStreamingAssets(locationPointsURL));
        }
        else
        {
            // Si no es Android, cargar el archivo directamente desde el sistema de archivos
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
        // Deserializar el JSON a una lista de objetos LocationPoint
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
            locationPoint.ArInformationId = arInformationIds.ToArray();
            SaveLocationPointsToFile();
        }
        else
        {
            Debug.LogError("LocationPoint not found with ID: " + locationPointId);
        }
    }


    void SaveLocationPointsToFile()
    {
        // Convertir la lista de puntos de ubicación a JSON
        string json = JsonUtility.ToJson(new LocationPointsWrapper(locationPoints));

        // Obtener la ruta del archivo JSON
        string filePath;

#if UNITY_ANDROID && !UNITY_EDITOR
        // Si es Android, usar la ruta persistente en el sistema de archivos específica de Android
        filePath = Path.Combine(Application.persistentDataPath, locationPointsPersistenceFileName);
#else
        // Si no es Android, usar la ruta en la carpeta de streamingAssets
        filePath = Path.Combine(Application.streamingAssetsPath, locationPointsPersistenceFileName);
#endif

        Debug.Log("File path: " + filePath);

        try
        {
            // Ensure the directory exists
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Escribir el JSON en el archivo
            File.WriteAllText(filePath, json);
            Debug.Log("Puntos de ubicación actualizados guardados en el archivo JSON.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al guardar el archivo: " + ex.Message);
        }
    }
}
