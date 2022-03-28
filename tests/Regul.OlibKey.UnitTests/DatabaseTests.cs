using System.Diagnostics;
using Regul.OlibKey.General;
using Regul.OlibKey.Structures;
using Xunit;
using Xunit.Abstractions;

namespace Regul.OlibKey.UnitTests;

public class DatabaseTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public DatabaseTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Database_Save_In_Xml()
    {
        Database database = new();
        database.DataList.Add(new Data
        {
            Name = "TestName",
            Login = new Login
            {
                Username = "TestUsername",
                Password = "TestPassword"
            }
        });

        string databaseXmlWithEmptyNodes = database.ToXml(false);
        Assert.NotEmpty(databaseXmlWithEmptyNodes);
        _testOutputHelper.WriteLine(databaseXmlWithEmptyNodes);

        _testOutputHelper.WriteLine("/////////////////////////");
        
        string databaseXml = database.ToXml(true);
        Assert.NotEmpty(databaseXml);
        _testOutputHelper.WriteLine(databaseXml);
        
        _testOutputHelper.WriteLine("");
        _testOutputHelper.WriteLine("/////////////////////////");
        _testOutputHelper.WriteLine("Saving if storage is empty");
        _testOutputHelper.WriteLine("/////////////////////////");

        database = new Database();
        databaseXml = database.ToXml(true);
        Assert.NotEmpty(databaseXml);
        _testOutputHelper.WriteLine(databaseXml);
    }

    [Fact]
    public void Database_Load_From_Xml()
    {
        string xml =
            "<Database xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n  <DataList>\n    <Data Name=\"TestName\" Favorite=\"false\">\n      <UseColor>false</UseColor>\n      <Login>\n        <Username>TestUsername</Username>\n        <Password>TestPassword</Password>\n      </Login>\n    </Data>\n  </DataList>\n</Database>";
        
        Database? database = Database.FromXml(xml);
        
        Assert.NotNull(database);
        _testOutputHelper.WriteLine(database.DataList[0].Name);
        _testOutputHelper.WriteLine(database.DataList[0].Login.Username);
        _testOutputHelper.WriteLine(database.DataList[0].Login.Password);
        _testOutputHelper.WriteLine(database.DataList[0].Color.ToString());
    }

    [Fact]
    public void Database_Compress_And_Encrypt()
    {
        Database database = new();
        database.DataList.Add(new Data
        {
            Name = "TestName",
            Login = new Login
            {
                Username = "TestUsername",
                Password = "TestPassword"
            }
        });

        Stopwatch stopwatch = new();

        stopwatch.Start();
        string databaseXml = database.ToXml(true);
        stopwatch.Stop();
        _testOutputHelper.WriteLine("Database ToXml: " + stopwatch.ElapsedMilliseconds + "ms");
        
        stopwatch.Restart();
        string compressedXml = Compressing.Compress(databaseXml);
        stopwatch.Stop();
        _testOutputHelper.WriteLine("Database Compress: " + stopwatch.ElapsedMilliseconds + "ms");
        
        stopwatch.Restart();
        string encryptedXml = Encryptor.EncryptString(compressedXml, "TestMasterPassword", 10000, 1);
        stopwatch.Stop();
        _testOutputHelper.WriteLine("Database Encrypt: " + stopwatch.ElapsedMilliseconds + "ms");

        Assert.True(true);
    }

    [Fact]
    public void Database_Decrypt_And_Decompress()
    {
        string encryptedXml =
            "U1yIcs5WH+Hr1oh16ZC9Yz6KUlrByotK8Lcs+oKgfKjWNv61CgTKNcgx7IU6zcpJhgInVlFaiD4TP2g2T8+DdFnnarTyBXwJlToe3zhTFPAotOjpor92COLAWnvAIqBrTBZY8osPuVkBAQ9yHKqZwaB9goG+DzyrERiFTTyJaRIIV+kpvAxTmCLnh/0DxUXE0kCeCD4AAJeiW3erHJ0iGdwunhp0YIoEL1JPBlw822kTUAOjAez9sli62PKyulbvtGvvRJ0YkNjbTCf5CryqZO6mESKOr9DM+oq2pFRjXoR1I7Nm1cI/qYhfg2auwNw+Cc7oKfZXomRK1aRCIGv7cKTdqjoB72qZJHpeQT+67594m6z+THMi5fAqbkQzJGeNKTdd2n58PbHtmBNbmk2q3A==";

        Stopwatch stopwatch = new();
        
        stopwatch.Start();
        string compressedXml = Encryptor.DecryptString(encryptedXml, "TestMasterPassword", 10000, 1);
        stopwatch.Stop();
        _testOutputHelper.WriteLine("Database Decrypt: " + stopwatch.ElapsedMilliseconds + "ms");
        
        stopwatch.Restart();
        string databaseXml = Compressing.Decompress(compressedXml);
        stopwatch.Stop();
        _testOutputHelper.WriteLine("Database Decompress: " + stopwatch.ElapsedMilliseconds + "ms");
        
        stopwatch.Restart();
        Database? database = Database.FromXml(databaseXml);
        stopwatch.Stop();
        _testOutputHelper.WriteLine("Database FromXml: " + stopwatch.ElapsedMilliseconds + "ms");
        
        Assert.True(true);
    }
}