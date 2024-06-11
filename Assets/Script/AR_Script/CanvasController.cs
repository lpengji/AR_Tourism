using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public GameObject displayARInformationCanvas;
    public TextMeshProUGUI defaultRecommendedBoxText;
    public Button deleteButton;
    public Button editButton;
    public ARButtonController aRButtonController;
    private User loggedUser;
    private int currentLocationPointId;

    void Awake()
    {
        if (displayARInformationCanvas == null)
        {
            Debug.LogError("DisplayARInformationCanvas no está asignado en el CanvasController.");
        }
        if (defaultRecommendedBoxText == null)
        {
            Debug.LogError("defaultRecommendedBoxText no está asignado en el CanvasController.");
        }
        if (deleteButton == null)
        {
            Debug.LogError("deleteButton no está asignado en el CanvasController.");
        }
        if (editButton == null)
        {
            Debug.LogError("editButton no está asignado en el CanvasController.");
        }
        if (aRButtonController == null)
        {
            Debug.LogError("aRButtonController no está asignado en el CanvasController.");
        }
    }

    void Start()
    {
        string userJson = PlayerPrefs.GetString("AuthenticatedUser");
        loggedUser = JsonUtility.FromJson<User>(userJson);
        currentLocationPointId = PlayerPrefs.GetInt("locationInfo");

        if (loggedUser.rol == "admin" || loggedUser.createdLocations.Contains(currentLocationPointId))
        {
            deleteButton.gameObject.SetActive(true);
            editButton.gameObject.SetActive(true);
        }
        else
        {
            deleteButton.gameObject.SetActive(false);
            editButton.gameObject.SetActive(false);
        }
    }
}
