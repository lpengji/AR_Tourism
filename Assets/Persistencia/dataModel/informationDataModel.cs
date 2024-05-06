using System.Collections.Generic;

[System.Serializable]
public class Information
{
    public int id;
    public string imageURL;
    public string defaultInfo;
    public List<Comment> comments;

    // Constructor
    public Information(int id, string imageURL, string defaultInfo, List<Comment> comments)
    {
        this.id = id;
        this.imageURL = imageURL;
        this.defaultInfo = defaultInfo;
        this.comments = comments;
    }
    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public string ImageURL
    {
        get { return imageURL; }
        set { imageURL = value; }
    }

    public string DefaultInfo
    {
        get { return defaultInfo; }
        set { defaultInfo = value; }
    }
}