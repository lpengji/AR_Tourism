using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RecommendedListInicializator : MonoBehaviour
{
    [SerializeField]
    private GameObject recommendedLocationInformationPrefab;
    [SerializeField]
    private VerticalLayoutGroup recommendedLocationInformationContentBox;
    [SerializeField]
    private RecommendedListDataManage recommendedListDataManage;
    private List<RecommendedLocationList> allRecommendedLists;
    [SerializeField]
    private Button showRecommendedLocationsButton;
    // Start is called before the first frame update
    void Start()
    {
        recommendedListDataManage.LoadData();
        StartCoroutine(WaitForInformationLoad());
    }

    IEnumerator WaitForInformationLoad()
    {
        // Esperar hasta que la información se haya cargado completamente
        while (!recommendedListDataManage.dataLoaded)
        {
            yield return null;
        }

        // Una vez que la información se ha cargado completamente, generar los TextMeshPro
        GenerateTextMeshPro();
    }

    void GenerateTextMeshPro()
    {
        this.allRecommendedLists = recommendedListDataManage.allRecommendedLists;
        foreach (var recommendedLocation in allRecommendedLists)
        {
            Debug.Log("!!!recommendedLocation.listName" + recommendedLocation.listName);
        }
        if (allRecommendedLists != null && allRecommendedLists.Count > 0)
        {
            // Limpiar el contenido existente
            foreach (Transform child in recommendedLocationInformationContentBox.transform)
            {
                Destroy(child.gameObject);
            }

            // Generar campos de texto para cada elemento en la lista recomendada
            foreach (var recommendedLocation in allRecommendedLists)
            {
                GenerateRecommendedTextField(recommendedLocation);
            }
        }
        else
        {
            Debug.LogWarning("No hay listas de ubicaciones recomendadas para mostrar.");
        }
    }

    private void GenerateRecommendedTextField(RecommendedLocationList recommendedLocation)
    {
        GameObject commentObject = CreateTextMeshPro(recommendedLocation, recommendedLocationInformationContentBox.transform);

        // Asignar el ID del comentario al objeto creado
        commentObject.name = recommendedLocation.ListID.ToString();
        commentObject.SetActive(true);
    }

    private GameObject CreateTextMeshPro(RecommendedLocationList recommendedLocation, Transform parent)
    {
        // Instanciar el prefab
        GameObject textObject = Instantiate(recommendedLocationInformationPrefab.gameObject, parent);

        // Obtener el componente TextMeshProUGUI del objeto instanciado
        TextMeshProUGUI textMeshPro = textObject.GetComponentInChildren<TextMeshProUGUI>();

        // Verificar si la instancia fue exitosa
        if (textMeshPro == null)
        {
            Debug.LogError("Error al obtener el componente TextMeshProUGUI en el objeto duplicado.");
            return null; // Devolver null si la instancia falló
        }

        // Establecer el texto
        textMeshPro.text = "ID: " + recommendedLocation.ListID + "\n"
                         + "Nombre: " + recommendedLocation.ListName;
        Debug.Log("recommendedLocation.ListID" + recommendedLocation.ListID);

        return textObject;
    }
}
