using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapButtonController : MonoBehaviour
{
    [SerializeField]
    private GameObject menuPopUp;
    [SerializeField]
    private Button menuButton;
    [SerializeField]
    private Button closeMenuButton;
    private String logScene = "Log_scene";

    // Función para abrir el menú emergente
    public void OpenMenuPopup()
    {
        // Activar la ventana del menú emergente
        menuPopUp.SetActive(true);

        // Ocultar el botón del menú
        menuButton.gameObject.SetActive(false);

        // Mostrar el botón para cerrar el menú
        closeMenuButton.gameObject.SetActive(true);
    }

    // Función para cerrar el menú emergente
    public void CloseMenuPopup()
    {
        // Desactivar la ventana del menú emergente
        menuPopUp.SetActive(false);

        // Ocultar el botón para cerrar el menú
        closeMenuButton.gameObject.SetActive(false);

        // Mostrar el botón del menú
        menuButton.gameObject.SetActive(true);
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey("AuthenticatedUser");
        SceneManager.LoadScene(logScene);
    }
}
