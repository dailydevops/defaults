namespace NetEvolve.Defaults.Tests.Integration;

using System.Threading.Tasks;

public class TestCase
{
    [Test]
    public Task AlwaysTrue() => Task.FromResult(true);
}
