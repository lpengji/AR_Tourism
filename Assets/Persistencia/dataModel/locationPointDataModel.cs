using UnityEngine;

[System.Serializable]
public class LocationPoint
{
    public int id;
    public float latitud;
    public float longitud;
    public float altitud;
    public int createdByUserID;
    public int? informationId; // Usa int? para permitir valores nulos

    // Constructor opcional para inicializar los valores
    public LocationPoint(int id, float latitud, float longitud, float altitud, int createdByUserID, int? informationId)
    {
        this.id = id;
        this.latitud = latitud;
        this.longitud = longitud;
        this.altitud = altitud;
        this.createdByUserID = createdByUserID;
        this.informationId = informationId;
    }

    public string ConcatenarLatitudLongitud()
    {
        string resultado = string.Format("{0}, {1}", this.latitud, this.longitud);
        return resultado;
    }

}
