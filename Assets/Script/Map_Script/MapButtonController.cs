using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapButtonController : MonoBehaviour
{
    [SerializeField]
    private GameObject normalUserMenuPopUp;
    [SerializeField]
    private GameObject adminUserMenuPopUp;
    [SerializeField]
    private Button menuButton;
    [SerializeField]
    private Button closeMenuButton;
    private String logScene = "Log_scene";
    private User loggedUser;

    void Start()
    {
        // Obtener el rol del usuario logueado
        string userJson = PlayerPrefs.GetString("AuthenticatedUser");
        loggedUser = JsonUtility.FromJson<User>(userJson);


    }

    // Función para abrir el menú emergente
    public void OpenMenuPopup()
    {
        menuButton.gameObject.SetActive(false);
        closeMenuButton.gameObject.SetActive(true);

        // Mostrar el menú correspondiente según el rol del usuario
        if (loggedUser.rol == "normal")
        {
            normalUserMenuPopUp.GetComponentInChildren<TextMeshProUGUI>().text = "Bienvenido, \n" + loggedUser.userName + "!";
            normalUserMenuPopUp.SetActive(true);
        }
        else if (loggedUser.rol == "admin")
        {
            adminUserMenuPopUp.GetComponentInChildren<TextMeshProUGUI>().text = "Bienvenido, \n" + loggedUser.userName + "!";
            adminUserMenuPopUp.SetActive(true);
        }
    }

    // Función para cerrar el menú emergente
    public void CloseMenuPopup()
    {
        menuButton.gameObject.SetActive(true);
        closeMenuButton.gameObject.SetActive(false);

        // Ocultar el menú correspondiente según el rol del usuario
        if (loggedUser.rol == "normal")
        {
            normalUserMenuPopUp.SetActive(false);
        }
        else if (loggedUser.rol == "admin")
        {
            adminUserMenuPopUp.SetActive(false);
        }
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey("AuthenticatedUser");
        SceneManager.LoadScene(logScene);
    }
}