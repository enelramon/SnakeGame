namespace SnakeGame.UI;

/// <summary>Panel con DoubleBuffering activado para evitar parpadeo.</summary>
public sealed class GamePanel : Panel
{
    public GamePanel() => DoubleBuffered = true;
}
