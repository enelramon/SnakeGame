using SnakeGame.Shared.Engine;
using SnakeGame.Shared.Models;
using SnakeGame.UI;
using Point = System.Drawing.Point;

namespace SnakeGame.UI;

/// <summary>
/// Ventana principal del juego. Solo coordina el input y el rendering.
/// El timer y el estado del juego viven en GameEngine (SnakeGame.Shared).
/// Flujo: KeyDown → GameEngine → OnChange → Invalidate → Renderer → Pantalla.
/// </summary>
public partial class SnakeForm : Form
{
    private const int GridWidth = 32;
    private const int GridHeight = 24;
    private const int HudHeight = 30;

    private readonly GameEngine _engine = new(GridWidth, GridHeight, tickIntervalMs: 230);

    public SnakeForm()
    {
        InitializeComponent();
        _gamePanel.Paint += (_, e) => Renderer.Render(e.Graphics, _engine.State);
        _engine.OnChange += () => _gamePanel.Invalidate();

    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Up or Keys.W: _engine.ChangeDirection(Direction.Up); break;
            case Keys.Down or Keys.S: _engine.ChangeDirection(Direction.Down); break;
            case Keys.Left or Keys.A: _engine.ChangeDirection(Direction.Left); break;
            case Keys.Right or Keys.D: _engine.ChangeDirection(Direction.Right); break;
            case Keys.Space: _engine.HandleSpace(); break;
            case Keys.R: _engine.RestartGame(); break;
            case Keys.Escape: _engine.TogglePause(); break;
        }
    }
}
