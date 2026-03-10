# 🐍 Snake Game — Programación Funcional en C#

Proyecto educativo demostrando conceptos de **programación funcional** aplicados
en un lenguaje orientado a objetos moderno (C# 12 / .NET 8).

---

## 🎮 Controles

| Tecla | Acción |
|-------|--------|
| `↑ ↓ ← →` o `WASD` | Mover la serpiente |
| `ESPACIO` | Iniciar / Pausar / Reiniciar |
| `ESC` | Pausar / Reanudar |
| `R` | Reiniciar desde cualquier estado |

---

## 🏗️ Arquitectura del Proyecto

```
SnakeGame/
├── Models/
│   └── GameState.cs      ← Records inmutables (datos)
├── Logic/
│   └── GameLogic.cs      ← Funciones puras (lógica)
├── UI/
│   ├── GameForm.cs       ← Orquestador de eventos
│   ├── GamePanel.cs      ← Panel con double-buffering
│   └── Renderer.cs       ← Funciones de dibujo
└── Program.cs            ← Punto de entrada
```

**Flujo de datos (unidireccional):**
```
  Evento (tecla/timer)
       ↓
  GameLogic.Función(estadoActual)   ← función pura
       ↓
  estadoNuevo (record inmutable)
       ↓
  Renderer.Render(graphics, estado) ← solo dibuja, no modifica
       ↓
  Pantalla
```

---

## 📚 Conceptos de Programación Funcional Ilustrados

### 1. 🔒 Inmutabilidad (Models/GameState.cs)

Los datos no se modifican; se crean versiones nuevas.

```csharp
// MAL: mutación directa
state.Score += 10;

// BIEN: nueva versión del estado
var nuevoEstado = state with { Score = state.Score + 10 };
```

C# `record` + `with` = inmutabilidad ergonómica.

---

### 2. 🧮 Funciones Puras (Logic/GameLogic.cs)

Una función pura:
- Siempre devuelve el mismo resultado para la misma entrada
- No modifica nada fuera de su scope (sin efectos secundarios)

```csharp
// Función pura: solo depende de sus parámetros
public static GameState ChangeDirection(GameState state, Direction dir) =>
    IsOppositeDirection(state.CurrentDirection, dir)
        ? state
        : state with { NextDirection = dir };
```

**¿Por qué importa?** Las funciones puras son:
- Triviales de testear unitariamente
- Fáciles de razonar (sin sorpresas)
- Componibles entre sí

---

### 3. 🔀 Switch Expressions (Logic/GameLogic.cs, UI/GameForm.cs)

Tratan los casos como expresiones de valor, no instrucciones con efectos:

```csharp
// Dirección: switch expression exhaustivo
private static Point CalculateNextHead(Point head, Direction dir) =>
    dir switch
    {
        Direction.Up    => head with { Y = head.Y - 1 },
        Direction.Down  => head with { Y = head.Y + 1 },
        Direction.Left  => head with { X = head.X - 1 },
        Direction.Right => head with { X = head.X + 1 },
        _               => head
    };
```

```csharp
// Teclado: cada tecla es una transformación de estado
_state = e.KeyCode switch
{
    Keys.Up    => GameLogic.ChangeDirection(_state, Direction.Up),
    Keys.Space => ManejarEspacio(_state),
    Keys.R     => GameLogic.RestartGame(_state),
    _          => _state
};
```

---

### 4. 📦 ImmutableList<T> (System.Collections.Immutable)

La serpiente es una lista inmutable de puntos. Al "modificarla", obtenemos
una lista nueva sin alterar la original:

```csharp
// Insert devuelve una nueva lista con el elemento agregado
var snakeMasLarga = snake.Insert(0, nuevaCabeza);

// RemoveAt devuelve una nueva lista sin el último elemento
var snakeMovida = snake.Insert(0, cabeza).RemoveAt(snake.Count - 1);
```

---

### 5. 🗂️ Separación de Responsabilidades (SRP)

| Archivo | Responsabilidad | ¿Tiene estado? |
|---------|----------------|----------------|
| `GameState.cs` | Definir la estructura de datos | No (solo tipos) |
| `GameLogic.cs` | Calcular transiciones de estado | No (funciones puras) |
| `Renderer.cs` | Dibujar el estado en pantalla | No (solo dibuja) |
| `GameForm.cs` | Conectar eventos con lógica | Sí (`_state`) |

---

### 6. ✨ Expression-Bodied Members

Para métodos de una sola expresión, C# permite la sintaxis `=>`:

```csharp
// Forma clásica
public static GameState StartGame(GameState state)
{
    return state with { Status = GameStatus.Playing };
}

// Expression-bodied: más conciso y legible
public static GameState StartGame(GameState state) =>
    state with { Status = GameStatus.Playing };
```

---

## 🧪 Ideas para Ejercicios de los Estudiantes

1. **Niveles de velocidad**: reducir `TickInterval` al crecer la serpiente.
2. **Múltiples comidas**: cambiar `Food: Point` por `Foods: ImmutableList<Point>`.
3. **Paredes opcionales**: modo "atravesar paredes" modificando solo `IsCollision`.
4. **Historial de puntajes**: agregar `ImmutableList<int> ScoreHistory` al GameState.
5. **Tests unitarios**: testear `GameLogic` sin formularios ni UI.

---

## ⚙️ Requisitos y Compilación

- .NET 8 SDK (Windows)
- Visual Studio 2022 o VS Code con C# DevKit

```bash
dotnet restore
dotnet build
dotnet run
```

---

## 🔑 Lección Principal

> La programación funcional **no requiere un lenguaje funcional puro**.
> C# moderno permite aplicar sus principios (inmutabilidad, funciones puras,
> composición) dentro de un contexto orientado a objetos, obteniendo lo mejor
> de ambos mundos.
