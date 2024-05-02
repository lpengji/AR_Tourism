using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mapbox.Examples;
using UnityEngine;
using System.Linq;

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
        this.InitializeNormalAndLikedLocationPoints();

        // this.InitializeMyLocationPoints();
    }

    void Update()
    {

    }

    private void InitializeNormalAndLikedLocationPoints()
    {
        // Cargar el contenido del archivo JSON
        string locationPointsInformation = File.ReadAllText(locationPointsURL);

        // Deserializar el JSON a una lista de objetos LocationPoint
        this.locationPoints = JsonUtility.FromJson<LocationPointsWrapper>(locationPointsInformation).locationPoints;

        // Verificar si la deserialización fue exitosa
        if (this.locationPoints == null)
        {
            Debug.LogError("Error al deserializar el JSON.");
            return;
        }

        // sacar los puntos creados por el usuario
        var nonCreatedLocationPoints = locationPoints.Where(point => !point.isCreated);
    TODO:
        // cuando tengamos la parte de usuarios, podemos sacar la lista de nonCreatedLocationPoints. crear una funcion que compare los id de la "lista de gustado" del usuario con 
        // la lista nonCreatedLocationPoints, y los que coincidan, sacarlos de locationPoints y meterlos en una lista nueva
        // mas o menos asi likedLocations = removeLikedPoints(nonCreatedLocationPoints), y después esta lista los pasamos a spanOnMap.InstantiateLikedLocationPointOnMap()

        foreach (var point in nonCreatedLocationPoints)
        {
            Debug.Log("EN DATABASE " + "ID: " + point.Id + ", Latitud-Longitud: " + point.ConcatenarLatitudLongitud() + ", Altitud: " + point.Altitud +
            ", Creado por Usuario ID: " + point.CreatedByUserID + ", ID de Información: " + point.InformationId + "is Created: " + point.isCreated);

            this.spawnOnMap.InstantiateNormalLocationPointOnMap(point);
        }
    }
}
