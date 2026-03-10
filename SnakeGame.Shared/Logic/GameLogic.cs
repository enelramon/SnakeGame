using System.Collections.Immutable;
using SnakeGame.Shared.Models;

namespace SnakeGame.Shared.Logic;

/// <summary>
/// Lógica del juego Snake implementada como funciones puras.
/// Cada método recibe un <see cref="GameState"/> y devuelve uno nuevo sin mutar el original.
/// </summary>
public static class GameLogic
{
    public static GameState CreateInitialState(int gridWidth, int gridHeight)
    {
        var center = new Coordenada(gridWidth / 2, gridHeight / 2);

        var snake = ImmutableList.Create(
            center,
            center with { X = center.X - 1 },
            center with { X = center.X - 2 }
        );

        return new GameState(
            Snake:            snake,
            Food:             SpawnFood(snake, gridWidth, gridHeight),
            CurrentDirection: Direction.Right,
            NextDirection:    Direction.Right,
            Score:            0,
            HighScore:        0,
            Status:           GameStatus.NotStarted,
            GridWidth:        gridWidth,
            GridHeight:       gridHeight
        );
    }

    /// <summary>Avanza el juego un tick. Si no está en progreso, devuelve el estado sin cambios.</summary>
    public static GameState Tick(GameState state) =>
        state.Status != GameStatus.Playing
            ? state
            : MoveSnake(state with { CurrentDirection = state.NextDirection });

    /// <summary>Solicita cambio de dirección. Ignora movimientos opuestos para evitar muerte instantánea.</summary>
    public static GameState ChangeDirection(GameState state, Direction newDirection) =>
        IsOppositeDirection(state.CurrentDirection, newDirection)
            ? state
            : state with { NextDirection = newDirection };

    public static GameState StartGame(GameState state) =>
        state with { Status = GameStatus.Playing };

    /// <summary>Alterna entre Playing y Paused.</summary>
    public static GameState TogglePause(GameState state) =>
        state.Status switch
        {
            GameStatus.Playing => state with { Status = GameStatus.Paused },
            GameStatus.Paused  => state with { Status = GameStatus.Playing },
            _                  => state
        };

    /// <summary>Reinicia el juego preservando el HighScore de la sesión.</summary>
    public static GameState RestartGame(GameState state) =>
        CreateInitialState(state.GridWidth, state.GridHeight) with
        {
            HighScore = state.HighScore,
            Status    = GameStatus.Playing
        };

    // -------------------------------------------------------------------------

    private static GameState MoveSnake(GameState state)
    {
        var newHead = CalculateNextHead(state.Snake[0], state.CurrentDirection);

        if (IsCollision(newHead, state.Snake, state.GridWidth, state.GridHeight))
            return state with { Status = GameStatus.GameOver };

        var ateFood = newHead == state.Food;

        var newSnake = ateFood
            ? state.Snake.Insert(0, newHead)
            : state.Snake.Insert(0, newHead).RemoveAt(state.Snake.Count - 1);

        var newScore = ateFood ? state.Score + 10 : state.Score;
        var newFood  = ateFood
            ? SpawnFood(newSnake, state.GridWidth, state.GridHeight)
            : state.Food;

        return state with
        {
            Snake     = newSnake,
            Food      = newFood,
            Score     = newScore,
            HighScore = Math.Max(newScore, state.HighScore)
        };
    }

    private static Coordenada CalculateNextHead(Coordenada head, Direction direction) =>
        direction switch
        {
            Direction.Up    => head with { Y = head.Y - 1 },
            Direction.Down  => head with { Y = head.Y + 1 },
            Direction.Left  => head with { X = head.X - 1 },
            Direction.Right => head with { X = head.X + 1 },
            _               => head
        };

    private static bool IsCollision(Coordenada head, ImmutableList<Coordenada> snake, int w, int h) =>
        head.X < 0 || head.X >= w ||
        head.Y < 0 || head.Y >= h ||
        snake.Contains(head);

    private static bool IsOppositeDirection(Direction current, Direction next) =>
        (current, next) switch
        {
            (Direction.Up,    Direction.Down)  => true,
            (Direction.Down,  Direction.Up)    => true,
            (Direction.Left,  Direction.Right) => true,
            (Direction.Right, Direction.Left)  => true,
            _                                  => false
        };

    private static Coordenada SpawnFood(ImmutableList<Coordenada> snake, int w, int h)
    {
        var rng = new Random();
        Coordenada food;
        do
        {
            food = new Coordenada(rng.Next(w), rng.Next(h));
        }
        while (snake.Contains(food));

        return food;
    }
}
