using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necesario para TextMeshProUGUI
using UnityEngine.UI; // Necesario para los botones

public class OpenARInformationCanvas : MonoBehaviour
{
    private ARLocationInformationObj aRLocationInformationObj;
    private GameObject displayARInformationCanvas;

    // Campos para el texto y los botones
    private TextMeshProUGUI defaultRecommendedBoxText;
    private Button deleteButton;
    private Button editButton;
    private ARButtonController aRButtonController;

    void Start()
    {
        aRLocationInformationObj = GetComponent<ARLocationInformationObj>();
        aRButtonController = GetComponent<ARButtonController>();

        if (displayARInformationCanvas != null && !displayARInformationCanvas.activeSelf)
        {
            displayARInformationCanvas.SetActive(true);
        }

        // Buscar el GameObject "DisplayARInformationCanvas" en la escena
        if (displayARInformationCanvas == null)
        {
            displayARInformationCanvas = GameObject.Find("DisplayARInformationCanvas");
            displayARInformationCanvas.SetActive(false);
        }

        if (displayARInformationCanvas == null)
        {
            Debug.LogError("DisplayARInformationCanvas no encontrado en la escena. Por favor asegúrate de que existe.");
        }
        else
        {
            // Buscar el componente de texto "DefaultRecommendedBox" dentro de DisplayARInformationCanvas
            Transform defaultRecommendedBoxTransform = displayARInformationCanvas.transform.Find("DefaultRecommendedBox");
            if (defaultRecommendedBoxTransform != null)
            {
                defaultRecommendedBoxText = defaultRecommendedBoxTransform.GetComponent<TextMeshProUGUI>();
                if (defaultRecommendedBoxText == null)
                {
                    Debug.LogError("El componente DefaultRecommendedBox no tiene un componente TextMeshProUGUI.");
                }
            }
            else
            {
                Debug.LogError("No se encontró el GameObject DefaultRecommendedBox dentro de DisplayARInformationCanvas.");
            }

            // Buscar el botón "DeleteButton" dentro de DisplayARInformationCanvas
            Transform deleteButtonTransform = displayARInformationCanvas.transform.Find("DeleteButton");
            if (deleteButtonTransform != null)
            {
                deleteButton = deleteButtonTransform.GetComponent<Button>();
                if (deleteButton == null)
                {
                    Debug.LogError("El componente DeleteButton no tiene un componente Button.");
                }
                else
                {
                    deleteButton.onClick.AddListener(() => aRButtonController.DeleteARInformation(aRLocationInformationObj.Id));
                }
            }
            else
            {
                Debug.LogError("No se encontró el GameObject DeleteButton dentro de DisplayARInformationCanvas.");
            }

            // Buscar el botón "EditButton" dentro de DisplayARInformationCanvas
            Transform editButtonTransform = displayARInformationCanvas.transform.Find("EditButton");
            if (editButtonTransform != null)
            {
                editButton = editButtonTransform.GetComponent<Button>();
                if (editButton == null)
                {
                    Debug.LogError("El componente EditButton no tiene un componente Button.");
                }
                else
                {
                    editButton.onClick.AddListener(() => aRButtonController.MoveToEdit(aRLocationInformationObj.Information, aRLocationInformationObj.Id));
                }
            }
            else
            {
                Debug.LogError("No se encontró el GameObject EditButton dentro de DisplayARInformationCanvas.");
            }
        }
    }

    private void OnMouseDown()
    {
        if (displayARInformationCanvas != null)
        {
            if (defaultRecommendedBoxText != null)
            {
                // Establecer el texto del DefaultRecommendedBox con la información del objeto AR
                defaultRecommendedBoxText.text = aRLocationInformationObj.Information;
            }
            displayARInformationCanvas.SetActive(true);
        }
        else
        {
            Debug.LogError("displayARInformationCanvas no está asignado.");
        }
    }
}
