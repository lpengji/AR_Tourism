using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.IO;
using System;

public class ARInforDDBBManagement : MonoBehaviour
{
    private string arLocationInformationFileName = "arlocationinforamtionDDBB.json";
    public List<ARLocationInformation> arLocationInformations;

    [SerializeField]
    private VPS_Manager vpsManager;

    void Start()
    {
        string idsString = PlayerPrefs.GetString("arInformationIds", "");
        Debug.Log("#idsString: " + idsString);

        this.LoadARLocationInformations(idsString);
    }

    public void LoadARLocationInformations(string idsString)
    {
        if (string.IsNullOrEmpty(idsString))
        {
            Debug.LogWarning("No IDs provided for AR location information.");
            return;
        }

        string arLocationInformationsURL = Path.Combine(Application.streamingAssetsPath, arLocationInformationFileName);

#if UNITY_ANDROID && !UNITY_EDITOR
        string filePath = Path.Combine(Application.persistentDataPath, arLocationInformationFileName);
#else
        string filePath = arLocationInformationsURL;
#endif

        Debug.Log("File path: " + filePath);

        if (Application.platform == RuntimePlatform.Android && File.Exists(filePath))
        {
            LoadFile(filePath);
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(LoadFileFromStreamingAssets(arLocationInformationsURL));
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
            string arLocationInformationsText = www.downloadHandler.text;
            ProcessARLocationInformations(arLocationInformationsText);
        }
    }

    void LoadFile(string filePath)
    {
        string arLocationInformationsText = File.ReadAllText(filePath);
        ProcessARLocationInformations(arLocationInformationsText);
    }

    void ProcessARLocationInformations(string arLocationInformationsText)
    {
        // Obtener los IDs desde PlayerPrefs
        List<int> targetIds = GetTargetIdsFromPlayerPrefs();

        if (targetIds.Count == 0)
        {
            Debug.LogWarning("No valid IDs found in PlayerPrefs.");
            return;
        }

        arLocationInformations = JsonUtility.FromJson<ARLocationInformationWrapper>(arLocationInformationsText).arlocationinformation;

        // Filtrar arLocationInformations según los IDs extraídos de PlayerPrefs
        arLocationInformations = arLocationInformations.Where(info => targetIds.Contains(info.id)).ToList();

        foreach (var info in arLocationInformations)
        {
            Debug.Log("ID: " + info.Id +
                      ", Latitud: " + info.Latitud +
                      ", Longitud: " + info.Longitud +
                      ", Altitud: " + info.Altitud +
                      ", Información: " + info.Information);
        }
    }

    List<int> GetTargetIdsFromPlayerPrefs()
    {
        string idsString = PlayerPrefs.GetString("arInformationIds", "");
        List<int> ids = idsString.Split(',')
                                 .Where(id => !string.IsNullOrEmpty(id))
                                 .Select(int.Parse)
                                 .ToList();
        return ids;
    }

    public void AddNewARInformation(string newInformation)
    {
        // Crear una nueva instancia de ARLocationInformation con un ID único
        int newId = GenerateUniqueID();
        ARLocationInformation newInfo = new ARLocationInformation(newId, vpsManager.geospatialPose.Altitude, vpsManager.geospatialPose.Latitude, vpsManager.geospatialPose.Altitude, newInformation);

        // Agregar la nueva información a la lista
        arLocationInformations.Add(newInfo);

        // Guardar la información actualizada en el archivo
        SaveInformationToFile();

        vpsManager.PlaceObjects(arLocationInformations);
    }

    // Método para generar un ID único
    private int GenerateUniqueID()
    {
        // Obtener el último ID existente
        int lastId = 0;
        foreach (var info in arLocationInformations)
        {
            if (info.id > lastId)
            {
                lastId = info.id;
            }
        }

        return lastId + 1;
    }

    // Métodos para actualizar y eliminar la información AR
    public void UpdateARInformation(int id, string newInformation)
    {
        int index = arLocationInformations.FindIndex(info => info.id == id);
        if (index != -1)
        {
            arLocationInformations[index].Information = newInformation;
            SaveInformationToFile();
            vpsManager.PlaceObjects(arLocationInformations);
        }
        else
        {
            Debug.LogError("No se encontró la información para actualizar.");
        }
    }

    public void DeleteARInformation(int id)
    {
        int index = arLocationInformations.FindIndex(info => info.id == id);
        if (index != -1)
        {
            arLocationInformations.RemoveAt(index);
            SaveInformationToFile();
            vpsManager.PlaceObjects(arLocationInformations);
        }
        else
        {
            Debug.LogError("No se encontró la información para eliminar.");
        }
    }

    void SaveInformationToFile()
    {
        string json = JsonUtility.ToJson(new ARLocationInformationWrapper(arLocationInformations));
        string filePath;

#if UNITY_ANDROID && !UNITY_EDITOR
        filePath = Path.Combine(Application.persistentDataPath, arLocationInformationFileName);
#else
        filePath = Path.Combine(Application.streamingAssetsPath, arLocationInformationFileName);
#endif

        try
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            File.WriteAllText(filePath, json);
            Debug.Log("Información actualizada guardada en el archivo JSON.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al guardar el archivo: " + ex.Message);
        }
    }
}
