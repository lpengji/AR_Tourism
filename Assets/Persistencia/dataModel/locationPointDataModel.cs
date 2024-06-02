using UnityEngine;

[System.Serializable]
public class LocationPoint
{
    public int id;
    public float latitud;
    public float longitud;
    public int createdByUserID;
    public int informationId;
    public int[] arInformationId;
    public bool isCreated;

    // Constructor opcional para inicializar los valores
    public LocationPoint(int id, float latitud, float longitud, int createdByUserID, int informationId, int[] arInformationId, bool isCreated)
    {
        this.id = id;
        this.latitud = latitud;
        this.longitud = longitud;
        this.createdByUserID = createdByUserID;
        this.informationId = informationId;
        this.arInformationId = arInformationId;
        this.isCreated = isCreated;
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

    public int[] ArInformationId
    {
        get { return arInformationId; }
        set { arInformationId = value; }
    }

    public bool IsCreated
    {
        get { return isCreated; }
        set { isCreated = value; }
    }

    public string ConcatenarLatitudLongitud()
    {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1}", this.latitud, this.longitud);
    }

}
