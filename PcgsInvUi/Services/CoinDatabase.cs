﻿using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Threading.Tasks;
using PcgsInvUi.Models;

namespace PcgsInvUi.Services;

public class CoinDatabase {
    public SQLiteConnection Connection { get; set; }
    private Boolean IsConnected { get; set; }
    public ObservableCollection<Coin> Collection { get; set; }
    public Boolean IsClientConnected { get; set; }

    private PcgsClient? _pcgsClient;
    
    /// <summary>
    /// Creates and initilizes a new instance of the CoinDatabase.
    /// Must call TryInitApiClient() after creating a new instance.
    /// </summary>
    public CoinDatabase() {
        // Create a connection to SQLite
        Connection = CreateConnection();
        // Create a base table.
        CreateCollection("CollectionTable");
        CreateApiTable();
        Console.WriteLine("Connected to database.");
        Collection = new ObservableCollection<Coin>();
        GetCollection("CollectionTable");
        Console.WriteLine("Got collection.");
    }

    /// <summary>
    /// Destructor. Currently not used, due to the strange nature of desktop applications.
    /// </summary>
    ~CoinDatabase() {
        Console.WriteLine("Destructor called, saving collection.");
        // Save collection.
        UpdateCollection("CollectionTable");
        // Close the connection.
        Connection.Close();
    }
    
    /// <summary>
    /// Attempts to intitialize the API client by selecting the API key from the database.
    /// </summary>
    /// <returns>True if the API key was found, false otherwise.</returns>
    public async Task<bool> TryInitApiClient() {
        Console.Write("Attempting to initialize API client... ");
        SQLiteCommand cmd = Connection.CreateCommand();
        cmd.CommandText = "SELECT API_KEY FROM ApiTable";

        using (var reader = cmd.ExecuteReader()) {
            if (reader.Read()) {
                var apiKey = reader.GetString(0);
                _pcgsClient = await PcgsClient.CreateClient(apiKey);

                // Check that the client is valid.
                if (_pcgsClient is null) {
                    IsClientConnected = false;
                    Console.WriteLine("Failed. API key was invalid.");
                    return IsClientConnected;
                }

                IsClientConnected = true;
                Console.WriteLine("Done! API key was found from database.");
            }
            else {
                IsClientConnected = false;
                Console.WriteLine("Failed. API key not found.");
            }
        }
        return IsClientConnected;
    }

    /// <summary>
    /// Atttempts to initialize the PCGS API client.
    /// </summary>
    /// <param name=apiKey>The API key to verify.</param>
    /// <returns>A Task with a return value that returns true if the client intializes properly.</returns>
    public async Task<bool> TryInitApiClient(string apiKey) {
        Console.Write("Attempting to initialize API client with key param... ");
        _pcgsClient = await PcgsClient.CreateClient(apiKey);
        // Check if the API key is valid and if it is, then add to the database.
        if (_pcgsClient is null) {
            IsClientConnected = false;
            Console.WriteLine("Failed. API key was invalid.");
            return IsClientConnected;
        }

        var cmd = Connection.CreateCommand();
        cmd.CommandText = $"INSERT INTO ApiTable (API_KEY, REQUESTS_REMAINING) VALUES (\"{apiKey}\", 1000)";
        await cmd.ExecuteNonQueryAsync();

        IsClientConnected = true;
        Console.WriteLine("Done! API key was added to database.");
        return IsClientConnected;
    }

