using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InformationLoading : MonoBehaviour
{
    public MenuButtonController menuButtonController;
    [SerializeField]
    private GetInformationFromDDBB getInformationFromDDBB;

    [SerializeField]
    private VerticalLayoutGroup informationContentBox;

    [SerializeField]
    private VerticalLayoutGroup commentContentBox;

    [SerializeField]
    private Graphic textObjetcTemplate;
    [SerializeField]
    private TextMeshProUGUI ratingBox;
    private User loggedUser;

    [SerializeField]
    private GameObject buttonPrefab;


    // Start is called before the first frame update
    void Start()
    {
        // Obtener la información del usuario almacenada en PlayerPrefs
        string userJson = PlayerPrefs.GetString("AuthenticatedUser");
        this.loggedUser = JsonUtility.FromJson<User>(userJson);

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

        if (information != null) // Comprobar si existe información
        {
            GenerateInformationField(information);

            if (information.comments != null && information.comments.Count > 0)
            {
                SetAverageRating(information);
                GenerateCommentField(information);
            }
        }
        else
        {
            Debug.LogError("No se ha cargado ninguna información desde la base de datos.");
        }
    }


    void SetAverageRating(Information information)
    {

        float totalRating = 0f;
        int commentCount = 0;

        // Calcular la suma de todos los ratings y contar el número de comentarios
        foreach (var comment in information.comments)
        {
            totalRating += comment.rating;
            commentCount++;
        }

        // Calcular el rating medio
        float averageRating = commentCount > 0 ? totalRating / commentCount : 0f;

        // Asignar el rating medio al ratingBox
        if (ratingBox != null)
        {
            ratingBox.text = "Valoración media: " + averageRating.ToString("F2") + " / 5";
        }
        else
        {
            Debug.LogError("El ratingBox no está asignado en el Inspector.");
        }
    }

    public void GenerateCommentField(Information information)
    {
        foreach (Transform child in commentContentBox.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var comment in information.comments)
        {
            if (!string.IsNullOrEmpty(comment.contenidoComment))
            {
                // Crear el texto del comentario con el nombre de usuario al principio y un salto de línea
                string commentText = "";

                // Si hay información del usuario, agregarla al texto del comentario
                if (!string.IsNullOrEmpty(comment.userName))
                {
                    // Agregar el nombre de usuario en negrita y subrayado
                    commentText += "usuario: <b><u>" + comment.userName + "</u></b>\n";
                    commentText += "valoración: <b>" + comment.rating + "</b>\n\n";
                }

                commentText += comment.contenidoComment;

                // Crear el objeto de comentario
                GameObject commentObject = CreateTextMeshPro(commentText, commentContentBox.transform);

                // Asignar el ID del comentario al objeto creado
                commentObject.name = comment.id.ToString();

                if (comment.id == loggedUser.userID)
                {
                    GenerateEditDeleteButton(commentObject, comment);
                }

            }
        }
    }

    void GenerateEditDeleteButton(GameObject commentObject, Comment comment)
    {
        // Instanciar el prefab del botón
        GameObject buttonPrefabInstance = Instantiate(buttonPrefab, commentObject.transform);

        // Activar el botón
        buttonPrefabInstance.SetActive(true);

        // Obtener los botones hijos del botón instanciado
        Button[] buttons = buttonPrefabInstance.GetComponentsInChildren<Button>();

        // Iterar sobre los botones y asignarles sus métodos respectivos
        foreach (Button button in buttons)
        {
            if (button.name == "editButton") // Nombre del botón de editar
            {
                // Agregar el listener de clic para editar
                button.onClick.AddListener(() => menuButtonController.MoveToEditField(comment));
            }
        }
    }



    void GenerateInformationField(Information information)
    {
        // Limpiar cualquier objeto hijo existente en informationContentBox solo si hay información
        if (!string.IsNullOrEmpty(information.defaultInfo))
        {
            foreach (Transform child in informationContentBox.transform)
            {
                Destroy(child.gameObject);
            }

            GameObject infoObject = CreateTextMeshPro(information.defaultInfo, informationContentBox.transform);

            // Asignar el ID de la información al objeto creado
            infoObject.name = information.id.ToString();
        }
        else
        {
            CreateTextMeshPro("Sin información disponible", informationContentBox.transform);
        }
    }

    GameObject CreateTextMeshPro(string text, Transform parent)
    {
        // Duplicar el GameObject original que contiene el componente TextMeshProUGUI
        GameObject textObject = Instantiate(textObjetcTemplate.gameObject, parent);

        // Obtener el componente TextMeshProUGUI del objeto duplicado
        TextMeshProUGUI textMeshPro = textObject.GetComponentInChildren<TextMeshProUGUI>();

        // Verificar si la duplicación fue exitosa
        if (textMeshPro == null)
        {
            Debug.LogError("Error al obtener el componente TextMeshProUGUI en el objeto duplicado.");
            return null; // Devolver null si la duplicación falló
        }

        // Establecer el texto
        textMeshPro.text = text;

        return textObject; // Devolver el GameObject creado
    }
}
