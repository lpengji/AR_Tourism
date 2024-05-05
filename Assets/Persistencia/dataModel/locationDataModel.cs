using System.Collections.Generic;

[System.Serializable]
public class Location
{
    public int id;
    public double latitud;
    public double longitud;
    public double altitud;
    public string tipoLocationPoint;
    public List<Comment> comments;

    // Constructor
    public Location(int id, double latitud, double longitud, double altitud, string tipoLocationPoint, List<Comment> comments)
    {
        this.id = id;
        this.latitud = latitud;
        this.longitud = longitud;
        this.altitud = altitud;
        this.tipoLocationPoint = tipoLocationPoint;
        this.comments = comments;
    }

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public double Latitud
    {
        get { return latitud; }
        set { latitud = value; }
    }

    public double Longitud
    {
        get { return longitud; }
        set { longitud = value; }
    }

    public double Altitud
    {
        get { return altitud; }
        set { altitud = value; }
    }

    public string TipoLocationPoint
    {
        get { return tipoLocationPoint; }
        set { tipoLocationPoint = value; }
    }
}
