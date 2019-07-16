using System.ComponentModel.DataAnnotations;

public class UserInfo
{
    [Key]
    public string UserId { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; } 
    public string HashedPassword { get; set; }
}