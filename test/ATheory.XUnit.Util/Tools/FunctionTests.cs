using ATheory.Util.Extensions;
using Xunit;

namespace ATheory.XUnit.Util.Tools
{
    public class FunctionTests
    {
        [Fact]
        public void JsonSave()
        {
            var config = new ConnConfig
            {
                Key1 = "xyz-yop",
                Key2 = "http://abc.com"
            };
            var result = new JsonShell<ConnConfig>().Save(config, @"C:\Dev\Configs\test.json");
            Assert.True(result);
        }
    }
}
