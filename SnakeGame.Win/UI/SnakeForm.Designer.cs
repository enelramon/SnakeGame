using Point = System.Drawing.Point;

namespace SnakeGame.UI;

partial class SnakeForm
{
    private GamePanel _gamePanel;

    private void InitializeComponent()
    {
        _gamePanel = new GamePanel();
        SuspendLayout();
        // 
        // _gamePanel
        // 
        _gamePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _gamePanel.Location = new Point(5, 4);
        _gamePanel.Name = "_gamePanel";
        _gamePanel.Size = new Size(711, 553);
        _gamePanel.TabIndex = 0;
        // 
        // SnakeForm
        // 
        BackColor = Color.FromArgb(20, 20, 20);
        ClientSize = new Size(719, 562);
        Controls.Add(_gamePanel);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        KeyPreview = true;
        MaximizeBox = false;
        Name = "SnakeForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Snake — Programación Funcional en C#  |  Enel - .NET";
        KeyDown += OnKeyDown;
        ResumeLayout(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _engine.StopTimer();

        base.Dispose(disposing);
    }
}