    /// <summary>
    /// Gets the collection from the database and stores it in the Collection property.
    /// Creates the collection table if it does not exist already.
    /// </summary>
    /// <returns>The SQLiteConnection that corresponds to the connected database.</returns>
    private SQLiteConnection CreateConnection() {
        // Check if database exists.
        if (!System.IO.File.Exists("coin_collection")) {
            // If not, create the database.
            Console.Write("Creating database... ");
            SQLiteConnection.CreateFile("coin_collection.sqlite");
            Console.WriteLine("Done.");
        }
        
        // Create a connection to the database.
        SQLiteConnection connection =
            new SQLiteConnection("DataSource=coin_collection");
        
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

    /// <summary>
    /// Creates the table that stores the API key and the number of requests remaining.
    /// </summary>
    public void CreateApiTable() {
        // Create a table to store the API key and the number of requests remaining.
        if (!IsConnected) {
            // TODO Create a NotConnectedToDatabaseException
            Console.WriteLine("Not connected to database.");
            return;
        }

        // Create a SQL command.
        SQLiteCommand cmd = Connection.CreateCommand();
        cmd.CommandText =
          "CREATE TABLE IF NOT EXISTS ApiTable (" +
          "API_KEY TEXT, " +
          "REQUESTS_REMAINING INTEGER, " + 
          "PRIMARY KEY(API_KEY))";

        // Execute the command
        cmd.ExecuteNonQuery();
        Console.WriteLine("API table initialized.");
    }

    /// <summary>
    /// Creates a table to store the collection.
    /// </summary>
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
            "NOTES TEXT," +
            "PRIMARY KEY(PCGS_NUMBER, GRADE))";
        
        // Execute the command.
        cmd.ExecuteNonQuery();
        Console.WriteLine("Collection table initialized.");
    }

    /// <summary>
    /// Inserts a Coin object into the database.
    /// </summary>
    public void InsertCoin(Coin coin) {
        Console.WriteLine("Inserting coin...");
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
            "CERT_NUM, QUANTITY, NOTES) " +
            $"VALUES ('{coin.PCGSNo}', '{coin.Grade}', {coin.Year}, '{coin.Denomination}', '{coin.MintMark}', {coin.PriceGuideValue}," +
            $"'{coin.CoinFactsLink}', '{coin.MajorVariety}', '{coin.MinorVariety}', '{coin.DieVariety}', '{coin.SeriesName}', " +
            $"'{coin.Category}', '{coin.Designation}', {coin.CertificateNumber}, {coin.Quantity}, @Notes)";
        cmd.Parameters.AddWithValue("@Notes", coin.Notes);
        
        // Execute the command.
        cmd.ExecuteNonQuery();
        Console.WriteLine("Coin inserted.");
    }

    /// <summary>
    /// Creates a Coin object and saves it to the database.
    /// </summary>
    /// <param name="pcgsNumber">The PCGS number of the coin.</param>
    /// <param name="grade">The grade of the coin.</param>
    /// <param name="quantity">The quantity of the coin.</param>
    /// <returns>The error type returned by the API.</returns>
    public async Task<PcgsClient.ErrorType> CreateCoin(int pcgsNumber, string grade, int quantity) {
        Console.WriteLine("Creating coin...");
        // Create a coin by calling the API, then insert it into the database.
        // First, check if the coin already exists - if it does, just update the quantity by adding the amount entered.
        var coin = GetCoin(pcgsNumber, grade);
        if (coin is not null) {
            coin.Quantity += quantity;
            SaveCoin("CollectionTable", coin);
            return PcgsClient.ErrorType.None;
        }
        
        // Otherwise, build a new coin with the API and insert it.
        var apiResult = await _pcgsClient.GetCoinFactsByGrade(pcgsNumber, grade);
        if (apiResult.Item1 == PcgsClient.ErrorType.None) {
            var newCoin = apiResult.Item2;
            newCoin.Quantity = quantity;
            InsertCoin(newCoin);
            Collection.Add(newCoin);
            return PcgsClient.ErrorType.None;
        }
        
        Console.WriteLine("Error getting coin from API - {0}", apiResult.Item1);
        return apiResult.Item1;
    }
    
