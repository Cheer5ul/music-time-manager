namespace music_time_manager.Persistence.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string UserName  { get; set; }
    public string PasswordHash {get; set;}
}