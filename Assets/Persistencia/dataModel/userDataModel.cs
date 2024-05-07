using System.Collections.Generic;

[System.Serializable]
public class User
{
    public int userID;
    public string userName;
    public string userPassword;
    public string rol;
    public List<int> likedLocations;
    public List<int> createdLocations;

    // Constructor
    public User(int userID, string userName, string userPassword, string rol, List<int> likedLocations, List<int> createdLocations)
    {
        this.userID = userID;
        this.userName = userName;
        this.userPassword = userPassword;
        this.rol = rol;
        this.likedLocations = likedLocations;
        this.createdLocations = createdLocations;
    }

    public int UserId
    {
        get { return userID; }
        set { userID = value; }
    }

    public string UserName
    {
        get { return userName; }
        set { userName = value; }
    }

    public string UserPassword
    {
        get { return userPassword; }
        set { userPassword = value; }
    }

    public string Rol
    {
        get { return rol; }
        set { rol = value; }
    }

    public List<int> LikedLocations
    {
        get { return likedLocations; }
        set { likedLocations = value; }
    }

    public List<int> CreatedLocations
    {
        get { return createdLocations; }
        set { createdLocations = value; }
    }
}