    /// <summary>
    /// Gets a coin from the database.
    /// </summary>
    /// <param name="pcgsNumber">The PCGS number of the coin.</param>
    /// <param name="grade">The grade of the coin.</param>
    /// <returns>The coin if it exists, null otherwise.</returns>
    public Coin? GetCoin(int pcgsNumber, string grade) {
        Console.WriteLine($"Getting coin {pcgsNumber}, {grade}...");
        int gradeInt = GetGradeFromString(grade);
        
        // Check the collection to see if the coin exists.
        // We can't make a more efficient algorithm since there's not really an ordering to the collection.
        foreach (var coin in Collection) {
            if (coin.PCGSNo == pcgsNumber.ToString() && coin.Grade.Contains(gradeInt.ToString())) {  // This has the possibility of a false positive for single digit grades.
                if (gradeInt < 10)
                    if (char.IsDigit(coin.Grade[3])) continue;  // TODO For right now, we're going to assume that the first 2 characters are ciruclation strings, and the next 2 are grade numbers.
                Console.WriteLine("Found coin.");
                return coin;
            }
        }
        
        Console.WriteLine("Couldn't find coin.");
        return null;
    }

    /// <summary>
    /// Used to get all Coin objects within the database.
    /// </summary>
    /// <param name="collectionName">The name of the collection table.</param>
    public void GetCollection(string collectionName) {
        Console.WriteLine("Getting collection...");
        SQLiteCommand cmd = Connection.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {collectionName}";
        
        using (var reader = cmd.ExecuteReader()) {
            while (reader.Read()) {
                // Create coin from reader then add to list.
                Collection.Add(CoinFromReader(reader));
            }
        }
    }

    /// <summary>
    /// Creates a CSV formatted string from a collection.
    /// </summary>
    /// <param name="collectionName">The name of the collection table.</param>
    /// <returns>A CSV formatted string.</returns>
    public String CollectionToCSV(string collectionName) {
        var cmd = Connection.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {collectionName}";

        var headersCommand = Connection.CreateCommand();
        headersCommand.CommandText = $"PRAGMA table_info({collectionName})";
        
        var resultString = "";
        var numHeaders = 0;
        
        // Get the headers.
        using (var reader = headersCommand.ExecuteReader()) {
            while (reader.Read()) {
                resultString += reader.GetString(1) + ",";
                numHeaders++;
            }
        }

        resultString += "\n";

        // Is there a more efficient way of doing this? This is running at O(n^2) time.
        using (var reader = cmd.ExecuteReader()) {
            while (reader.Read()) {
                // Read information from the reader, then add to CSV string.
                for (int i = 0; i < numHeaders; i++) {
                    // Get the type of each column.
                    var type = reader.GetFieldType(i);
                    Console.WriteLine("Type: " + type);
                    if (type == typeof(Int64)) {
                        resultString += reader.GetInt32(i) + ",";
                    }
                    else if (type == typeof(Double)) {
                        resultString += reader.GetDouble(i) + ",";
                    }
                    else if (type == typeof(String)) {
                        resultString += reader.GetString(i) + ",";
                    }
                    else {
                        resultString += "NULL" + ",";
                    }
                }
                resultString += "\n";
            }
        }

        return resultString;
    }

