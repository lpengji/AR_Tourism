using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // Start is called before the first frame update
    void Start()
    {
        previousScene = "Map_Scene";
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
        // Establece el contenido del campo de entrada de texto
        editInformationField.text = comment.contenidoComment;
        editRatingField.value = comment.rating - 1;

        editSceneObject.SetActive(true);
        menuSceneObject.SetActive(false);
    }

    public void MoveToEditField(Information information)
    {
        this.editRatingField.gameObject.SetActive(false);
        this.information = information;
        // Establece el contenido del campo de entrada de texto
        editInformationField.text = information.defaultInfo;

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
        //editando comentario (contenido + rating)
        if (comment != null && editInformationField.text != "")
        {
            this.comment.contenidoComment = editInformationField.text;
            this.comment.rating = editRatingField.value + 1;
            getInformationFromDDBB.editComment(this.comment);
        }
        // editando informacion 
        if (information != null && editInformationField.text != "")
        {
            this.information.defaultInfo = editInformationField.text; ;
            getInformationFromDDBB.editInformation(this.information);
        }
        // alta nuevo locationInformation
        if (newComment && editInformationField.text != "")
        {
            getInformationFromDDBB.AddNewComment(editInformationField.text, editRatingField.value + 1, loggedUser.userID, loggedUser.userName);
        }
        CleanInputFields();
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
        Debug.Log("delete comment:" + commentId);
        getInformationFromDDBB.DeleteComment(commentId);
    }

    public void DeleteInformation()
    {
        Debug.Log("delete information:");
        getInformationFromDDBB.DeleteInformation();
    }

}
