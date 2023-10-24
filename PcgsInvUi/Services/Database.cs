using System;
using System.Collections.Generic;
using System.Data.SQLite;
using PcgsInvUi.Models;

namespace PcgsInvUi.Services;

public class Database {
    public SQLiteConnection Connection { get; set; }
    private Boolean IsConnected { get; set; }
    
    public Database() {
        // Create a connection to SQLite
        Connection = CreateConnection();
        // Create a base table.
        CreateCollection("CollectionTable");
        Console.WriteLine("Connected to database.");
    }

    private SQLiteConnection CreateConnection() {
        // Create a connection.
        SQLiteConnection connection =
            new SQLiteConnection("DataSource=C:\\Users\\lvas4\\coin_collection");
        
        // Open the connection.
        try {
            connection.Open();
            IsConnected = true;
        }
        catch (Exception e) {
            Console.WriteLine(e.Message);
        }

        return connection;
    }

    public void CreateCollection(string collectionName) {
        // Create a table to store the collection.
        if (!IsConnected) {
            // TODO Create a NotConnectedToDatabaseException
            Console.WriteLine("Not connected to database.");
            return;
        }
        
        // Create a SQL command.
        SQLiteCommand cmd = Connection.CreateCommand();
        cmd.CommandText =
            $"CREATE TABLE IF NOT EXISTS {collectionName} (" +
            "PCGS_NUMBER TEXT," +
            "GRADE TEXT" +
            "NAME TEXT," +
            "YEAR INTEGER," +
            "DENOMINATION TEXT," +
            "MINT_MARK TEXT," +
            "PRICE_GUIDE_VALUE REAL," +
            "COIN_FACTS_LINK TEXT," +
            "MAJOR_VARIETY TEXT," +
            "MINOR_VARIETY TEXT," +
            "DIE_VARIETY TEXT," +
            "SERIES_NAME TEXT," +
            "CATEGORY TEXT," +
            "DESIGNATION TEXT," +
            "CERT_NUM INTEGER," +
            "QUANTITY INTEGER," +
            "PRIMARY KEY(PCGS_NUMBER, GRADE))";
        
        // Execute the command.
        cmd.ExecuteNonQuery();
    }

    public void InsertCoin(Coin newCoin) {
        // Use our existing connection to insert a coin to the database.
        if (!IsConnected) {
            // TODO Create a NotConnectedToDatabaseException
            Console.WriteLine("Not connected to database.");
            return;
        }
        
        // Create a SQL command.
        SQLiteCommand cmd = Connection.CreateCommand();
        cmd.CommandText =
            "INSERT INTO CollectionTable (PCGS_NUMBER, GRADE, YEAR," +
            "DENOMINATION, MINT_MARK, PRICE_GUIDE_VALUE, COIN_FACTS_LINK," +
            "MAJOR_VARIETY, MINOR_VARIETY, DIE_VARIETY, SERIES_NAME, CATEGORY, DESIGNATION," +
            "CERT_NUM, QUANTITY) " +
            "VALUES (@PcgsNumber, @Grade, @Year, @Denomination, @MintMark, @PriceGuideValue," +
            "@CoinFactsLink, @MajorVariety, @MinorVariety, @DieVariety, @SeriesName, @Category, @Designation," +
            "@CertificateNumber, @Quantity)";
        cmd.Parameters.AddWithValue("@PcgsNumber", newCoin.PCGSNo);
        cmd.Parameters.AddWithValue("@Grade", newCoin.Grade);
        cmd.Parameters.AddWithValue("@Year", newCoin.Year);
        cmd.Parameters.AddWithValue("@Denomination", newCoin.Denomination);
        cmd.Parameters.AddWithValue("@MintMark", newCoin.MintMark);
        cmd.Parameters.AddWithValue("@PriceGuideValue", newCoin.PriceGuideValue);
        cmd.Parameters.AddWithValue("@CoinFactsLink", newCoin.CoinFactsLink);
        cmd.Parameters.AddWithValue("@MajorVariety", newCoin.MajorVariety);
        cmd.Parameters.AddWithValue("@MinorVariety", newCoin.MinorVariety);
        cmd.Parameters.AddWithValue("@DieVariety", newCoin.DieVariety);
        cmd.Parameters.AddWithValue("@SeriesName", newCoin.SeriesName);
        cmd.Parameters.AddWithValue("@Category", newCoin.Category);
        cmd.Parameters.AddWithValue("@Designation", newCoin.Designation);
        cmd.Parameters.AddWithValue("@CertificateNumber", newCoin.CertificateNumber);
        cmd.Parameters.AddWithValue("@Quantity", newCoin.Quantity);
        
        // Execute the command.
        cmd.ExecuteNonQuery();
    }

    public List<Coin> GetCollection(string collectionName) {
        SQLiteCommand cmd = Connection.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {collectionName}";

        var coins = new List<Coin>();
        
        using (var reader = cmd.ExecuteReader()) {
            while (reader.Read()) {
                // Create coin from reader then add to list.
                coins.Add(CoinFromReader(reader));
            }
        }
        
        return coins;
    }
    
    public Coin? GetCoin(int pcgsNumber, string grade) {
        // Get the coin based off of the pcgs number.
        SQLiteCommand cmd = Connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM CollectionTable WHERE Id = @Id AND Grade = @Grade";
        cmd.Parameters.AddWithValue("@Id", pcgsNumber);
        cmd.Parameters.AddWithValue("@Grade", grade);

        Coin? returnCoin = null;
        
        // Execute the command.
        using (var reader = cmd.ExecuteReader()) {
            while (reader.Read()) {
                returnCoin = CoinFromReader(reader);
            }
        }

        return returnCoin;
    }
    
    private Coin CoinFromReader(SQLiteDataReader reader) {
        Coin coin = new Coin();
        coin.PCGSNo = reader.GetString(0);
        coin.Grade = reader.GetString(1);
        coin.Year = reader.GetInt32(2);
        coin.Denomination = reader.GetString(3);
        coin.MintMark = reader.GetString(4);
        coin.PriceGuideValue = reader.GetDouble(5);
        coin.CoinFactsLink = reader.GetString(6);
        coin.MajorVariety = reader.GetString(7);
        coin.MinorVariety = reader.GetString(8);
        coin.DieVariety = reader.GetString(9);
        coin.SeriesName = reader.GetString(10);
        coin.Category = reader.GetString(11);
        coin.Designation = reader.GetString(12);
        coin.CertificateNumber = reader.GetInt32(13);
        coin.Quantity = reader.GetInt32(14);
        coin.TotalPrice = coin.Quantity * coin.PriceGuideValue;
        return coin;
    }

    public void DeleteCoin(Coin delCoin) {
        SQLiteCommand cmd = Connection.CreateCommand();
        cmd.CommandText = "DELETE FROM CollectionTable WHERE PCGS_NUMBER = @PcgsNumber AND GRADE = @Grade";
        cmd.Parameters.AddWithValue("@PcgsNumber", delCoin.PCGSNo);
        cmd.Parameters.AddWithValue("@Grade", delCoin.Grade);
        
        cmd.ExecuteNonQuery();
    }
}