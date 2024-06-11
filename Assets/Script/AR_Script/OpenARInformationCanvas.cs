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

        // Buscar el CanvasController en la escena
        CanvasController canvasController = FindObjectOfType<CanvasController>();
        if (canvasController != null)
        {
            displayARInformationCanvas = canvasController.displayARInformationCanvas;
            defaultRecommendedBoxText = canvasController.defaultRecommendedBoxText;
            deleteButton = canvasController.deleteButton;
            editButton = canvasController.editButton;
            aRButtonController = canvasController.aRButtonController;
        }

        if (displayARInformationCanvas == null)
        {
            Debug.LogError("DisplayARInformationCanvas no encontrado en la escena. Por favor asegúrate de que existe.");
            return;
        }

        // Proceder con la inicialización
        InitializeCanvas();
    }

    private void InitializeCanvas()
    {
        if (defaultRecommendedBoxText == null)
        {
            Debug.LogError("El componente DefaultRecommendedBox no tiene un componente TextMeshProUGUI.");
        }

        if (deleteButton != null)
        {
            // Asegurarse de que no haya múltiples suscripciones
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(() =>
            {
                Debug.Log($"Intentando eliminar: {aRLocationInformationObj.Id}");
                aRButtonController.DeleteARInformation(aRLocationInformationObj.Id);
            });
        }
        else
        {
            Debug.LogError("El componente DeleteButton no tiene un componente Button.");
        }

        if (editButton != null)
        {
            // Asegurarse de que no haya múltiples suscripciones
            editButton.onClick.RemoveAllListeners();
            editButton.onClick.AddListener(() =>
            {
                Debug.Log($"Intentando editar: {aRLocationInformationObj.Id} con información: {aRLocationInformationObj.Information}");
                aRButtonController.MoveToEdit(aRLocationInformationObj.Information, aRLocationInformationObj.Id);
            });
        }
        else
        {
            Debug.LogError("El componente EditButton no tiene un componente Button.");
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
