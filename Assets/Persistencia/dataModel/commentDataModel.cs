using System.Collections.Generic;

[System.Serializable]
public class Comment
{
    public int id;
    public string contenidoComment;
    public int rating;
    public int createdByUserID;
    public string userName;

    // Constructor
    public Comment(int id, string contenidoComment, int rating, int createdByUserID, string userName)
    {
        this.id = id;
        this.contenidoComment = contenidoComment;
        this.rating = rating;
        this.createdByUserID = createdByUserID;
        this.userName = userName;
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

    public string Username
    {
        get { return userName; }
        set { userName = value; }
    }
}
