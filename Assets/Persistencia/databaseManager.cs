using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mapbox.Examples;
using UnityEngine;

public class databaseManager : MonoBehaviour
{
    // conexión con otras funcionalidades del app
    [SerializeField]
    private SpawnOnMap spawnOnMap;

    // Ruta al archivo JSON
    private string locationPointsURL = "Assets/Persistencia/bbddAr_Tour/locationPointDDBB.json";

    // Variables para almacenar los objetos LocationPoint deserializados del JSON
    private List<LocationPoint> locationPoints;

    void Start()
    {
        this.InitializeLocationPoints();
    }

    void Update()
    {

    }

    private void InitializeLocationPoints()
    {
        // Cargar el contenido del archivo JSON
        string locationPointsInformation = File.ReadAllText(locationPointsURL);

        // Deserializar el JSON a una lista de objetos LocationPoint
        locationPoints = JsonUtility.FromJson<LocationPointsWrapper>(locationPointsInformation).locationPoints;

        // Verificar si la deserialización fue exitosa
        if (locationPoints == null)
        {
            Debug.LogError("Error al deserializar el JSON.");
            return;
        }

        foreach (var point in locationPoints)
        {
            Debug.Log("EN DATABASE " + "ID: " + point.Id + ", Latitud-Longitud: " + point.ConcatenarLatitudLongitud() + ", Altitud: " + point.Altitud + ", Creado por Usuario ID: " + point.CreatedByUserID + ", ID de Información: " + point.InformationId);

            this.spawnOnMap.InstantiateNormalLocationPointOnMap(point);
        }
    }
}
