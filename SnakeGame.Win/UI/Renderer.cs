using System.Collections.Immutable;
using SnakeGame.Shared.Models;

namespace SnakeGame.UI;

/// <summary>Renderizador del juego. Todas sus funciones son estáticas y no modifican el estado.</summary>
public static class Renderer
{
    public const int CellSize = 22;

    private static readonly Color ColorFondo      = Color.FromArgb(30,  30,  30);
    private static readonly Color ColorGrilla     = Color.FromArgb(45,  45,  45);
    private static readonly Color ColorCabeza     = Color.FromArgb(0,   230, 80);
    private static readonly Color ColorCuerpo     = Color.FromArgb(34,  139, 34);
    private static readonly Color ColorComida     = Color.FromArgb(220, 50,  50);
    private static readonly Color ColorOverlay    = Color.FromArgb(170, 0,   0,  0);
    private static readonly Color ColorTitulo     = Color.FromArgb(0,   230, 80);
    private static readonly Color ColorSubtitulo  = Color.White;
    private static readonly Color ColorHUD        = Color.FromArgb(200, 230, 230, 230);

    public static void Render(Graphics g, GameState state)
    {
        RenderBackground(g, state);
        RenderFood(g, state.Food);
        RenderSnake(g, state.Snake);
        RenderHUD(g, state);
        RenderOverlay(g, state);
    }

    private static void RenderBackground(Graphics g, GameState state)
    {
        g.Clear(ColorFondo);

        using var pen = new Pen(ColorGrilla, 1);
        for (int x = 0; x <= state.GridWidth; x++)
            g.DrawLine(pen, x * CellSize, 0, x * CellSize, state.GridHeight * CellSize);
        for (int y = 0; y <= state.GridHeight; y++)
            g.DrawLine(pen, 0, y * CellSize, state.GridWidth * CellSize, y * CellSize);
    }

    private static void RenderFood(Graphics g, Coordenada food)
    {
        var rect = CellToRect(food);
        rect.Inflate(-3, -3);
        using var brush = new SolidBrush(ColorComida);
        g.FillEllipse(brush, rect);

        var highlight = new Rectangle(rect.X + 3, rect.Y + 2, rect.Width / 3, rect.Height / 3);
        using var highlightBrush = new SolidBrush(Color.FromArgb(120, 255, 180, 180));
        g.FillEllipse(highlightBrush, highlight);
    }

    private static void RenderSnake(Graphics g, ImmutableList<Coordenada> snake)
    {
        if (snake.Count == 0) return;

        // Dibuja el cuerpo de atrás hacia adelante; los segmentos más lejanos se atenúan.
        for (int i = snake.Count - 1; i >= 1; i--)
        {
            var alpha = Math.Clamp(255 - i * 4, 80, 255);
            var color = Color.FromArgb(alpha, ColorCuerpo);
            RenderSegment(g, snake[i], color, isHead: false);
        }

        RenderSegment(g, snake[0], ColorCabeza, isHead: true);
    }

    private static void RenderSegment(Graphics g, Coordenada coordenada, Color color, bool isHead)
    {
        var rect = CellToRect(coordenada);
        rect.Inflate(-2, -2);

        using var brush = new SolidBrush(color);
        g.FillRectangle(brush, rect);

        if (isHead)
            DrawEyes(g, rect);
    }

    private static void DrawEyes(Graphics g, Rectangle headRect)
    {
        int eyeSize = Math.Max(2, headRect.Width / 6);
        int ey = headRect.Y + headRect.Height / 4;

        using var eyeBrush = new SolidBrush(Color.Black);
        g.FillEllipse(eyeBrush,
            new Rectangle(headRect.X + headRect.Width / 4,     ey, eyeSize, eyeSize));
        g.FillEllipse(eyeBrush,
            new Rectangle(headRect.X + headRect.Width * 3 / 5, ey, eyeSize, eyeSize));
    }

    private static void RenderHUD(Graphics g, GameState state)
    {
        int panelW = state.GridWidth * CellSize;
        int panelH = state.GridHeight * CellSize;
        int hudH   = 30;

        using var hudBrush = new SolidBrush(Color.FromArgb(200, 20, 20, 20));
        g.FillRectangle(hudBrush, 0, panelH, panelW, hudH);

        using var font  = new Font("Consolas", 9, FontStyle.Bold);
        using var brush = new SolidBrush(ColorHUD);

        var left   = $"  🐍 Longitud: {state.Snake.Count}   Score: {state.Score}";
        var right  = $"HighScore: {state.HighScore}  ";
        var center = "← ↑ → ↓  |  ESC: Pausa  |  R: Reiniciar";

        g.DrawString(left,   font, brush, 5,               panelH + 8);
        g.DrawString(center, font, brush, panelW / 2 - 130, panelH + 8);
        g.DrawString(right,  font, brush, panelW - 130,     panelH + 8);
    }

    /// <summary>Muestra el overlay de pantalla (inicio, pausa, game over). No dibuja nada si el juego está activo.</summary>
    private static void RenderOverlay(Graphics g, GameState state)
    {
        if (state.Status == GameStatus.Playing) return;

        var (title, subtitle, hint) = state.Status switch
        {
            GameStatus.NotStarted => (
                "SNAKE",
                "Programación Funcional en C#",
                "[ ESPACIO ] para iniciar"
            ),
            GameStatus.GameOver => (
                "GAME OVER",
                $"Puntaje final: {state.Score}",
                "[ ESPACIO ] reiniciar  |  [ R ] volver al inicio"
            ),
            GameStatus.Paused => (
                "PAUSADO",
                $"Score: {state.Score}",
                "[ ESPACIO ] continuar"
            ),
            _ => ("", "", "")
        };

        DrawOverlay(g, state, title, subtitle, hint);
    }

    private static void DrawOverlay(
        Graphics g, GameState state,
        string title, string subtitle, string hint)
    {
        int w  = state.GridWidth  * CellSize;
        int h  = state.GridHeight * CellSize;
        int cx = w / 2;
        int cy = h / 2;

        using var overlayBrush = new SolidBrush(ColorOverlay);
        g.FillRectangle(overlayBrush, 0, 0, w, h);

        using var titleFont = new Font("Consolas", 36, FontStyle.Bold);
        using var subFont   = new Font("Consolas", 13, FontStyle.Regular);
        using var hintFont  = new Font("Consolas", 11, FontStyle.Italic);
        using var titleBrush = new SolidBrush(ColorTitulo);
        using var subBrush   = new SolidBrush(ColorSubtitulo);
        using var hintBrush  = new SolidBrush(Color.FromArgb(180, 200, 200, 200));

        DrawCentered(g, title,    titleFont, titleBrush, cx, cy - 55);
        DrawCentered(g, subtitle, subFont,   subBrush,   cx, cy + 5);
        DrawCentered(g, hint,     hintFont,  hintBrush,  cx, cy + 38);
    }

    private static Rectangle CellToRect(Coordenada p) =>
        new(p.X * CellSize, p.Y * CellSize, CellSize, CellSize);

    private static void DrawCentered(
        Graphics g, string text, Font font, Brush brush, int cx, int cy)
    {
        var size = g.MeasureString(text, font);
        g.DrawString(text, font, brush, cx - size.Width / 2, cy - size.Height / 2);
    }
}
