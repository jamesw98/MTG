using System.Data;
using MongoDB.Driver;
using Mtg.Functions.Models;

namespace Mtg.Functions.Utils;

public class MongoUtil
{
    private readonly IMongoDatabase _db;
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <exception cref="DataException"></exception>
    public MongoUtil()
    {
        var connString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        if (connString is null)
        {
            throw new DataException("Could not find MongoDbConnectionString environment variable.");
        }
        
        var mongo = new MongoClient(connString);
        _db = mongo.GetDatabase("jw-dev");
    }

    #region MATCHES

    public void CreateMatch(Match newMatch)
    {
        GetCollection<Match>().InsertOne(newMatch);
    }

    public List<Match> GetMatches()
    {
        return GetCollection<Match>()
            .AsQueryable()
            .ToList();
    }

    #endregion
    
    #region DECKS

    public void CreateDeck(Deck newDeck, User user)
    {
        newDeck.UserId = user.UserId;
        GetCollection<Deck>().InsertOne(newDeck);
    }
    
    public List<Deck> GetDecks()
    {
        return GetCollection<Deck>()
            .AsQueryable()
            .ToList();
    }

    public Deck? GetDeck(Guid guid)
    {
        return GetCollection<Deck>()
            .AsQueryable()
            .FirstOrDefault(x => x.DeckGuid == guid);
    }

    public List<Deck> GetDecksForUser(User user)
    {
        return GetCollection<Deck>()
            .AsQueryable()
            .Where(x => x.UserId == user.UserId)
            .ToList();
    }

    public List<Deck> SearchDecks(string query)
    {
        return GetCollection<Deck>()
            .AsQueryable()
            .Where(x => x.DeckName.ToLower().Contains(query.ToLower()))
            .ToList();
    }

    #endregion

    #region PRIVATE

    private IMongoCollection<T> GetCollection<T>()
    {
        return _db.GetCollection<T>(typeof(T).ToString());
    }

    #endregion
}