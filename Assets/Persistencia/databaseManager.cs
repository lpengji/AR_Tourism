using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mapbox.Examples;
using System.Linq;
using System.IO;
using System;
using Mapbox.Utils;

public class databaseManager : MonoBehaviour
{
    [SerializeField]
    private SpawnOnMap spawnOnMap;
    private string locationPointsPersistenceFileName = "locationPointDDBB.json";
    private List<LocationPoint> locationPoints;
    private User loggedUser;
    [SerializeField]
    private LocationPointInformationDataManagement locationPointInformationDataManagement;
    [SerializeField]
    private UserAuthentication userAuthentication;
    public bool locationPointsLoaded = false;

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

        // Filtrar los puntos no creados por el usuario
        var nonCreatedLocationPoints = locationPoints.Where(point => !point.isCreated);
        var createdLocationPoints = locationPoints.Where(point => point.isCreated);

        // Iterar sobre los puntos no creados y crear objetos en el mapa
        foreach (var point in nonCreatedLocationPoints)
        {
            if (loggedUser.likedLocations.Contains(point.Id))
            {
                spawnOnMap.InstantiateLikedLocationPointOnMap(point);
            }
            else
            {
                spawnOnMap.InstantiateNormalLocationPointOnMap(point);
            }
        }

        // Iterar sobre los puntos creados y crear objetos en el mapa
        foreach (var point in createdLocationPoints)
        {
            if (loggedUser.createdLocations.Contains(point.Id))
            {
                Debug.Log("estoy pasando por created loccation ");
                spawnOnMap.InstantiateMyLocationPointOnMap(point);
            }
        }

        locationPointsLoaded = true;
    }

    public void AddNewLocationPoint(float latitud, float longitud, float altitud, int createdByUserID, bool isCreated)
    {
        if (locationPoints == null)
        {
            locationPoints = new List<LocationPoint>();
        }

        // Generar IDs únicos para el nuevo punto de ubicación
        int newId = GenerateUniqueId();
        int newInformationId = newId;

        int[] emptyIdList = new int[0];

        // Crear un nuevo punto de ubicación con los parámetros proporcionados
        LocationPoint newPoint = new LocationPoint(newId, latitud, longitud, createdByUserID, newInformationId, emptyIdList, isCreated);

        // Agregar el nuevo punto a la lista de puntos de ubicación
        locationPoints.Add(newPoint);
        locationPointInformationDataManagement.AddNewInformation(newInformationId);

        this.loggedUser.CreatedLocations.Add(newId);
        this.userAuthentication.UpdateUser(loggedUser);
        string userJson = JsonUtility.ToJson(loggedUser);
        PlayerPrefs.SetString("AuthenticatedUser", userJson);
        PlayerPrefs.Save();

        // Guardar los cambios en el archivo JSON
        SaveLocationPointsToFile();
        this.LoadLocationPoints();
    }

    private int GenerateUniqueId()
    {
        int lastId = 0;
        foreach (LocationPoint point in locationPoints)
        {
            if (point.Id > lastId)
            {
                lastId = point.Id;
            }
        }
        return lastId + 1;
    }

    public void EditLocationPoint(LocationPoint editedPoint)
    {
        // Buscar el punto de ubicación a editar
        int index = locationPoints.FindIndex(point => point.Id == editedPoint.Id);
        if (index != -1)
        {
            locationPoints[index] = editedPoint;
            SaveLocationPointsToFile();
        }
        else
        {
            Debug.LogError("No se pudo encontrar el punto de ubicación a editar.");
        }
    }

    public void DeleteLocationPoint(int pointId)
    {
        // Buscar el punto de ubicación a eliminar
        LocationPoint pointToDelete = locationPoints.Find(point => point.Id == pointId);
        if (pointToDelete != null)
        {
            locationPoints.Remove(pointToDelete);
            SaveLocationPointsToFile();
        }
        else
        {
            Debug.LogError("No se pudo encontrar el punto de ubicación a eliminar.");
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

