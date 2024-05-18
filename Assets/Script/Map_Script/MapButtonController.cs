using System;
using System.Collections;
using System.Collections.Generic;
using GeoCoordinatePortable;
using Mapbox.Examples;
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
    [SerializeField]
    private databaseManager databaseManager;

    private LocationStatus playerLocation;
    void Start()
    {
        this.playerLocation = GameObject.Find("LocationCoordinatesCanvas").GetComponent<LocationStatus>();

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

    public void CreateNewNormalLocationPoint()
    {
        databaseManager.AddNewLocationPoint((float)playerLocation.getLatitude(), (float)playerLocation.getLongitude(), 0, this.loggedUser.userID, false);
        this.CloseMenuPopup();

    }
    public void CreateNewMyLocationPoint()
    {
        databaseManager.AddNewLocationPoint((float)playerLocation.getLatitude(), (float)playerLocation.getLongitude(), 0, this.loggedUser.userID, true);
        this.CloseMenuPopup();
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey("AuthenticatedUser");
        SceneManager.LoadScene(logScene);
    }
}