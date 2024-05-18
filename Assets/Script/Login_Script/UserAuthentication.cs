using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.IO;
using System;

public class UserAuthentication : MonoBehaviour
{
    private string usersPersistenceFileName = "userDDBB.json"; // File name for storing user data
    private List<User> allUsersList; // List to store all user data
    public bool usersLoaded = false; // Flag to indicate if users are loaded

    void Start()
    {
        LoadUsers();
    }

    public void LoadUsers()
    {
        // Get the file path for the user JSON file
        string filePath;

#if UNITY_ANDROID && !UNITY_EDITOR
        // If it's Android, use the persistent data path
        filePath = Path.Combine(Application.persistentDataPath, usersPersistenceFileName);
#else
        // If it's not Android, use the streaming assets path
        filePath = Path.Combine(Application.streamingAssetsPath, usersPersistenceFileName);
#endif

        Debug.Log("File path: " + filePath);

        // Check if the platform is Android and if the file exists
        if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(LoadFileFromStreamingAssets(filePath));
        }
        else
        {
            // If it's not Android, load the file directly from the file system
            LoadFile(filePath);
        }
    }

    IEnumerator LoadFileFromStreamingAssets(string filePath)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, usersPersistenceFileName);
        UnityWebRequest www = UnityWebRequest.Get(streamingAssetsPath);
#else
        UnityWebRequest www = UnityWebRequest.Get("file://" + filePath);
#endif

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error loading user file: " + www.error);
        }
        else
        {
            string usersInformation = www.downloadHandler.text;
            ProcessUserInformation(usersInformation);
        }
    }

    void LoadFile(string filePath)
    {
        string usersInformation = File.ReadAllText(filePath);
        ProcessUserInformation(usersInformation);
    }

    void ProcessUserInformation(string usersInformation)
    {
        // Deserialize the JSON to a list of User objects
        allUsersList = JsonUtility.FromJson<UserWrapper>(usersInformation).users;
        usersLoaded = true;
    }

    // Method to authenticate the user
    public User AuthenticateUser(string username, string password)
    {
        // Check if the users are loaded correctly
        if (!usersLoaded)
        {
            Debug.Log("Error: Users are not loaded correctly.");
            return null;
        }

        // Find the user by username and password
        User authenticatedUser = allUsersList.Find(u => u.userName == username && u.userPassword == password);

        if (authenticatedUser != null)
        {
            return authenticatedUser; // Return the authenticated user
        }
        else
        {
            return null; // Return null if authentication fails
        }
    }

    public void UpdateUser(User updatedUser)
    {
        if (!usersLoaded)
        {
            Debug.LogError("Users not loaded, cannot update user.");
            return;
        }

        int index = allUsersList.FindIndex(u => u.userName == updatedUser.userName);
        if (index != -1)
        {
            allUsersList[index] = updatedUser;
            SaveUsersToFile();
        }
        else
        {
            Debug.LogError("User not found, cannot update.");
        }
    }

    void SaveUsersToFile()
    {
        // Convert the updated user list to JSON
        string json = JsonUtility.ToJson(new UserWrapper(allUsersList));

        // Get the file path for the user JSON file
        string filePath;

#if UNITY_ANDROID && !UNITY_EDITOR
        // If it's Android, use the persistent data path
        filePath = Path.Combine(Application.persistentDataPath, usersPersistenceFileName);
#else
        // If it's not Android, use the streaming assets path
        filePath = Path.Combine(Application.streamingAssetsPath, usersPersistenceFileName);
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

            // Write the JSON to the file
            File.WriteAllText(filePath, json);
            Debug.Log("User data saved to JSON file.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving user file: " + ex.Message);
        }
    }
}
