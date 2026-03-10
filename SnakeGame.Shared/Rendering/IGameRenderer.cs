using SnakeGame.Shared.Models;

namespace SnakeGame.Shared.Rendering;

/// <summary>
/// Contrato que deben cumplir todos los renderizadores del juego.
/// Permite desacoplar la lógica de presentación de la tecnología de UI.
/// </summary>
public interface IGameRenderer
{
    void Render(GameState state);
}