    /// <summary>
    /// Saves a Coin object to the database.
    /// </summary>
    /// <param name="collectionName">The name of the collection table.</param>
    /// <param name="coin">The coin to save.</param>
    public void SaveCoin(string collectionName, Coin coin) {
        // Get and replace the coin from the database.
        Console.WriteLine("Saving coin...");
        SQLiteCommand cmd = Connection.CreateCommand();
        cmd.CommandText = $"UPDATE {collectionName} " +
                          $"SET YEAR = {coin.Year}, " +
                          $"DENOMINATION = \'{coin.Denomination}\', " +
                          $"MINT_MARK = \'{coin.MintMark}\', " +
                          $"PRICE_GUIDE_VALUE = \'{coin.PriceGuideValue}\', " +
                          $"COIN_FACTS_LINK = \'{coin.CoinFactsLink}\', " +
                          $"MAJOR_VARIETY = \'{coin.MajorVariety}\', " +
                          $"MINOR_VARIETY = \'{coin.MinorVariety}\', " +
                          $"DIE_VARIETY = \'{coin.DieVariety}\', " +
                          $"SERIES_NAME = \'{coin.SeriesName}\', " +
                          $"CATEGORY = \'{coin.Category}\', " +
                          $"DESIGNATION = \'{coin.Designation}\', " +
                          $"CERT_NUM = \'{coin.CertificateNumber}\', " +
                          $"QUANTITY = {coin.Quantity}, " +
                          $"NOTES = @Notes " + 
                          $"WHERE PCGS_NUMBER = {coin.PCGSNo} AND GRADE = \'{coin.Grade}\'";
        cmd.Parameters.AddWithValue("@Notes", coin.Notes);
        Console.WriteLine(cmd.CommandText);
        // Execute the command.
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Synchronizes the database with the current contents of the specified collection.
    /// </summary>
    /// <param name="collectionName">The name of the collection table.</param>
    public void UpdateCollection(string collectionName) {
        // Update the database with the contents of the collection.
        // TODO This is a very inefficient way of doing this. I should be able to do this in one command.
        foreach (var coin in Collection) {
            SaveCoin(collectionName, coin);  // CAREFUL! This will not create a coin if it doesn't currently exist.
        }
    }
    
    /// <summary>
    /// Creates a Coin from the information in a reader specifically from this database.
    /// </summary>
    /// <param name="reader">The reader to get the information from.</param>
    private Coin CoinFromReader(SQLiteDataReader reader) {
        // Create a coin while filtering out any possible null values - they throw exceptions.
        Coin coin = new Coin();
        if (!reader.IsDBNull(0)) coin.PCGSNo = reader.GetString(0);
        if (!reader.IsDBNull(1)) coin.Grade = reader.GetString(1);
        if (!reader.IsDBNull(2)) coin.Year = reader.GetInt32(2);
        if (!reader.IsDBNull(3)) coin.Denomination = reader.GetString(3);
        if (!reader.IsDBNull(4)) coin.MintMark = reader.GetString(4);
        if (!reader.IsDBNull(5)) coin.PriceGuideValue = reader.GetDouble(5);
        if (!reader.IsDBNull(6)) coin.CoinFactsLink = reader.GetString(6);
        if (!reader.IsDBNull(7)) coin.MajorVariety = reader.GetString(7);
        if (!reader.IsDBNull(8)) coin.MinorVariety = reader.GetString(8);
        if (!reader.IsDBNull(9)) coin.DieVariety = reader.GetString(9);
        if (!reader.IsDBNull(10)) coin.SeriesName = reader.GetString(10);
        if (!reader.IsDBNull(11)) coin.Category = reader.GetString(11);
        if (!reader.IsDBNull(12)) coin.Designation = reader.GetString(12);
        if (!reader.IsDBNull(13)) coin.CertificateNumber = reader.GetInt32(13);
        if (!reader.IsDBNull(14)) coin.Quantity = reader.GetInt32(14);
        if (!reader.IsDBNull(15)) coin.Notes = reader.GetString(15);
        coin.TotalPrice = coin.Quantity * coin.PriceGuideValue;
        return coin;
    }
    
    /// <summary>
    /// Deletes a Coin object from the database.
    /// </summary>
    /// <param name="delCoin">The coin to delete.</param>
    public void DeleteCoin(Coin delCoin) {
        SQLiteCommand cmd = Connection.CreateCommand();
        cmd.CommandText = "DELETE FROM CollectionTable WHERE PCGS_NUMBER = @PcgsNumber AND GRADE = @Grade";
        cmd.Parameters.AddWithValue("@PcgsNumber", delCoin.PCGSNo);
        cmd.Parameters.AddWithValue("@Grade", delCoin.Grade);
        
        cmd.ExecuteNonQuery();
    }
    
    /// <summary>
    /// Gets the grade from a string returned from the main window.
    /// These are always in a format similar to "MS-65", so we can just get the last two characters.
    /// </summary>
    /// <param name="grade">The grade string.</param>
    private int GetGradeFromString(string grade) {
        var actualGrade = int.Parse(grade.Substring(grade.Length - 2));
        Console.WriteLine($"Grade is {actualGrade}");
        return actualGrade;
    }
}

public class NotConnectedToDatabaseException : Exception {
    public NotConnectedToDatabaseException() : base("Not connected to database.") { }
}
