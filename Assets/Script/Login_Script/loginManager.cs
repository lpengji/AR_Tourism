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
    public GameObject errorPopUp;
    private string nextSceneName = "Map_Scene";
    public void OnClick()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        // Llama al método de autenticación del usuario y pasa los datos de entrada
        User authenticatedUser = userAuthentication.AuthenticateUser(username, password);
        //Debug.Log("usuario extraido" + authenticatedUser);
        if (authenticatedUser != null)
        {
            // Guardar la información del usuario en PlayerPrefs
            string userJson = JsonUtility.ToJson(authenticatedUser);
            PlayerPrefs.SetString("AuthenticatedUser", userJson);
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            errorPopUp.SetActive(true);
            Debug.Log("Error de inicio de sesión. Nombre de usuario o contraseña incorrectos.");
        }
    }

    public void ClosePopUp()
    {
        errorPopUp.SetActive(false);
    }


}
