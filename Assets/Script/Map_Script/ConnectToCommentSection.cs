using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectToCommentSection : MonoBehaviour
{
    private LocationPointInformation locationInfo;
    private string nextSceneName = "Menu_Scene";
    private void Start()
    {
        locationInfo = GetComponent<LocationPointInformation>();
    }

    // private void OnMouseDown() {
    //     Debug.Log("Distancia restante: "+locationInfo.getDistanceBetweenPlayerAndLocation().ToString("0.00")+" m");
    // }

    private void OnMouseDown()
    {
        // pasar el id del tablon de informacion a la siguiente escena y cargar la escena nueva
        PlayerPrefs.SetInt("locationInfo", locationInfo.InformationId);
        // Guardar los arInformationIds en PlayerPrefs como una cadena separada por comas
        PlayerPrefs.SetString("arInformationIds", string.Join(",", locationInfo.ArInformationId));

        SceneManager.LoadScene(nextSceneName);

        Debug.Log("DATOS LOCATION POINT: " +
            "ID: " + locationInfo.Id +
            ", Latitud-Longitud: " + locationInfo.ActualCoordinate +
            ", Altitud: " + locationInfo.Altitud +
            ", Creado por Usuario ID: " + locationInfo.CreatedByUserID +
            ", ID de Información: " + locationInfo.InformationId +
            ", AR locations ids: " + string.Join(",", locationInfo.ArInformationId));
    }
}
