using System.ComponentModel.DataAnnotations.Schema;

namespace AccountServer.DB;

#region Enum

public enum UserRole
{
    Admin,
    User
}

public enum UserState
{
    Activate,
    Deactivate,
    Suspension,
    Ban
}

public enum UnitClass
{
    None,
    Peasant,
    Squire,
    Knight,
    HighRankingKnight,
    Baron,
    Count,
    Duke,
}

public enum UnitRole
{
    None,
    Warrior,
    Ranger,
    Mage,
    Supporter,
    Tanker
}

public enum Camp
{
    None,
    Sheep,
    Wolf
}

public enum UnitId
{
    UnknownUnit = 0,
    Bunny = 101,
    Rabbit = 102,
    Hare = 103,
    Mushroom = 104,
    Fungi = 105,
    Toadstool = 106,
    Seed = 107,
    Sprout = 108,
    FlowerPot = 109,
    Bud = 110,
    Bloom = 111,
    Blossom = 112,
    PracticeDummy = 113,
    TargetDummy = 114,
    TrainingDummy = 115,
    Shell = 116,
    Spike = 117,
    Hermit = 118,
    SunBlossom = 119, 
    SunflowerFairy = 120,
    SunfloraPixie = 121,
    MothLuna = 122,
    MothMoon = 123,
    MothCelestial = 124,
    Soul = 125,
    Haunt = 126,
    SoulMage = 127,
    DogPup = 501,
    DogBark = 502,
    DogBowwow = 503,
    Burrow = 504,
    MoleRat = 505,
    MoleRatKing = 506,
    MosquitoBug = 507,
    MosquitoPester = 508,
    MosquitoStinger = 509,
    WolfPup = 510,
    Wolf = 511,
    Werewolf = 512,
    Bomb = 513,
    SnowBomb = 514,
    PoisonBomb = 515,
    Cacti = 516,
    Cactus = 517,
    CactusBoss = 518,
    Snakelet = 519,
    Snake = 520,
    SnakeNaga = 521,
    Lurker = 522,
    Creeper = 523,
    Horror = 524,
    Skeleton = 525,
    SkeletonGiant = 526,
    SkeletonMage = 527
}

#endregion

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