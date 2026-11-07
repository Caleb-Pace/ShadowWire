namespace ShadowWire.Desktop.Client.TUI.Components.Core;

/// <summary>
/// Implemented by components that can be marked as the current selection of a container (e.g. a list row).
/// </summary>
/// <remarks>
/// The container only flips <see cref="IsSelected"/>; the component decides how that looks      <br/>
/// (swap to a selected style, draw a marker, etc.) and must invalidate itself when it changes.  <br/>
/// This is separate from <see cref="IFocusable"/>: focus is "does this receive keyboard input", <br/>
/// selection is "which item of a collection is chosen".
/// </remarks>
public interface ISelectable
{
    bool IsSelected { get; set; }
}
