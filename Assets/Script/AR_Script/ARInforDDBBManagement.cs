using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mapbox.Examples;
using System.Linq;
using System.IO;
using System;
using Mapbox.Utils;

public class ARInforDDBBManagement : MonoBehaviour
{
    private string arLocationInformationFileName = "arlocationinforamtionDDBB.json";
    public List<ARLocationInformation> arLocationInformations;

    void Start()
    {
        LoadARLocationInformations();
    }

    void LoadARLocationInformations()
    {
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
        List<int> targetIds = GetTargetIdsFromPlayerPrefs();
        arLocationInformations = JsonUtility.FromJson<ARLocationInformationWrapper>(arLocationInformationsText).arlocationinformation;

        // Filtrar arLocationInformations según los IDs extraídos de PlayerPrefs
        arLocationInformations = arLocationInformations.Where(info => targetIds.Contains(info.id)).ToList();

        foreach (var info in arLocationInformations)
        {
            Debug.Log("ID: " + info.id +
                      ", Latitud: " + info.latitud +
                      ", Longitud: " + info.longitud +
                      ", Altitud: " + info.altitud +
                      ", Información: " + info.information);
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
}