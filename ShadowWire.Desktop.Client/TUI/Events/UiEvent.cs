namespace ShadowWire.Desktop.Client.TUI.Events;

public abstract record UiEvent();

// Core UI
public record Redraw : UiEvent;
public record Quit : UiEvent;
public record ChangeScene(Scenes.IScene NewScene) : UiEvent;

// Input
public record KeyEvent(ConsoleKeyInfo KeyInfo) : UiEvent;
public record PasteEvent(string Text) : UiEvent;

// Messages
public record MessageReceived(string Text) : UiEvent;
