using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Data.Common;

public class ARButtonController : MonoBehaviour
{

    [SerializeField]
    private Button arMenuButton;
    [SerializeField]
    private GameObject ARMenuCanvas;
    [SerializeField]
    private Button closeArMenuButton;
    private User loggedUser;
    [SerializeField]
    private Button exitARModeButton;
    [SerializeField]
    private Button createARInfoButton;
    [SerializeField]
    private GameObject editARInformationCanvas;
    [SerializeField]
    private Button aceptEditionButton;
    [SerializeField]
    private Button cancelEditionButton;
    [SerializeField]
    private TMP_InputField editInformationField;
    [SerializeField]
    private Button editButton;
    [SerializeField]
    private Button deleteButton;
    [SerializeField]
    private Button closeARInformationButton;
    [SerializeField]
    private GameObject displayARInformationCanvas;
    [SerializeField]
    private ARInforDDBBManagement aRInforDDBBManagement;
    private int currentLocationPointId;
    private int aRInformationId;

    void Start()
    {
        // Obtener el rol del usuario logueado
        string userJson = PlayerPrefs.GetString("AuthenticatedUser");
        loggedUser = JsonUtility.FromJson<User>(userJson);
        this.currentLocationPointId = PlayerPrefs.GetInt("locationInfo");
    }

    public void OpenMenuPopup()
    {
        arMenuButton.gameObject.SetActive(false);
        closeArMenuButton.gameObject.SetActive(true);

        if (loggedUser.rol == "admin" || loggedUser.createdLocations.Contains(currentLocationPointId))
        {
            createARInfoButton.gameObject.SetActive(true);
        }
        ARMenuCanvas.SetActive(true);
    }
    public void CloseMenuPopup()
    {
        arMenuButton.gameObject.SetActive(true);
        closeArMenuButton.gameObject.SetActive(false);

        if (loggedUser.rol == "admin")
        {
            createARInfoButton.gameObject.SetActive(false);
        }
        ARMenuCanvas.SetActive(false);
    }

    public void ExitARMode()
    {
        SceneManager.LoadScene("Menu_Scene");
    }

    public void OpenAREditCanvas()
    {
        editARInformationCanvas.SetActive(true);
    }

    public void CancelEdit()
    {
        CleanInputFields();
        this.CloseMenuPopup();
        editARInformationCanvas.SetActive(false);
    }
    private void CleanInputFields()
    {
        editInformationField.text = "";
    }



    // este método tiene que ser craedo por método cuando se crea el OpenARInformationCanvas, para 
    // poder pasarle el id y la informacion 
    public void MoveToEdit(string information, int id)
    {
        editInformationField.text = information;
        editARInformationCanvas.SetActive(true);
        this.aRInformationId = id;
    }
    public void AceptEdit()
    {
        if (editInformationField.text != "")
        {
            if (aRInformationId != 0)
            {
                aRInforDDBBManagement.UpdateARInformation(aRInformationId, editInformationField.text);
            }
            else
            {
                aRInforDDBBManagement.AddNewARInformation(editInformationField.text);
            }
        }

        editARInformationCanvas.SetActive(false);
        editInformationField.text = "";
        aRInformationId = 0;
        this.CloseMenuPopup();
    }
    // este método tiene que ser craedo por método cuando se crea el OpenARInformationCanvas, para 
    // poder pasarle el id y la informacion 
    public void DeleteARInformation(int id)
    {
        // Eliminar la información en la base de datos o sistema correspondiente
        aRInforDDBBManagement.DeleteARInformation(id);
        displayARInformationCanvas.SetActive(false);
    }
    public void ExitARInformationCanvas()
    {
        displayARInformationCanvas.SetActive(false);
    }
    // fijar en cómo se ha hecho en le menú del menu xd

}
