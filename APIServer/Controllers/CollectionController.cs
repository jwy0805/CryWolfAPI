using Microsoft.AspNetCore.Mvc;
using AccountServer.DB;
using AccountServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
#pragma warning disable CS0472 // 이 형식의 값은 'null'과 같을 수 없으므로 식의 결과가 항상 동일합니다.

namespace AccountServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CollectionController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;
    private readonly TokenValidator _tokenValidator;
    
    public CollectionController(AppDbContext context, TokenService ts, TokenValidator tv)
    {
        _context = context;
        _tokenService = ts;
        _tokenValidator = tv;
    }
    
    [HttpPost]
    [Route("GetCards")]
    public IActionResult GetCollection([FromBody] GetOwnedCardsPacketRequired required)
    {
        var principal = _tokenValidator.ValidateAccessToken(required.AccessToken);
        if (principal == null) return Unauthorized();
        
        var res = new GetOwnedCardsPacketResponse();
        var userId = _tokenValidator.GetUserIdFromAccessToken(principal);

        if (userId != null)
        {
            var units = _context.Unit.AsNoTracking().ToList();
            var userUnitIds = _context.UserUnit.AsNoTracking()
                .Where(userUnit => userUnit.UserId == userId && userUnit.Count > 0)
                .Select(userUnit => userUnit.UnitId)
                .ToList();

            var ownedCardList = units
                .Where(unit => userUnitIds.Contains(unit.UnitId))
                .Select(unit => new UnitInfo
                {
                    Id = unit.UnitId,
                    Class = unit.Class,
                    Level = unit.Level,
                    Species = unit.Species,
                    Role = unit.Role,
                    Camp = unit.Camp
                }).ToList();
            
            var notOwnedCardList = units
                .Where(unit => userUnitIds.Contains(unit.UnitId) == false)
                .Where(unit => ownedCardList.All(unitInfo => unitInfo.Species != unit.Species) && unit.Level == 3)
                .Select(unit => new UnitInfo
                {
                    Id = unit.UnitId,
                    Class = unit.Class,
                    Level = unit.Level,
                    Species = unit.Species,
                    Role = unit.Role,
                    Camp = unit.Camp
                }).ToList();
            
            res.OwnedCardList = ownedCardList;
            res.NotOwnedCardList = notOwnedCardList;
            res.GetCardsOk = true;
        }
        else
        {
            res.GetCardsOk = false;
        }

        return Ok(res);
    }
    
    [HttpPost]
    [Route("GetDecks")]
    public IActionResult GetDeck([FromBody] GetInitDeckPacketRequired required)
    {
        var principal = _tokenValidator.ValidateAccessToken(required.AccessToken);
        if (principal == null) return Unauthorized();
        
        var res = new GetInitDeckPacketResponse();
        var userId = _tokenValidator.GetUserIdFromAccessToken(principal);

        if (userId != null)
        {
            var deckInfoList = _context.Deck
                .AsNoTracking()
                .Where(deck => deck.UserId == userId)
                .Select(deck => new DeckInfo
                {
                    DeckId = deck.DeckId,
                    UnitInfo = _context.DeckUnit.AsNoTracking()
                        .Where(deckUnit => deckUnit.DeckId == deck.DeckId)
                        .Select(deckUnit => _context.Unit.AsNoTracking()
                            .FirstOrDefault(unit => unit.UnitId == deckUnit.UnitId))
                        .Where(unit => unit != null)
                        .Select(unit => new UnitInfo
                        {
                            Id = unit!.UnitId,
                            Class = unit.Class,
                            Level = unit.Level,
                            Species = unit.Species,
                            Role = unit.Role,
                            Camp = unit.Camp
                        }).ToArray(),
                    DeckNumber = deck.DeckNumber,
                    Camp = (int)deck.Camp,
                    LastPicked = deck.LastPicked
                }).ToList();
            
            res.DeckList = deckInfoList;
            res.GetDeckOk = true;
        }
        else
        {
            res.GetDeckOk = false;
        }

        return Ok(res);
    }

    [HttpPut]
    [Route("UpdateDeck")]
    public IActionResult UpdateDeck([FromBody] UpdateDeckPacketRequired required)
    {
        var principal = _tokenValidator.ValidateAccessToken(required.AccessToken);
        if (principal == null) return Unauthorized();

        var res = new UpdateDeckPacketResponse();
        var userId = _tokenValidator.GetUserIdFromAccessToken(principal);


        if (userId != null)
        {   // 실제로 유저가 소유한 카드로 요청이 왔는지 검증 후 덱 업데이트
            var targetDeckId = required.DeckId;
            var unitToBeDeleted = required.UnitIdToBeDeleted;
            var unitToBeUpdated = required.UnitIdToBeUpdated;
            var deckUnit = _context.DeckUnit
                .FirstOrDefault(deckUnit => 
                    deckUnit.DeckId == targetDeckId && 
                    deckUnit.UnitId == unitToBeDeleted && 
                    _context.UserUnit.Any(userUnit => userUnit.UnitId == unitToBeUpdated && userUnit.UserId == userId));
            
            if (deckUnit != null)
            {
                _context.DeckUnit.Remove(deckUnit);
                _context.SaveChangesExtended();
                
                var newDeckUnit = new DeckUnit { DeckId = targetDeckId, UnitId = unitToBeUpdated };
                _context.DeckUnit.Add(newDeckUnit);
                _context.SaveChangesExtended();
                
                res.UpdateDeckOk = 0;
            }
            else
            {
                res.UpdateDeckOk = 1;
            }
        }
        else
        {
            res.UpdateDeckOk = 2;
        }

        return Ok(res);
    }

    [HttpPut]
    [Route("UpdateLastDeck")]
    public IActionResult UpdateLastDeck([FromBody] UpdateLastDeckPacketRequired required)
    {
        var principal = _tokenValidator.ValidateAccessToken(required.AccessToken);
        if (principal == null) return Unauthorized();
        
        var res = new UpdateLastDeckPacketResponse();
        var userId = _tokenValidator.GetUserIdFromAccessToken(principal);

        if (userId != null)
        {
            var targetDeck = required.LastPickedInfo;
            var targetDeckIds = targetDeck.Keys.ToList();
            var decks = _context.Deck
                .Where(deck => targetDeckIds.Contains(deck.DeckId)).ToList();
            foreach (var deck in decks) deck.LastPicked = targetDeck[deck.DeckId];
            _context.SaveChangesExtended();
            res.UpdateLastDeckOk = true;
        }
        else
        {
            res.UpdateLastDeckOk = false;
        }

        return Ok(res);
    }
}