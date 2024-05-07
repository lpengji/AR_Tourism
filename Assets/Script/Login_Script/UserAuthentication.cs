using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.IO;
using System;

public class UserAuthentication : MonoBehaviour
{
    private string usersPersistenceFileName = "userDDBB.json"; // Nombre del archivo que contiene la información de los usuarios
    private List<User> allUsersList; // Lista para almacenar la información de todos los usuarios
    public bool usersLoaded = false; // Variable para indicar si se han cargado los usuarios correctamente

    void Start()
    {
        LoadUsers();
    }
    public void LoadUsers()
    {
        // Cargar y leer el archivo JSON que contiene la información de los usuarios
        string filePath = Path.Combine(Application.streamingAssetsPath, usersPersistenceFileName);

        // Verificar si la plataforma es Android
        if (Application.platform == RuntimePlatform.Android)
        {
            // Si es Android, cargar el archivo usando UnityWebRequest
            UnityWebRequest www = UnityWebRequest.Get(filePath);

            // Esperar a que se complete la descarga
            StartCoroutine(DownloadFile(www));
        }
        else
        {
            // Si no es Android, cargar el archivo directamente desde el sistema de archivos
            LoadFile(filePath);
        }
    }

    IEnumerator DownloadFile(UnityWebRequest www)
    {
        // Esperar a que se complete la descarga
        yield return www.SendWebRequest();

        // Verificar si hubo algún error
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al cargar el archivo de usuarios en Android: " + www.error);
        }
        else
        {
            // Obtener el contenido del archivo JSON
            string usersInformation = www.downloadHandler.text;

            // Deserializar el JSON a una lista de objetos User
            allUsersList = JsonUtility.FromJson<UserWrapper>(usersInformation).users;

            // Indicar que los usuarios se han cargado correctamente
            usersLoaded = true;
        }
    }

    void LoadFile(string filePath)
    {
        Debug.Log("filepath" + filePath);
        // Cargar el archivo directamente desde el sistema de archivos
        string usersInformation = File.ReadAllText(filePath);
        Debug.Log("usersInformation" + usersInformation);
        // Deserializar el JSON a una lista de objetos User
        allUsersList = JsonUtility.FromJson<UserWrapper>(usersInformation).users;

        // Indicar que los usuarios se han cargado correctamente
        usersLoaded = true;
    }

    // Método para autenticar al usuario
    public User AuthenticateUser(string username, string password)
    {
        // Verificar si los usuarios se han cargado correctamente
        if (!usersLoaded)
        {
            Debug.Log("Error: Los usuarios no se han cargado correctamente.");
            return null; // Devolver null si los usuarios no se han cargado correctamente
        }

        // Buscar el usuario por nombre de usuario y contraseña
        User authenticatedUser = allUsersList.Find(u => u.userName == username && u.userPassword == password);

        // Si se encuentra un usuario con las credenciales proporcionadas, se considera autenticado
        if (authenticatedUser != null)
        {
            return authenticatedUser; // Devolver el usuario autenticado
        }
        else
        {
            // Si no se encuentra un usuario con las credenciales proporcionadas, la autenticación falla
            return null; // Devolver null si la autenticación falla
        }
    }

}
