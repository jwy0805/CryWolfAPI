namespace AccountServer.DB;

#pragma warning disable CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
public class CreateUserAccountPacketRequired
{
    public string UserAccount { get; set; }
    public string Password { get; set; }
}

public class CreateUserAccountPacketResponse
{
    public bool CreateOk { get; set; }
    public string UserAccount { get; set; }
}

public class CreateInitDeckPacketRequired
{
    public string UserAccount { get; set; }
}

public class CreateInitDeckPacketResponse
{
    public bool CreateDeckOk { get; set; }
}

public class LoginUserAccountPacketRequired
{
    public string UserAccount { get; set; }
    public string Password { get; set; }
}

public class ServerInfo
{
    public string Name { get; set; }
    public string Ip { get; set; }
    public int Port { get; set; }
    public int BusyScore { get; set; }
}

public class LoginUserAccountPacketResponse
{
    public bool LoginOk { get; set; }
    public int UserId { get; set; }
    // public int Token { get; set; }
    // public List<ServerInfo> ServerList { get; set; } = new();
}

public class GetOwnedCardsPacketRequired
{
    public string UserAccount { get; set; }
}

public class UnitInfo
{
    public UnitId Id { get; set; }
    public UnitClass Class { get; set; }
    public int Level { get; set; }
    public UnitId Species { get; set; }
    public UnitRole Role { get; set; }
    public Camp Camp { get; set; }
}

public class GetOwnedCardsPacketResponse
{
    public bool GetCardsOk { get; set; }
    public List<UnitInfo> OwnedCardList { get; set; }
    public List<UnitInfo> NotOwnedCardList { get; set; }
}

public class GetInitDeckPacketRequired
{
    public string UserAccount { get; set; }
}

public class DeckInfo
{
    public int DeckId { get; set; }
    public UnitInfo[] UnitInfo { get; set; }
    public int DeckNumber { get; set; }
    public int Camp { get; set; }
    public bool LastPicked { get; set; }
}

public class GetInitDeckPacketResponse
{
    public bool GetDeckOk { get; set; }
    public List<DeckInfo> DeckList { get; set; }
}

public class UpdateDeckPacketRequired
{
    public string UserAccount { get; set; }
    public int DeckId { get; set; }
    public UnitId UnitIdToBeDeleted { get; set; }
    public UnitId UnitIdToBeUpdated { get; set; }
}

public class UpdateDeckPacketResponse
{
    public int UpdateDeckOk { get; set; }
}

public class UpdateLastDeckPacketRequired
{
    public string UserAccount { get; set; }
    public Dictionary<int, bool> LastPickedInfo { get; set; }
}

public class UpdateLastDeckPacketResponse
{
    public bool UpdateLastDeckOk { get; set; }
}