using ShadowWire.Desktop.Client.TUI.Events;
using System.Drawing;

namespace ShadowWire.Desktop.Client.TUI.Components;

/// <summary>
/// A vertically stacked list of components, scrolled one item at a time.
/// </summary>
/// <remarks>
/// Only items that fully fit are drawn. Changes to a child are not tracked, call <see cref="BaseComponent.Invalidate"/> on the list.
/// </remarks>
public class List<TComp>(Point origin, Size size) : BaseComponent(origin, size), IEventConsumer
    where TComp : BaseComponent
{
    private readonly System.Collections.Generic.List<TComp> _items = new();

    public IReadOnlyList<TComp> Items => _items;

    /// <summary>
    /// Index of the first visible item.
    /// </summary>
    public int TopIndex => _top;
    private int _top = 0;

    public override bool IsDirty
    {
        get
        {
            if (base.IsDirty)
                return true;

            // Check visible children
            int end = _top + VisibleCount();
            for (int i = _top; i < end; i++)
            {
                if (_items[i].IsDirty)
                    return true;
            }

            return false;
        }
    }


    public void Add(TComp item)
    {
        _items.Add(item);
        Invalidate();
    }

    public void Clear()
    {
        _items.Clear();
        _top = 0;
        Invalidate();
    }

    /// <summary>
    /// Scrolls so the item at <paramref name="index"/> is the first visible one (clamped).
    /// </summary>
    public void ScrollTo(int index)
    {
        int clamped = Math.Clamp(index, 0, MaxTop());
        if (clamped == _top)
            return;

        _top = clamped;
        Invalidate();
    }


    public bool Handle(UiEvent e)
    {
        if (e is not KeyEvent { KeyInfo: var k })
            return false;

        switch (k.Key)
        {
            case ConsoleKey.UpArrow: ScrollTo(_top - 1); return true;
            case ConsoleKey.DownArrow: ScrollTo(_top + 1); return true;
            case ConsoleKey.PageUp: ScrollTo(_top - Math.Max(1, VisibleCount())); return true;
            case ConsoleKey.PageDown: ScrollTo(_top + Math.Max(1, VisibleCount())); return true;
            case ConsoleKey.Home: ScrollTo(0); return true;
            case ConsoleKey.End: ScrollTo(int.MaxValue); return true;
        }

        return false;
    }

    public override void Resize(Size newSize)
    {
        base.Resize(newSize);
        _top = Math.Clamp(_top, 0, MaxTop());
    }


    /// <summary>
    /// Number of items from <paramref name="start"/> that fit fully in the viewport.
    /// </summary>
    /// <remarks>
    /// Always at least one (if any exist), so an oversized item cannot block scrolling.
    /// </remarks>
    private int CountFitting(int start)
    {
        int used = 0, count = 0;
        for (int i = start; i < _items.Count; i++)
        {
            int h = _items[i].Height;
            if (used + h > Height && count > 0)
                break;
            used += h;
            count++;
        }
        return count;
    }

    private int VisibleCount() => CountFitting(_top);

    /// <summary>
    /// The largest top index that still fills the viewport.
    /// </summary>
    private int MaxTop()
    {
        if (_items.Count == 0)
            return 0;

        int used = 0, top = _items.Count - 1;
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            if (used + _items[i].Height > Height && i != _items.Count - 1)
                break;
            used += _items[i].Height;
            top = i;
        }
        return top;
    }


    protected override void Draw()
    {
        // Clear the viewport.
        string blank = new(' ', Math.Max(0, Width));
        for (int row = 0; row < Height; row++)
        {
            Console.SetCursorPosition(X, Y + row);
            Console.Write(blank);
        }

        int y = Y;
        int count = VisibleCount();
        for (int i = _top; i < _top + count; i++)
        {
            var item = _items[i];
            item.Origin = new Point(X, y);
            item.Invalidate(); // Area was cleared.
            item.Render();
            y += item.Height;
        }
    }
}