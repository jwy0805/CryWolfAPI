using AccountServer.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserAccountController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public UserAccountController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpPost]
    [Route("CreateAccount")]
    public CreateUserAccountPacketResponse CreateAccount([FromBody] CreateUserAccountPacketRequired required)
    {
        CreateUserAccountPacketResponse res = new();
        var account = _context.User
            .AsNoTracking()
            .FirstOrDefault(user => user.UserAccount == required.UserAccount);

        if (account == null)
        {
            var newUser = new User
            {
                UserAccount = required.UserAccount,
                UserName = "",
                Password = required.Password,
                Role = UserRole.User,
                State = UserState.Activate,
                CreatedAt = DateTime.UtcNow,
                RankPoint = 0,
                Gold = 500,
                Gem = 0
            };
            
            _context.User.Add(newUser);
            var success = _context.SaveChangesExtended(); // 이 때 UserId가 생성
            newUser.UserName = $"Player{newUser.UserId}";
            
            res.CreateOk = success;
        }
        else
        {
            res.CreateOk = false;
        }
        
        return res;
    }

    [HttpPost]
    [Route("CreateInitDeck")]
    public CreateInitDeckPacketResponse CreateInitDeck([FromBody] CreateInitDeckPacketRequired required)
    {
        CreateInitDeckPacketResponse res = new();
        var account = _context.User
            .AsNoTracking()
            .FirstOrDefault(user => user.UserAccount == required.UserAccount);

        if (account != null)
        {
            CreateInitDeckAndCollection(account.UserId, new [] {
                UnitId.Hare, UnitId.Toadstool, UnitId.FlowerPot, 
                UnitId.Blossom, UnitId.TrainingDummy, UnitId.SunfloraPixie
            }, Camp.Sheep);
            
            CreateInitDeckAndCollection(account.UserId, new [] {
                UnitId.DogBowwow, UnitId.MoleRatKing, UnitId.MosquitoStinger, 
                UnitId.Werewolf, UnitId.CactusBoss, UnitId.SnakeNaga
            }, Camp.Wolf);
            
            res.CreateDeckOk = true;
        }
        else
        {
            res.CreateDeckOk = false;
        }

        return res;
    }
    
    private void CreateInitDeckAndCollection(int userId, UnitId[] unitIds, Camp camp)
    {
        foreach (var unitId in unitIds)
        {
            _context.UserUnit.Add(new UserUnit { UserId = userId, UnitId = unitId, Count = 1});
        }

        for (int i = 0; i < 3; i++)
        {
            var deck = new Deck { UserId = userId, Camp = camp, DeckNumber = i + 1};
            _context.Deck.Add(deck);
            _context.SaveChangesExtended();
        
            foreach (var unitId in unitIds)
            {
                _context.DeckUnit.Add(new DeckUnit
                { DeckId = deck.DeckId, UnitId = unitId });
            }
            _context.SaveChangesExtended();
        }
    }

    [HttpPost]
    [Route("Login")]
    public LoginUserAccountPacketResponse LoginAccount([FromBody] LoginUserAccountPacketRequired required)
    {
        LoginUserAccountPacketResponse res = new();
        var account = _context.User
            .AsNoTracking()
            .FirstOrDefault(user => user.UserAccount == required.UserAccount && user.Password == required.Password);

        if (account == null)
        {
            res.LoginOk = false;
        }
        else
        {
            res.LoginOk = true;
        }

        if (account != null) res.UserId = account.UserId;

        return res;
    }
}
