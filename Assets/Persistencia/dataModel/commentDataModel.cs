using System.Collections.Generic;

[System.Serializable]
public class Comment
{
    public int id;
    public string contenidoComment;
    public int rating;
    public int createdByUserID;

    // Constructor
    public Comment(int id, string contenidoComment, int rating, int createdByUserID)
    {
        this.id = id;
        this.contenidoComment = contenidoComment;
        this.rating = rating;
        this.createdByUserID = createdByUserID;
    }

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public string ContenidoComment
    {
        get { return contenidoComment; }
        set { contenidoComment = value; }
    }

    public int Rating
    {
        get { return rating; }
        set { rating = value; }
    }

    public int CreatedByUserID
    {
        get { return createdByUserID; }
        set { createdByUserID = value; }
    }
}
