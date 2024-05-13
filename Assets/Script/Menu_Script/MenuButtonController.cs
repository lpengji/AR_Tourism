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
    private Comment comment;
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
        this.comment = comment;
        // Establece el contenido del campo de entrada de texto
        editInformationField.text = comment.contenidoComment;

        editSceneObject.SetActive(true);
        menuSceneObject.SetActive(false);
    }

    public void AceptEdit()
    {
        this.comment.contenidoComment = editInformationField.text;
        getInformationFromDDBB.editComment(this.comment);
        editSceneObject.SetActive(false);
        menuSceneObject.SetActive(true);
    }

    public void CancelEdit()
    {
        editInformationField.text = "";
        editSceneObject.SetActive(false);
        menuSceneObject.SetActive(true);
    }

    public void DeleteCommenet(int commentId)
    {
        Debug.Log("delete comment:" + commentId);
    }
}
