using System.ComponentModel.DataAnnotations.Schema;

namespace AccountServer.DB;

[Table("User")]
public class User
{
    public int UserId { get; set; }
    public string UserAccount { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; }
    public UserRole Role { get; set; }
    public UserState State { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserLevel { get; set; }
    public int Exp { get; set; }
    public int RankPoint { get; set; }
    public int Gold { get; set; }
    public int Gem { get; set; }
}

[Table("RefreshToken")]
public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
}

[Table("ExpTable")]
public class ExpTable
{
    public int Level { get; set; }
    public int Exp { get; set; }
}

[Table("Unit")]
public class Unit
{
    public UnitId UnitId { get; set; }
    public UnitClass Class { get; set; }
    public int Level { get; set; }
    public UnitId Species { get; set; }
    public UnitRole Role { get; set; }
    public Camp Camp { get; set; }
}

[Table("Deck")]
public class Deck
{
    public int DeckId { get; set; }
    public int UserId { get; set; }
    public Camp Camp { get; set; }
    public int DeckNumber { get; set; }
    public bool LastPicked { get; set; }
}

[Table("Deck_Unit")]
public class DeckUnit
{
    public int DeckId { get; set; }
    public UnitId UnitId { get; set; }
}

[Table("User_Unit")]
public class UserUnit
{
    public int UserId { get; set; }
    public UnitId UnitId { get; set; }
    public int Count { get; set; }
}