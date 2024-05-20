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

    public void LoadData()
    {
        // Get the JSON file path
        string filePath = GetFilePath();

        if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(LoadFileFromStreamingAssets());
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
        try
        {
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                ProcessData(jsonData);
            }
            else
            {
                Debug.LogError("File not found: " + filePath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error reading file: " + ex.Message);
        }
    }

    IEnumerator LoadFileFromStreamingAssets()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, recommendedLocationFileName);

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest www = UnityWebRequest.Get(filePath);
#else
        UnityWebRequest www = UnityWebRequest.Get("file://" + filePath);
#endif

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
        try
        {
            // Deserialize JSON to a list of RecommendedLocationList objects
            allRecommendedLists = JsonUtility.FromJson<RecommendedLocationWrapper>(jsonData).recommendedLocationLists;
            dataLoaded = true;

            // Log the data for debugging purposes
            Debug.Log("Loaded data: " + JsonUtility.ToJson(new RecommendedLocationWrapper(allRecommendedLists), true));
        }
        catch (Exception ex)
        {
            Debug.LogError("Error processing data: " + ex.Message);
        }
    }

    public void SaveData()
    {
        // Convert the list of recommended lists to JSON
        string json = JsonUtility.ToJson(new RecommendedLocationWrapper(allRecommendedLists));

        // Get the JSON file path
        string filePath = GetFilePath();

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