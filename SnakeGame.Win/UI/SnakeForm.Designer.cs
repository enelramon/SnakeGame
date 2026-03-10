using Point = System.Drawing.Point;

namespace SnakeGame.UI;

partial class SnakeForm
{
    private GamePanel _gamePanel;

    private void InitializeComponent()
    {
        _gamePanel = new GamePanel
        {
            Location  = new Point(0, 0),
            Size      = new Size(GridWidth * Renderer.CellSize, GridHeight * Renderer.CellSize + HudHeight),
            BackColor = Color.FromArgb(30, 30, 30)
        };
        _gamePanel.Paint += (_, e) => Renderer.Render(e.Graphics, _engine.State);

        SuspendLayout();
        Text            = "Snake — Programación Funcional en C#  |  Enel - .NET";
        ClientSize      = new Size(GridWidth * Renderer.CellSize, GridHeight * Renderer.CellSize + HudHeight);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = Color.FromArgb(20, 20, 20);
        KeyPreview      = true;
        KeyDown        += OnKeyDown;
        Controls.Add(_gamePanel);
        ResumeLayout(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _engine.StopTimer();

        base.Dispose(disposing);
    }
}
