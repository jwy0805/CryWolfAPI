using AccountServer.DB;
using AccountServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AccountServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserAccountController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;
    private readonly TokenValidator _tokenValidator;
    
    public UserAccountController(AppDbContext context, TokenService tokenService, TokenValidator validator)
    {
        _context = context;
        _tokenService = tokenService;
        _tokenValidator = validator;
    }
    
    [HttpPost]
    [Route("CreateAccount")]
    public CreateUserAccountPacketResponse CreateAccount([FromBody] CreateUserAccountPacketRequired required)
    {
        var res = new CreateUserAccountPacketResponse();
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
            // 기본 덱, 컬렉션 생성
            CreateInitDeckAndCollection(newUser.UserId, new [] {
                UnitId.Hare, UnitId.Toadstool, UnitId.FlowerPot, 
                UnitId.Blossom, UnitId.TrainingDummy, UnitId.SunfloraPixie
            }, Camp.Sheep);
            
            CreateInitDeckAndCollection(newUser.UserId, new [] {
                UnitId.DogBowwow, UnitId.MoleRatKing, UnitId.MosquitoStinger, 
                UnitId.Werewolf, UnitId.CactusBoss, UnitId.SnakeNaga
            }, Camp.Wolf);
        }
        else
        {
            res.CreateOk = false;
            res.Message = "Duplicate ID";
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
        var res = new LoginUserAccountPacketResponse();
        var account = _context.User
            .AsNoTracking()
            .FirstOrDefault(user => user.UserAccount == required.UserAccount && user.Password == required.Password);

        if (account == null)
        {
            res.LoginOk = false;
        }
        else
        {
            var tokens = _tokenService.GenerateTokens(account.UserId);
            res.AccessToken = tokens.AccessToken;
            res.RefreshToken = tokens.RefreshToken;
            res.LoginOk = true;
        }
        
        return res;
    }

    [HttpPost]
    [Route("RefreshToken")]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequired request)
    {
        try
        {
            var tokens = _tokenValidator.RefreshAccessToken(request.RefreshToken);
            var response = new RefreshTokenResponse()
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken
            };
            return Ok(response);
        }
        catch (SecurityTokenException exception)
        {
            return Unauthorized(new { message = exception.Message });
        }
    }
}
