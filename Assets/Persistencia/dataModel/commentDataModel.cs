using System.Collections.Generic;
public class Comment
{
    public int Id { get; set; }
    public string ContenidoComment { get; set; }
    public int Rating { get; set; }
    public User CreatedBy { get; set; }
}