using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InformationLoading : MonoBehaviour
{
    [SerializeField]
    private GetInformationFromDDBB getInformationFromDDBB;

    [SerializeField]
    private VerticalLayoutGroup informationContentBox;

    [SerializeField]
    private VerticalLayoutGroup commentContentBox;

    [SerializeField]
    private TextMeshProUGUI textMeshProTemplate;


    // Start is called before the first frame update
    void Start()
    {
        // Cargar la información desde la base de datos
        getInformationFromDDBB.LoadInformation();

        // Esperar un pequeño tiempo para asegurarse de que la información se haya cargado completamente
        StartCoroutine(WaitForInformationLoad());
    }

    IEnumerator WaitForInformationLoad()
    {
        // Esperar hasta que la información se haya cargado completamente
        while (!getInformationFromDDBB.informationLoaded)
        {
            yield return null;
        }

        // Una vez que la información se ha cargado completamente, generar los TextMeshPro
        GenerateTextMeshPro();
    }

    void GenerateTextMeshPro()
    {
        Information information = getInformationFromDDBB.information;

        GenerateCommentField(information);
        GenerateInformationField(information);
    }

    void GenerateCommentField(Information information)
    {
        // Limpiar cualquier objeto hijo existente en commentContentBox
        foreach (Transform child in commentContentBox.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var comment in information.comments)
        {
            if (!string.IsNullOrEmpty(comment.contenidoComment))
            {
                GameObject commentObject = CreateTextMeshPro(comment.contenidoComment, commentContentBox.transform);

                // Obtener el componente TextMeshProUGUI del objeto creado
                TextMeshProUGUI textMeshPro = commentObject.GetComponent<TextMeshProUGUI>();

                // Asignar el ID del comentario al objeto creado
                textMeshPro.gameObject.name = comment.id.ToString();
            }
        }
    }

    void GenerateInformationField(Information information)
    {
        // Limpiar cualquier objeto hijo existente en informationContentBox
        foreach (Transform child in informationContentBox.transform)
        {
            Destroy(child.gameObject);
        }

        if (!string.IsNullOrEmpty(information.defaultInfo))
        {
            GameObject infoObject = CreateTextMeshPro(information.defaultInfo, informationContentBox.transform);

            // Obtener el componente TextMeshProUGUI del objeto creado
            TextMeshProUGUI textMeshPro = infoObject.GetComponent<TextMeshProUGUI>();

            // Asignar el ID de la información al objeto creado
            textMeshPro.gameObject.name = information.id.ToString();
        }
        else
        {
            CreateTextMeshPro("Sin información disponible", informationContentBox.transform);
        }
    }


    GameObject CreateTextMeshPro(string text, Transform parent)
    {
        // Duplicar el GameObject original que contiene el componente TextMeshProUGUI
        GameObject textObject = Instantiate(textMeshProTemplate.gameObject, parent);

        // Obtener el componente TextMeshProUGUI del objeto duplicado
        TextMeshProUGUI textMeshPro = textObject.GetComponent<TextMeshProUGUI>();

        // Verificar si la duplicación fue exitosa
        if (textMeshPro == null)
        {
            Debug.LogError("Error al duplicar el componente TextMeshProUGUI.");
            return null; // Devolver null si la duplicación falló
        }

        // Establecer el texto
        textMeshPro.text = text;

        // // Restablecer la posición y rotación del componente duplicado
        // textMeshPro.rectTransform.localPosition = Vector3.zero;
        // textMeshPro.rectTransform.localRotation = Quaternion.identity;

        return textObject; // Devolver el GameObject creado
    }





}
