using System.Collections.Generic;

[System.Serializable]
public class User
{
    public int id;
    public string userName;
    public string userPassword;
    public string rol;
    public List<int> likedLocations;
    public List<int> createdLocations;

    // Constructor
    public User(int id, string userName, string userPassword, string rol, List<int> likedLocations, List<int> createdLocations)
    {
        this.id = id;
        this.userName = userName;
        this.userPassword = userPassword;
        this.rol = rol;
        this.likedLocations = likedLocations;
        this.createdLocations = createdLocations;
    }

    public int Id
    {
        get { return id; }
        set { id = value; }
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
