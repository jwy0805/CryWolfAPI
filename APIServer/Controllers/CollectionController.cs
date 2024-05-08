using Microsoft.AspNetCore.Mvc;
using AccountServer.DB;
using Microsoft.EntityFrameworkCore;
#pragma warning disable CS0472 // 이 형식의 값은 'null'과 같을 수 없으므로 식의 결과가 항상 동일합니다.

namespace AccountServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CollectionController
{
    private readonly AppDbContext _context;
    
    public CollectionController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpPost]
    [Route("GetCards")]
    public GetOwnedCardsPacketResponse GetCollection([FromBody] GetOwnedCardsPacketRequired required)
    {
        GetOwnedCardsPacketResponse res = new();
        var account = _context.User
            .AsNoTracking()
            .FirstOrDefault(user => user.UserAccount == required.UserAccount);

        if (account != null)
        {
            var units = _context.Unit.AsNoTracking().ToList();
            var userUnitIds = _context.UserUnit.AsNoTracking()
                .Where(userUnit => userUnit.UserId == account.UserId && userUnit.Count > 0)
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

        return res;
    }
    
    [HttpPost]
    [Route("GetDecks")]
    public GetInitDeckPacketResponse GetDeck([FromBody] GetInitDeckPacketRequired required)
    {
        GetInitDeckPacketResponse res = new();
        var account = _context.User
            .AsNoTracking()
            .FirstOrDefault(user => user.UserAccount == required.UserAccount);

        if (account != null)
        {
            var deckInfoList = _context.Deck
                .AsNoTracking()
                .Where(deck => deck.UserId == account.UserId)
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

        return res;
    }

    [HttpPut]
    [Route("UpdateDeck")]
    public UpdateDeckPacketResponse UpdateDeck([FromBody] UpdateDeckPacketRequired required)
    {
        UpdateDeckPacketResponse res = new();
        var account = _context.User
            .AsNoTracking()
            .FirstOrDefault(user => user.UserAccount == required.UserAccount);

        if (account != null)
        {   // 실제로 유저가 소유한 카드로 요청이 왔는지 검증 후 덱 업데이트
            var targetDeckId = required.DeckId;
            var unitToBeDeleted = required.UnitIdToBeDeleted;
            var unitToBeUpdated = required.UnitIdToBeUpdated;
            var userId = account.UserId;
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

        return res;
    }

    [HttpPut]
    [Route("UpdateLastDeck")]
    public UpdateLastDeckPacketResponse UpdateLastDeck([FromBody] UpdateLastDeckPacketRequired required)
    {
        UpdateLastDeckPacketResponse res = new();
        var account = _context.User
            .AsNoTracking()
            .FirstOrDefault(user => user.UserAccount == required.UserAccount);

        if (account != null)
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

        return res;
    }
}