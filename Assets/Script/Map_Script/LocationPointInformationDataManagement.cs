using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.IO;
using System;

public class LocationPointInformationDataManagement : MonoBehaviour
{
    private string locationInformationPersistenceFileName = "locationInformationDDBB.json";
    private List<Information> allInformationList;
    public bool informationLoaded = false;

    public void LoadInformation()
    {
        // Obtener la ruta del archivo JSON
        string filePath;

#if UNITY_ANDROID && !UNITY_EDITOR
    // Si es Android, usar la ruta persistente en el sistema de archivos específica de Android
    filePath = Path.Combine(Application.persistentDataPath, locationInformationPersistenceFileName);
#else
        // Si no es Android, usar la ruta en la carpeta de streamingAssets
        filePath = Path.Combine(Application.streamingAssetsPath, locationInformationPersistenceFileName);
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
            StartCoroutine(LoadFileFromStreamingAssets(filePath));
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
            string locationInformation = www.downloadHandler.text;
            ProcessInformation(locationInformation);
        }
    }

    void LoadFile(string filePath)
    {
        string locationInformation = File.ReadAllText(filePath);
        ProcessInformation(locationInformation);
    }

    void ProcessInformation(string locationInformation)
    {
        // Deserializar el JSON a una lista de objetos Information
        allInformationList = JsonUtility.FromJson<InformationWrapper>(locationInformation).informations;
        informationLoaded = true;
    }

    // Método para agregar un nuevo Information con solo el ID
    public void AddNewInformation(int id)
    {
        this.LoadInformation();

        if (allInformationList == null)
        {
            allInformationList = new List<Information>();
        }

        // Crear un nuevo Information con valores predeterminados
        Information newInformation = new Information(id, "", "", new List<Comment>());

        // Agregar el nuevo Information a la lista
        allInformationList.Add(newInformation);

        // Guardar los cambios en el archivo JSON
        SaveInformationToFile();
    }


    // guardar los cambios en la bbdd
    void SaveInformationToFile()
    {
        // Convertir la lista de información actualizada a JSON
        string json = JsonUtility.ToJson(new InformationWrapper(allInformationList));

        // Obtener la ruta del archivo JSON
        string filePath;

#if UNITY_ANDROID && !UNITY_EDITOR
    // Si es Android, usar la ruta persistente en el sistema de archivos específica de Android
    filePath = Path.Combine(Application.persistentDataPath, locationInformationPersistenceFileName);
#else
        // Si no es Android, usar la ruta en la carpeta de streamingAssets
        filePath = Path.Combine(Application.streamingAssetsPath, locationInformationPersistenceFileName);
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
            Debug.Log("Información actualizada guardada en el archivo JSON.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al guardar el archivo: " + ex.Message);
        }
    }



}
