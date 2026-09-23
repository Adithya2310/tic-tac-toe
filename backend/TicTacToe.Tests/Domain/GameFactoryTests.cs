using TicTacToe.Api.Domain;

namespace TicTacToe.Tests.Domain;

public sealed class GameFactoryTests
{
    private readonly GameFactory _factory = new(new StandardRules());

    [Fact]
    public void Create_TwoPlayer_ConfiguresTwoHumanPlayers()
    {
        var game = _factory.Create(GameType.TwoPlayer);

        Assert.False(game.PlayerX.IsComputer);
        Assert.False(game.PlayerO.IsComputer);
    }

    [Fact]
    public void Create_Computer_ConfiguresOnlyPlayerOAsComputer()
    {
        var game = _factory.Create(GameType.Computer);

        Assert.False(game.PlayerX.IsComputer);
        Assert.True(game.PlayerO.IsComputer);
    }
}
