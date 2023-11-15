using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Threading.Tasks;
using PcgsInvUi.Models;

namespace PcgsInvUi.Services;

public class CoinDatabase {
    public SQLiteConnection Connection { get; set; }
    private Boolean IsConnected { get; set; }
    public ObservableCollection<Coin> Collection { get; set; }

    private PcgsClient _pcgsClient;

    public CoinDatabase() {
        // Create a connection to SQLite
        Connection = CreateConnection();
        // Create a client to access the API.
        _pcgsClient = new PcgsClient(
            "eAb8gS0I2XAvT_5gJeiGJaglMia1Tk-oB4kJUK6kuafyrny_S61vIJY-Ikl4nCQM67wrdxzUqLVWTV2kBSxD3d5XNBHxHnYBhcSS6dOPug0hZaF3qAv56df3gYSzOGh9Tif5y0eP3Iw0LrqKDr1Hj-dk6SV6GKog2IIqCPQhhHH8FMTWBTYO-_O8cx7qLdM5GM8KlTsic6g3VRUhM8EA_4OO04dCfmNLGhqINRl3jGZ0Q4ziI8fng2bVWsIyteqiPzUn10rIQ3-OPpqVZG_DxeOmOejj4GzbUNyqUOajy-nr5rYY");
        // Create a base table.
        CreateCollection("CollectionTable");
        Console.WriteLine("Connected to database.");
        Collection = new ObservableCollection<Coin>();
        GetCollection("CollectionTable");
        Console.WriteLine("Got collection.");
    }

    ~CoinDatabase() {
        Console.WriteLine("Destructor called, saving collection.");
        // Save collection.
        UpdateCollection("CollectionTable");
        // Close the connection.
        Connection.Close();
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
            "NOTES TEXT," +
            "PRIMARY KEY(PCGS_NUMBER, GRADE))";
        
        // Execute the command.
        cmd.ExecuteNonQuery();
    }

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

    public void UpdateCollection(string collectionName) {
        // Update the database with the contents of the collection.
        // TODO This is a very inefficient way of doing this. I should be able to do this in one command.
        foreach (var coin in Collection) {
            SaveCoin(collectionName, coin);  // CAREFUL! This will not create a coin if it doesn't currently exist.
        }
    }
    
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
    
    public void DeleteCoin(Coin delCoin) {
        SQLiteCommand cmd = Connection.CreateCommand();
        cmd.CommandText = "DELETE FROM CollectionTable WHERE PCGS_NUMBER = @PcgsNumber AND GRADE = @Grade";
        cmd.Parameters.AddWithValue("@PcgsNumber", delCoin.PCGSNo);
        cmd.Parameters.AddWithValue("@Grade", delCoin.Grade);
        
        cmd.ExecuteNonQuery();
    }
    
    private int GetGradeFromString(string grade) {
        var actualGrade = int.Parse(grade.Substring(grade.Length - 2));
        Console.WriteLine($"Grade is {actualGrade}");
        return actualGrade;
    }
}