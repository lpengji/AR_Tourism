using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonController : MonoBehaviour
{
    private string previousScene;
    [SerializeField]
    private GameObject editSceneObject;
    [SerializeField]
    private GameObject menuSceneObject;
    [SerializeField]
    private TMP_InputField editInformationField;
    [SerializeField]
    private TMP_Dropdown editRatingField;
    [SerializeField]
    private GameObject editRatingSprite;
    private Comment comment;
    private Information information;
    private bool newComment;
    private User loggedUser;
    [SerializeField]
    private GetInformationFromDDBB getInformationFromDDBB;
    [SerializeField]
    private GameObject DeleteWarning;
    [SerializeField]
    private Button AceptDeleteButton;
    private TextMeshProUGUI warningText;

    [SerializeField]
    private UserAuthentication userAuthentication;
    // [SerializeField]
    // private ARInforDDBBManagement arInforDDBBManagement;


    // Start is called before the first frame update
    void Start()
    {
        warningText = DeleteWarning.transform.Find("Background/TextBackground/WarningText").GetComponent<TextMeshProUGUI>();
        previousScene = "Map_Scene";
        Debug.Log(warningText.text);
    }
    public void BackToMapScene()
    {
        PlayerPrefs.DeleteKey("locationInfo");
        SceneManager.LoadScene(previousScene);
    }

    public void MoveToEditField(Comment comment)
    {
        this.editRatingField.gameObject.SetActive(true);
        this.comment = comment;
        this.newComment = false;
        // Establece el contenido del campo de entrada de texto
        editInformationField.text = comment.contenidoComment;
        editRatingField.value = comment.rating - 1;

        editSceneObject.SetActive(true);
        menuSceneObject.SetActive(false);
    }

    public void MoveToEditField(Information information)
    {
        editRatingField.gameObject.SetActive(false);
        this.information = information;
        // Establece el contenido del campo de entrada de texto
        editInformationField.text = information.defaultInfo;

        editRatingSprite.SetActive(false);
        editSceneObject.SetActive(true);
        menuSceneObject.SetActive(false);
    }

    public void MoveToEditField(bool newComment, User currentUser)
    {
        this.editRatingField.gameObject.SetActive(true);
        this.newComment = newComment;
        this.loggedUser = currentUser;

        editSceneObject.SetActive(true);
        menuSceneObject.SetActive(false);
    }
    public void AceptEdit()
    {
        // Añadir nuevo comentario
        if (newComment && !string.IsNullOrEmpty(editInformationField.text))
        {
            Debug.Log("añadiendo comentario");
            getInformationFromDDBB.AddNewComment(editInformationField.text, editRatingField.value + 1, loggedUser.userID, loggedUser.userName);
            ResetState();
        }

        // Editar comentario existente
        if (comment != null && !string.IsNullOrEmpty(editInformationField.text))
        {
            Debug.Log("#editando comentario: ");
            this.comment.contenidoComment = editInformationField.text;
            this.comment.rating = editRatingField.value + 1;
            getInformationFromDDBB.editComment(this.comment);
            ResetState();
        }

        // Editar información existente
        if (information != null && !string.IsNullOrEmpty(editInformationField.text))
        {
            Debug.Log("#editando informacion: ");
            this.information.defaultInfo = editInformationField.text;
            getInformationFromDDBB.editInformation(this.information);
            ResetState();
        }

        CleanInputFields();
        editRatingSprite.SetActive(true);
        editSceneObject.SetActive(false);
        menuSceneObject.SetActive(true);
    }

    private void ResetState()
    {
        // Resetear todas las variables relevantes
        comment = null;
        information = null;
        newComment = false;

        CleanInputFields();
        editRatingSprite.SetActive(true);
        editSceneObject.SetActive(false);
        menuSceneObject.SetActive(true);
    }


    public void CancelEdit()
    {
        CleanInputFields();
        if (comment != null)
        {
            comment = null;
        }
        if (information != null)
        {
            information = null;
        }
        if (newComment)
        {
            newComment = false;
        }
        editRatingSprite.SetActive(true);
        editSceneObject.SetActive(false);
        menuSceneObject.SetActive(true);
    }

    private void CleanInputFields()
    {
        editInformationField.text = "";
        editRatingField.value = 0;
    }

    public void DeleteCommenet(int commentId)
    {
        this.warningText.text = "Estás seguro de quere eliminar el comentario seleccionado ?";
        this.DeleteWarning.SetActive(true);
        AceptDeleteButton.onClick.RemoveAllListeners();
        AceptDeleteButton.onClick.AddListener(() => AceptDeleteCommenet(commentId));
    }
    public void AceptDeleteCommenet(int commentId)
    {
        getInformationFromDDBB.DeleteComment(commentId);
        this.DeleteWarning.SetActive(false);
    }

    public void DeleteInformation()
    {
        this.warningText.text = "Estás seguro de quere eliminar la información seleccionada ?";
        this.DeleteWarning.SetActive(true);
        AceptDeleteButton.onClick.RemoveAllListeners();
        AceptDeleteButton.onClick.AddListener(() => AceptDeleteInformation());
    }

    public void AceptDeleteInformation()
    {
        getInformationFromDDBB.DeleteInformation();
        this.DeleteWarning.SetActive(false);
    }

    public void CancelDelete()
    {
        this.DeleteWarning.SetActive(false);
    }

    public void LikeUnlikeTextManager(int InfoDefaultInfo, User user, Button likeUnlikeButton)
    {
        // Verificar si la información ya está en la lista de favoritos del usuario
        if (user.likedLocations.Contains(InfoDefaultInfo))
        {
            // Si está en la lista, removerla
            user.likedLocations.Remove(InfoDefaultInfo);
            likeUnlikeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Añadir a Favoritos";
            likeUnlikeButton.image.color = Color.white;
        }
        else
        {
            // Si no está en la lista, añadirla
            user.likedLocations.Add(InfoDefaultInfo);
            likeUnlikeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Quitar de Favoritos";
            likeUnlikeButton.image.color = Color.red;
        }
        userAuthentication.UpdateUser(user);
        string userJson = JsonUtility.ToJson(user);
        PlayerPrefs.SetString("AuthenticatedUser", userJson);
        PlayerPrefs.Save();
    }

    public void NavigateToARScene()
    {
        string idsString = PlayerPrefs.GetString("arInformationIds", "");
        SceneManager.LoadScene("AR_Scene");
        // arInforDDBBManagement.LoadARLocationInformations(idsString);


    }

}
