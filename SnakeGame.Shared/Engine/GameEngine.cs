using SnakeGame.Shared.Logic;
using SnakeGame.Shared.Models;

namespace SnakeGame.Shared.Engine;

/// <summary>
/// Núcleo del juego: estado, lógica y timer unificados para todas las plataformas.
/// Usa PeriodicTimer (System.Threading), disponible en net10.0 y net10.0-windows.
/// Cada plataforma solo suscribe OnChange y llama StartTimer() cuando la partida comienza.
/// </summary>
public sealed class GameEngine : IAsyncDisposable
{
    private GameState _state;
    private PeriodicTimer? _timer;
    private Task _tickLoop = Task.CompletedTask;

    public int TickIntervalMs { get; }

    public GameEngine(int gridWidth = 32, int gridHeight = 24, int tickIntervalMs = 250)
    {
        _state        = GameLogic.CreateInitialState(gridWidth, gridHeight);
        TickIntervalMs = tickIntervalMs;
    }

    public GameState State => _state;

    public event Action? OnChange;

    // ── Acciones del juego ─────────────────────────────────

    public void StartGame()
    {
        _state = GameLogic.StartGame(_state);
        EnsureTimerRunning();
        Notify();
    }

    public void TogglePause()
    {
        _state = GameLogic.TogglePause(_state);
        Notify();
    }

    public void RestartGame()
    {
        _state = GameLogic.RestartGame(_state);
        EnsureTimerRunning();
        Notify();
    }

    public void ChangeDirection(Direction direction)
    {
        _state = GameLogic.ChangeDirection(_state, direction);
    }

    public void HandleSpace()
    {
        _state = _state.Status switch
        {
            GameStatus.NotStarted => GameLogic.StartGame(_state),
            GameStatus.GameOver   => GameLogic.RestartGame(_state),
            _                     => GameLogic.TogglePause(_state)
        };
        EnsureTimerRunning();
        Notify();
    }

    // ── Timer ──────────────────────────────────────────────

    private void EnsureTimerRunning()
    {
        if (_timer is not null) return;
        _timer    = new PeriodicTimer(TimeSpan.FromMilliseconds(TickIntervalMs));
        _tickLoop = RunLoopAsync();
    }

    private async Task RunLoopAsync()
    {
        while (_timer is not null && await _timer.WaitForNextTickAsync())
        {
            if (_state.Status == GameStatus.Playing)
            {
                _state = GameLogic.Tick(_state);
                Notify();
            }

            if (_state.Status == GameStatus.GameOver)
            {
                StopTimer();
                Notify();
                break;
            }
        }
    }

    /// <summary>Detiene el timer de forma síncrona. Seguro llamarlo desde cualquier hilo.</summary>
    public void StopTimer()
    {
        _timer?.Dispose();
        _timer = null;
    }

    private void Notify() => OnChange?.Invoke();

    public async ValueTask DisposeAsync()
    {
        StopTimer();
        await _tickLoop;
    }
}
