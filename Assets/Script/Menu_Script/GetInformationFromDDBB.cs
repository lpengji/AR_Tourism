using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.IO;
using System;

public class GetInformationFromDDBB : MonoBehaviour
{
    public Information information;
    private string locationInformationPersistenceFileName = "locationInformationDDBB.json";
    private int informationId;
    private List<Information> allInformationList;

    IEnumerator Start()
    {
        // Obtener el ID de la información del PlayerPrefs
        informationId = PlayerPrefs.GetInt("locationInfo", -1);
        Debug.Log(informationId);

        // Cargar y leer el archivo JSON
        string filePath = Path.Combine(Application.streamingAssetsPath, locationInformationPersistenceFileName);

        // Verificar si la plataforma es Android
        if (Application.platform == RuntimePlatform.Android)
        {
            // Si es Android, cargar el archivo usando UnityWebRequest
            UnityWebRequest www = UnityWebRequest.Get(filePath);

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
                string locationInformation = www.downloadHandler.text;

                // Deserializar el JSON a una lista de objetos LocationPoint
                allInformationList = JsonUtility.FromJson<InformationWrapper>(locationInformation).informations;

                // Filtrar las informaciones hasta encontrar el id correcto
                this.information = allInformationList.Find(info => info.id == informationId);

                if (information != null)
                {
                    // Loguear la información encontrada
                    Debug.Log("Información encontrada:");
                    Debug.Log("ID: " + information.id);
                    Debug.Log("ImageURL: " + information.imageURL);
                    Debug.Log("DefaultInfo: " + information.defaultInfo);
                    foreach (var comment in information.comments)
                    {
                        Debug.Log("Comentario:");
                        Debug.Log("ContenidoComment: " + comment.contenidoComment);
                    }
                }
                else
                {
                    Debug.LogError("No se encontró información con el ID: " + informationId);
                }
            }
        }
        else
        {
            // Si no es Android, cargar el archivo directamente desde el sistema de archivos
            string locationInformation = File.ReadAllText(filePath);

            // Deserializar el JSON a una lista de objetos Information
            allInformationList = JsonUtility.FromJson<InformationWrapper>(locationInformation).informations;
            Debug.Log(locationInformation);
            Debug.Log(allInformationList);
            // Buscar la información con el ID correspondiente
            this.information = allInformationList.Find(info => info.id == informationId);

            // Verificar si se encontró la información
            if (information != null)
            {
                // Loguear la información encontrada
                Debug.Log("Información encontrada:");
                Debug.Log("ID: " + information.id);
                Debug.Log("ImageURL: " + information.imageURL);
                Debug.Log("DefaultInfo: " + information.defaultInfo);
                foreach (var comment in information.comments)
                {
                    if (!string.IsNullOrEmpty(comment.contenidoComment))
                    {
                        Debug.Log("Comentario:");
                        Debug.Log("ContenidoComment: " + comment.contenidoComment);
                    }
                }
            }
            else
            {
                Debug.LogError("No se encontró información con el ID: " + informationId);
            }

        }
    }
}
