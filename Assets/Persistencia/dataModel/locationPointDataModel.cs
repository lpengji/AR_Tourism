using UnityEngine;

[System.Serializable]
public class LocationPoint
{
    public int id;
    public float latitud;
    public float longitud;
    public float altitud;
    public int createdByUserID;
    public int informationId; // Usa int? para permitir valores nulos

    // Constructor opcional para inicializar los valores
    public LocationPoint(int id, float latitud, float longitud, float altitud, int createdByUserID, int informationId)
    {
        this.id = id;
        this.latitud = latitud;
        this.longitud = longitud;
        this.altitud = altitud;
        this.createdByUserID = createdByUserID;
        this.informationId = informationId;
    }

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public float Latitud
    {
        get { return latitud; }
        set { latitud = value; }
    }

    public float Longitud
    {
        get { return longitud; }
        set { longitud = value; }
    }

    public float Altitud
    {
        get { return altitud; }
        set { altitud = value; }
    }

    public int CreatedByUserID
    {
        get { return createdByUserID; }
        set { createdByUserID = value; }
    }

    public int InformationId
    {
        get { return informationId; }
        set { informationId = value; }
    }

    public string ConcatenarLatitudLongitud()
    {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1}", this.latitud, this.longitud);
    }

}
