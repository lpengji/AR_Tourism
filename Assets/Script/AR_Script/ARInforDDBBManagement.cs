using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.IO;
using System;
using TMPro;

public class ARInforDDBBManagement : MonoBehaviour
{
    private string arLocationInformationFileName = "arlocationinforamtionDDBB.json";
    public List<ARLocationInformation> arLocationInformations;
    [SerializeField]
    private VPS_Manager vpsManager;
    [SerializeField]
    private LocationPointDDBBManagement databaseManager;
    private int currentLocationPointId;
    [SerializeField]
    private TextMeshProUGUI aRPrefabAvaiableText;

    void Start()
    {
        string idsString = PlayerPrefs.GetString("arInformationIds", "");
        currentLocationPointId = PlayerPrefs.GetInt("locationInfo");
        Debug.Log("#idsString: " + idsString);
        LoadARLocationInformations(idsString);
    }

    public void LoadARLocationInformations(string idsString)
    {
        if (string.IsNullOrEmpty(idsString))
        {
            Debug.LogWarning("No IDs provided for AR location information.");
            UpdateARPrefabAvaiableText();
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
            Debug.Log("##### pasando por ProcessARLocationInformations #####");
            string arLocationInformationsText = www.downloadHandler.text;
            ProcessARLocationInformations(arLocationInformationsText);
        }
    }

    void LoadFile(string filePath)
    {
        string arLocationInformationsText = File.ReadAllText(filePath);
        Debug.Log($"File Content: {arLocationInformationsText}");
        ProcessARLocationInformations(arLocationInformationsText);
        Debug.Log("##### pasando por LoadFile #####");
    }

    void ProcessARLocationInformations(string arLocationInformationsText)
    {
        List<int> targetIds = GetTargetIdsFromPlayerPrefs();

        if (targetIds.Count == 0)
        {
            Debug.LogWarning("No valid IDs found in PlayerPrefs.");
            UpdateARPrefabAvaiableText();
            return;
        }

        arLocationInformations = JsonUtility.FromJson<ARLocationInformationWrapper>(arLocationInformationsText).arlocationinformation;
        arLocationInformations = arLocationInformations.Where(info => targetIds.Contains(info.Id)).ToList();

        Debug.Log($"Found {arLocationInformations.Count} AR locations to place.");
        foreach (var info in arLocationInformations)
        {
            Debug.Log($"AR Location: ID={info.Id}, Lat={info.Latitud}, Lon={info.Longitud}, Alt={info.Altitud}, Info={info.Information}");
        }

        UpdateARPrefabAvaiableText();
        vpsManager.Instantiate();
    }

    void UpdateARPrefabAvaiableText()
    {
        if (arLocationInformations == null || arLocationInformations.Count == 0)
        {
            aRPrefabAvaiableText.text = "NO HAY INFORMACIÓN DISPONIBLE";
        }
        else
        {
            aRPrefabAvaiableText.text = $"INFORMACIÓN DISPONIBLE: \n{arLocationInformations.Count}";
        }
    }

    List<int> GetTargetIdsFromPlayerPrefs()
    {
        string idsString = PlayerPrefs.GetString("arInformationIds", "");
        if (string.IsNullOrEmpty(idsString))
        {
            Debug.LogWarning("idsString is empty in PlayerPrefs.");
            return new List<int>();
        }
        List<int> ids = idsString.Split(',')
                                 .Where(id => !string.IsNullOrEmpty(id))
                                 .Select(int.Parse)
                                 .ToList();
        Debug.Log($"Retrieved targetIds from PlayerPrefs: {string.Join(", ", ids)}");
        return ids;
    }

    public void AddNewARInformation(string newInformation)
    {
        int newId = GenerateUniqueID();
        ARLocationInformation newInfo = new ARLocationInformation(newId, vpsManager.geospatialPose.Latitude, vpsManager.geospatialPose.Longitude, vpsManager.geospatialPose.Altitude, newInformation);
        arLocationInformations.Add(newInfo);
        databaseManager.AddARInformation(currentLocationPointId, newId);

        // Añadir el nuevo ID a PlayerPrefs
        List<int> ids = GetTargetIdsFromPlayerPrefs();
        ids.Add(newId);
        PlayerPrefs.SetString("arInformationIds", string.Join(",", ids));
        PlayerPrefs.Save();

        SaveInformationToFile();
        UpdateARPrefabAvaiableText();
        vpsManager.Instantiate();
    }

    private int GenerateUniqueID()
    {
        if (arLocationInformations.Count == 0)
        {
            return 1;
        }
        int lastId = arLocationInformations.Max(info => info.Id);
        return lastId + 1;
    }

    public void UpdateARInformation(int id, string newInformation)
    {
        int index = arLocationInformations.FindIndex(info => info.Id == id);
        if (index != -1)
        {
            arLocationInformations[index].Information = newInformation;
            SaveInformationToFile();
            UpdateARPrefabAvaiableText();
            vpsManager.Instantiate();
        }
        else
        {
            Debug.LogError("No se encontró la información para actualizar.");
        }
    }

    public void DeleteARInformation(int id)
    {
        Debug.Log("#id del comentario a delete: " + id);

        int index = arLocationInformations.FindIndex(info => info.Id == id);
        Debug.Log("#index: " + index);

        if (index != -1)
        {
            arLocationInformations.RemoveAt(index);
            Debug.Log("#length de la lista de arprefab despues del delete: " + arLocationInformations.Count);

            databaseManager.RemoveARInformation(currentLocationPointId, id);

            // Eliminar el ID de PlayerPrefs
            List<int> ids = GetTargetIdsFromPlayerPrefs();
            ids.Remove(id);
            PlayerPrefs.SetString("arInformationIds", string.Join(",", ids));
            PlayerPrefs.Save();

            SaveInformationToFile();
            UpdateARPrefabAvaiableText();
            vpsManager.Instantiate();
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
