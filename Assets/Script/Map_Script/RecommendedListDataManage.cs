using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class RecommendedListDataManage : MonoBehaviour
{
    private string recommendedLocationFileName = "recomendedLocationPointDDBB.json";
    public List<RecommendedLocationList> allRecommendedLists;
    public bool dataLoaded = false;

    void Start()
    {
        LoadData();
    }

    public void LoadData()
    {
        // Get the JSON file path
        string filePath = GetFilePath();
        Debug.Log("File path: " + filePath);


        if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(LoadFileFromStreamingAssets(filePath));
        }
        else
        {
            LoadFile(filePath);
        }
    }

    string GetFilePath()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return Path.Combine(Application.persistentDataPath, recommendedLocationFileName);
#else
        return Path.Combine(Application.streamingAssetsPath, recommendedLocationFileName);
#endif
    }

    void LoadFile(string filePath)
    {
        string jsonData = File.ReadAllText(filePath);
        ProcessData(jsonData);
    }

    IEnumerator LoadFileFromStreamingAssets(string filePath)
    {
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error loading file: " + www.error);
        }
        else
        {
            string jsonData = www.downloadHandler.text;
            ProcessData(jsonData);
        }
    }

    void ProcessData(string jsonData)
    {
        // Deserialize JSON to a list of RecommendedLocationList objects
        allRecommendedLists = JsonUtility.FromJson<RecommendedLocationWrapper>(jsonData).recommendedLists;
        dataLoaded = true;
    }



    public void SaveData()
    {
        // Convert the list of recommended lists to JSON
        string json = JsonUtility.ToJson(new RecommendedLocationWrapper(allRecommendedLists));

        // Get the JSON file path
        string filePath = GetFilePath();
        Debug.Log("File path: " + filePath);

        try
        {
            // Ensure the directory exists
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Write the JSON data to the file
            File.WriteAllText(filePath, json);
            Debug.Log("Recommended location lists saved to JSON file.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving file: " + ex.Message);
        }
    }
}