using API.Entities.Common;

namespace API.Entities;

public class AppUser : Entity
{
    public string UserName { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
}