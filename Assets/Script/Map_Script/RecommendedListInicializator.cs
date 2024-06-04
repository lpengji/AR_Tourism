using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private MapButtonController mapButtonController;

    // Start is called before the first frame update
    void Start()
    {
        recommendedListDataManage.LoadData();
        StartCoroutine(WaitForInformationLoad());
    }

    IEnumerator WaitForInformationLoad()
    {
        // Wait until the data has been completely loaded
        while (!recommendedListDataManage.dataLoaded)
        {
            yield return null;
        }

        // Once the data is fully loaded, generate the TextMeshPro
        GenerateTextMeshPro();
    }

    void GenerateTextMeshPro()
    {
        this.allRecommendedLists = recommendedListDataManage.allRecommendedLists;

        if (allRecommendedLists != null && allRecommendedLists.Count > 0)
        {
            // Clear existing content
            foreach (Transform child in recommendedLocationInformationContentBox.transform)
            {
                Destroy(child.gameObject);
            }

            // Generate text fields for each item in the recommended list
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
        // Create and set up the TextMeshPro object
        GameObject commentObject = CreateTextMeshPro(recommendedLocation, recommendedLocationInformationContentBox.transform);

        // Check if the commentObject was successfully created
        if (commentObject != null)
        {
            // Assign the ID to the name of the commentObject
            commentObject.name = recommendedLocation.ListID.ToString();

            // Ensure the commentObject is active
            commentObject.SetActive(true);

            // Generate the follow button functionality
            GenerateFollowButton(commentObject, recommendedLocation);
        }
    }

    private void GenerateFollowButton(GameObject commentObject, RecommendedLocationList recommendedLocation)
    {
        Button followButton = commentObject.GetComponentInChildren<Button>();
        followButton.onClick.AddListener(() => mapButtonController.displaySelectedRuote(recommendedLocation.locations));
    }

    private GameObject CreateTextMeshPro(RecommendedLocationList recommendedLocation, Transform parent)
    {
        // Instantiate the prefab
        GameObject textObject = Instantiate(recommendedLocationInformationPrefab, parent);

        // Obtain the TextMeshProUGUI component
        TextMeshProUGUI textMeshPro = textObject.GetComponentInChildren<TextMeshProUGUI>();

        // Check if the TextMeshProUGUI component was found
        if (textMeshPro == null)
        {
            Debug.LogError("Error al obtener el componente TextMeshProUGUI en el objeto duplicado.");
            return null; // Return null if the component wasn't found
        }

        // Set the text
        textMeshPro.text = "ID: " + recommendedLocation.ListID + "\n" + recommendedLocation.ListName;

        // Ensure the RectTransform is correctly set
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition3D = Vector3.zero; // Reset position
        rectTransform.localScale = Vector3.one; // Reset scale

        return textObject;
    }
}
