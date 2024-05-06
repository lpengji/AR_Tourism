using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class loginManager : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;

    public UserAuthentication userAuthentication;
    private string nextSceneName = "Map_Scene";
    public void OnClick()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        // Llama al método de autenticación del usuario y pasa los datos de entrada
        User authenticatedUser = userAuthentication.AuthenticateUser(username, password);

        if (authenticatedUser != null)
        {
            Debug.Log("Inicio de sesión exitoso para el usuario: " + authenticatedUser.userName);
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("Error de inicio de sesión. Nombre de usuario o contraseña incorrectos.");
        }
    }


}
