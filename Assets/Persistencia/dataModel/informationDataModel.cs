using System.Collections.Generic;

public class Information
{
    public int Id { get; set; }
    public string ImageURL { get; set; }
    public string DefaultInfo { get; set; }
    public List<Comment> Comments { get; set; }
}