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

    #region DECKS

    public void CreateDeck(Deck newDeck, User user)
    {
        newDeck.User = user;
        GetCollection<Deck>().InsertOne(newDeck);
    }
    
    public List<Deck> GetDecks()
    {
        var result = GetCollection<Deck>()
            .AsQueryable()
            .ToList();
        return result;
    }

    public Deck? GetDeck(Guid guid)
    {
        return GetCollection<Deck>()
            .AsQueryable()
            .FirstOrDefault(x => x.DeckGuid == guid);
    }

    public List<Deck> GetDecksForUser(User user)
    {
        var result = GetCollection<Deck>()
            .AsQueryable()
            .Where(x => x.User.UserId == user.UserId)
            .ToList();
        return result;
    }

    #endregion

    #region PRIVATE

    private IMongoCollection<T> GetCollection<T>()
    {
        return _db.GetCollection<T>(typeof(T).ToString());
    }

    #endregion
}