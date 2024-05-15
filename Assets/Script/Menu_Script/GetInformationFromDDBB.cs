using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.IO;
using System;

public class GetInformationFromDDBB : MonoBehaviour
{
    public Information information;
    public InformationLoading informationLoading;
    private string locationInformationPersistenceFileName = "locationInformationDDBB.json";
    private int informationId;
    private List<Information> allInformationList;
    public bool informationLoaded = false;

    public void LoadInformation()
    {
        // Obtener el ID de la información del PlayerPrefs
        informationId = PlayerPrefs.GetInt("locationInfo", -1);
        Debug.Log(informationId);

        // Cargar y leer el archivo JSON
        string filePath = Path.Combine(Application.streamingAssetsPath, locationInformationPersistenceFileName);

        // Verificar si la plataforma es Android
        if (Application.platform == RuntimePlatform.Android)
        {
            // Si es Android, cargar el archivo usando UnityWebRequest
            UnityWebRequest www = UnityWebRequest.Get(filePath);

            // Esperar a que se complete la descarga
            StartCoroutine(DownloadFile(www));
        }
        else
        {
            // Si no es Android, cargar el archivo directamente desde el sistema de archivos
            LoadFile(filePath);
        }
    }

    IEnumerator DownloadFile(UnityWebRequest www)
    {
        // Esperar a que se complete la descarga
        yield return www.SendWebRequest();

        // Verificar si hubo algún error
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al cargar el archivo: " + www.error);
        }
        else
        {
            // Obtener el contenido del archivo JSON
            string locationInformation = www.downloadHandler.text;

            // Deserializar el JSON a una lista de objetos LocationPoint
            allInformationList = JsonUtility.FromJson<InformationWrapper>(locationInformation).informations;

            // Filtrar las informaciones hasta encontrar el id correcto
            information = allInformationList.Find(info => info.id == informationId);

            informationLoaded = true;
        }
    }

    void LoadFile(string filePath)
    {
        // Cargar el archivo directamente desde el sistema de archivos
        string locationInformation = File.ReadAllText(filePath);

        // Deserializar el JSON a una lista de objetos Information
        allInformationList = JsonUtility.FromJson<InformationWrapper>(locationInformation).informations;

        // Buscar la información con el ID correspondiente
        information = allInformationList.Find(info => info.id == informationId);

        informationLoaded = true;
    }

    public void editComment(Comment editedComment)
    {
        // Verificar si la información se ha cargado correctamente
        if (!informationLoaded)
        {
            Debug.LogError("No se puede editar el comentario porque la información no se ha cargado correctamente.");
            return;
        }

        // Buscar el comentario a editar dentro de la lista de comentarios de la información actual
        Comment commentToEdit = information.comments.Find(comment => comment.id == editedComment.id);

        // Verificar si se encontró el comentario
        if (commentToEdit != null)
        {
            // Actualizar el contenido del comentario
            commentToEdit.contenidoComment = editedComment.contenidoComment;
            UpdateAllInformationList(information);

            // Llamar a GenerateCommentField para regenerar los comentarios
            informationLoading.GenerateCommentField(information);
            informationLoading.SetAverageRating(information);
        }
        else
        {
            Debug.LogError("No se pudo encontrar el comentario a editar.");
        }
    }
    public void editInformation(Information newInformation)
    {
        // Verificar si la información se ha cargado correctamente
        if (!informationLoaded)
        {
            Debug.LogError("No se puede editar el comentario porque la información no se ha cargado correctamente.");
            return;
        }

        this.information = newInformation;

        // Verificar si se encontró el comentario
        if (information != null)
        {
            UpdateAllInformationList(information);

            // Llamar a GenerateCommentField para regenerar los comentarios
            informationLoading.GenerateInformationField(information);
        }
        else
        {
            Debug.LogError("No se pudo encontrar el comentario a editar.");
        }
    }

    public void AddNewComment(string contenidoComment, int rating, int createdByUserID, string userName)
    {
        // Verificar si la información actual es válida
        if (information == null)
        {
            Debug.LogError("No hay información actual para agregar un comentario.");
            return;
        }

        // Crear un nuevo comentario con un ID único
        int newCommentId = GenerateUniqueCommentId();
        Comment newComment = new Comment(newCommentId, contenidoComment, rating, createdByUserID, userName);

        // Agregar el nuevo comentario a la lista de comentarios de la información actual
        information.comments.Add(newComment);

        informationLoading.GenerateCommentField(information);
        informationLoading.SetAverageRating(information);
        UpdateAllInformationList(information);
    }

    private int GenerateUniqueCommentId()
    {
        // Obtener el último ID de comentario existente
        int lastCommentId = 0;
        foreach (Comment comment in information.comments)
        {
            if (comment.Id > lastCommentId)
            {
                lastCommentId = comment.Id;
            }
        }

        // Incrementar el último ID de comentario para obtener un ID único
        return lastCommentId + 1;
    }

    void UpdateAllInformationList(Information updatedInformation)
    {
        // Buscar el índice de la información actualizada dentro de la lista
        int index = allInformationList.FindIndex(info => info.id == updatedInformation.id);
        if (index != -1)
        {
            // Reemplazar la información antigua con la información actualizada
            allInformationList[index] = updatedInformation;

            // Llamar a GenerateCommentField para regenerar los comentarios
            informationLoading.GenerateCommentField(updatedInformation);
            this.SaveInformationToFile();
        }
        else
        {
            Debug.LogError("No se encontró la información actualizada en la lista.");
        }
    }

    void SaveInformationToFile()
    {
        // Convertir la lista de información actualizada a JSON
        string json = JsonUtility.ToJson(new InformationWrapper(allInformationList));

        // Obtener la ruta del archivo JSON
        string filePath;

#if UNITY_ANDROID && !UNITY_EDITOR
        // Si es Android, usar la ruta persistente en el sistema de archivos específica de Android
        filePath = Path.Combine(Application.persistentDataPath, locationInformationPersistenceFileName);
#else
        // Si no es Android, usar la ruta en la carpeta de streamingAssets
        filePath = Path.Combine(Application.streamingAssetsPath, locationInformationPersistenceFileName);
#endif

        // Escribir el JSON en el archivo
        File.WriteAllText(filePath, json);

        Debug.Log("Información actualizada guardada en el archivo JSON.");
    }
}
