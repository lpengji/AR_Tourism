using System.Collections.Generic;

public class User
{
    public int UserID { get; set; }
    public string UserName { get; set; }
    public string UserPassword { get; set; }
    public string Rol { get; set; }
    public List<int> LikedLocations { get; set; }
}
