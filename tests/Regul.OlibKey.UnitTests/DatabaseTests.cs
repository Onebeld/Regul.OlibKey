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
        Database database = new Database();
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
        
        Database database = Database.XmlToDatabase(xml);
        
        Assert.NotNull(database);
        _testOutputHelper.WriteLine(database.DataList[0].Name);
        _testOutputHelper.WriteLine(database.DataList[0].Login.Username);
        _testOutputHelper.WriteLine(database.DataList[0].Login.Password);
        _testOutputHelper.WriteLine(database.DataList[0].Color.ToString());
    }
}