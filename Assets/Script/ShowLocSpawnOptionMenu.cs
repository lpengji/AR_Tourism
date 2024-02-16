using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLocSpawnOptionMenu : MonoBehaviour
{
    private LocationPointInformation locationInfo;
    private void Start() {
        locationInfo = GetComponent<LocationPointInformation>();
    }

    private void OnMouseDown() {
        Debug.Log("Distancia restante: "+locationInfo.getDistanceBetweenPlayerAndLocation().ToString("0.00")+" m");
    }
}
