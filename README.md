# CoreKit

CoreKit is a modular C# utility library designed to streamline Unity and .NET development. It provides essential components such as dependency injection, UI management, and utility helpers to accelerate your project setup and maintain clean architecture.

## Features

- **Dependency Injection**: Lightweight DI container for flexible and testable code.
- **UI System**: Extensible UI root and manager for scalable Unity UI workflows.
- **Utilities**: Common helpers and patterns for Unity and C# projects.
- **App**: Core application interfaces and base classes for app lifecycle management, such as focus, pause, update, and back button handling.
- **Saving**: Interfaces and services for game data saving, serialization, migration, and storage (e.g., PlayerPrefs, JSON files).
- **Scenes**: Scene loading interfaces and implementations for managing Unity scene transitions.
- **StateMachine**: Simple state machine framework for managing game or app states with extensible state logic.

## Getting Started

1. **Clone the repository:**
   git clone https://github.com/YevheniiKost/MainPortfolioProject.git
2. **Import into Unity or reference in your .NET project.**

3. **Example Usage:**

   ```csharp
   // Setting up the DI container
   var container = new Container();
   container.Bind<IMyService, MyService>().AsSingleton();
   var myService = container.Resolve<IMyService>();

   // Using the UI system in Unity
   public class UIRoot : MonoBehaviour
   {
       public static UIRoot Instance { get; private set; }
       public UIManager UIManager { get; private set; }
       // ...
   }