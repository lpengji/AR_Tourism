using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDistanceBtwnUserAndLocationPoint : MonoBehaviour
{
    private LocationPointInformation locationInfo;
    private void Start() {
        locationInfo = GetComponent<LocationPointInformation>();
    }

    // private void OnMouseDown() {
    //     Debug.Log("Distancia restante: "+locationInfo.getDistanceBetweenPlayerAndLocation().ToString("0.00")+" m");
    // }

    private void OnMouseDown() {
    Debug.Log("DATOS LOCATION POINT: " +
        "ID: " + locationInfo.Id + 
        ", Latitud-Longitud: " + locationInfo.ActualCoordinate + 
        ", Altitud: " + locationInfo.Altitud + 
        ", Creado por Usuario ID: " + locationInfo.CreatedByUserID + 
        ", ID de Información: " + locationInfo.InformationId);
    }
}
