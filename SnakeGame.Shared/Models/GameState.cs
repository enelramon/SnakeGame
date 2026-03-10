using System.Collections.Immutable;

namespace SnakeGame.Shared.Models;

/// <summary>
/// Representa el estado completo del juego en un instante de tiempo.
/// Nunca se modifica directamente; las funciones de lógica devuelven un estado nuevo.
/// </summary>
public record GameState(
    ImmutableList<Coordenada> Snake,
    Coordenada Food,
    Direction CurrentDirection,
    // NextDirection evita que el jugador invierta la dirección en el mismo frame.
    Direction NextDirection,
    int Score,
    int HighScore,
    GameStatus Status,
    int GridWidth,
    int GridHeight
);
