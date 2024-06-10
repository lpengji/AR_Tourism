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
}
