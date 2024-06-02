using System.Collections.Generic;

[System.Serializable]
public class ARLocationInformation
{
    public int id;
    public double latitud;
    public double longitud;
    public double altitud;
    public string information;

    // Constructor
    public ARLocationInformation(int id, double latitud, double longitud, double altitud, string information)
    {
        this.id = id;
        this.latitud = latitud;
        this.longitud = longitud;
        this.altitud = altitud;
        this.information = information;
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

    public string Information
    {
        get { return information; }
        set { information = value; }
    }
}