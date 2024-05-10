using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mapbox.Examples;
using System.Linq;
using System.IO;

public class databaseManager : MonoBehaviour
{
    [SerializeField]
    private SpawnOnMap spawnOnMap;

    // Nombre del archivo JSON en StreamingAssets
    private string locationPointsPersistenceFileName = "locationPointDDBB.json";

    private List<LocationPoint> locationPoints;
    private User loggedUser;
    IEnumerator Start()
    {
        // Obtener la información del usuario almacenada en PlayerPrefs
        string userJson = PlayerPrefs.GetString("AuthenticatedUser");
        this.loggedUser = JsonUtility.FromJson<User>(userJson);

        Debug.Log("loggedUser: " + loggedUser.userID + " , " + loggedUser.userName);

        // Construir la ruta completa al archivo JSON en StreamingAssets
        string locationPointsURL = Path.Combine(Application.streamingAssetsPath, locationPointsPersistenceFileName);

        // Verificar si la plataforma es Android
        if (Application.platform == RuntimePlatform.Android)
        {
            // Si es Android, cargar el archivo usando UnityWebRequest
            UnityWebRequest www = UnityWebRequest.Get(locationPointsURL);

            // Esperar a que se complete la descarga
            yield return www.SendWebRequest();

            // Verificar si hubo algún error
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al cargar el archivo: " + www.error);
            }
            else
            {
                // Obtener el contenido del archivo JSON
                string locationPointsInformation = www.downloadHandler.text;

                // Deserializar el JSON a una lista de objetos LocationPoint
                locationPoints = JsonUtility.FromJson<LocationPointsWrapper>(locationPointsInformation).locationPoints;

                // Filtrar los puntos no creados por el usuario
                var nonCreatedLocationPoints = locationPoints.Where(point => !point.isCreated);

                // Iterar sobre los puntos no creados y crear objetos en el mapa
                foreach (var point in nonCreatedLocationPoints)
                {
                    spawnOnMap.InstantiateNormalLocationPointOnMap(point);
                }
            }
        }
        else
        {
            // Si no es Android, cargar el archivo directamente desde el sistema de archivos
            string locationPointsInformation = File.ReadAllText(locationPointsURL);

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
            foreach (var point in createdLocationPoints)
            {
                if (loggedUser.createdLocations.Contains(point.Id))
                {
                    spawnOnMap.InstantiateMyLocationPointOnMap(point);
                }
            }
        }
    }
}
