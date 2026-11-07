using ShadowWire.Desktop.Client.TUI.Components.Core;
using ShadowWire.Desktop.Client.TUI.Events;
using ShadowWire.Desktop.Client.TUI.Themes;
using System.Drawing;

namespace ShadowWire.Desktop.Client.TUI.Components.Primitives;

/// <summary>
/// A vertical list of components with one selected item, scrolled to keep it visible.
/// </summary>
/// <remarks>
/// Items are drawn whole or not at all. A visible item that becomes dirty marks the list dirty, <br/>
/// but item heights are only read when drawing, so call <see cref="BaseComponent.Invalidate"/>  <br/>
/// on the list after changing a child's height.
/// </remarks>
public class SelectableList<TComp> : BaseComponent, IEventConsumer
    where TComp : BaseComponent, ISelectable
{
    private readonly List<TComp> _items = new();

    public IReadOnlyList<TComp> Items => _items;

    /// <summary>
    /// Index of the first visible item.
    /// </summary>
    public int TopIndex => _top;
    private int _top = 0;

    /// <summary>
    /// Index of the selected item, (or <c>-1</c> when the list is empty).
    /// </summary>
    public int SelectedIndex => _selected;
    private int _selected = -1;

    public TComp? SelectedItem => (_selected >= 0) ? _items[_selected] : null;

    /// <summary>
    /// Raised with the new index whenever the selection changes.
    /// </summary>
    public event Action<int>? SelectionChanged;

    /// <summary>
    /// Raised when Enter is pressed on the selected item.
    /// </summary>
    public event Action<TComp>? SelectionTriggered;

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


    public SelectableList(Point origin, Size size) : base(origin, size)
    {
        Style = new Style(default(TrueColor), default(TrueColor)); // Avoid colour bleeding by default.
    }


    public void Add(TComp item)
    {
        item.IsSelected = false;
        _items.Add(item);

        if (_selected == -1)
            Select(0); // First item becomes the selection.

        Invalidate();
    }

    public void Clear()
    {
        _items.Clear();
        _top = 0;
        _selected = -1;
        Invalidate();
    }

    /// <summary>
    /// Selects the item at <paramref name="index"/> (clamped) and scrolls it into view.
    /// </summary>
    public void Select(int index)
    {
        if (_items.Count == 0)
            return; // Early exit: No items to select.

        int clamped = Math.Clamp(index, 0, (_items.Count - 1));
        if (clamped == _selected)
            return; // Early exit: Already selected.

        if (_selected >= 0)
            _items[_selected].IsSelected = false; // Unselect previous

        _selected = clamped;
        _items[_selected].IsSelected = true;

        EnsureSelectedVisible();
        Invalidate(); // Redraw
        SelectionChanged?.Invoke(_selected);
    }


    public bool Handle(UiEvent e)
    {
        if (e is not KeyEvent { KeyInfo: var k })
            return false;

        // Trigger item action
        if (k.Key == ConsoleKey.Enter)
        {
            if (SelectedItem is not { } item)
                return false;

            SelectionTriggered?.Invoke(item);
            return true;
        }

        // Select item
        int page = Math.Max(1, VisibleCount());

        int? targetIndex = k.Key switch
        {
            ConsoleKey.UpArrow   => _selected - 1,
            ConsoleKey.DownArrow => _selected + 1,
            ConsoleKey.PageUp    => _selected - page,
            ConsoleKey.PageDown  => _selected + page,
            ConsoleKey.Home      => 0,
            ConsoleKey.End       => int.MaxValue,
            _                    => null,
        };

        if (targetIndex is not { } index)
            return false;

        Select(index); // Clamps, so overshooting at either end is fine.
        return true;
    }

    public override void Resize(Size newSize)
    {
        base.Resize(newSize);
        EnsureSelectedVisible();
    }


    /// <summary>
    /// Number of items from <paramref name="startIndex"/> that fit fully in the viewport.
    /// </summary>
    /// <remarks>
    /// Always at least one (if any exist), so an oversized item cannot hide the selection.
    /// </remarks>
    private int CountFitting(int startIndex)
    {
        int remaining = Height;
        int count = 0;

        for (int i = startIndex; i < _items.Count; i++)
        {
            int height = _items[i].Height;

            // The first item is always taken, so an oversized item can't stall scrolling.
            if (count > 0 && height > remaining)
                break;

            remaining -= height;
            count++;
        }

        return count;
    }

    private int VisibleCount()
        => CountFitting(_top);

    /// <summary>
    /// Scrolls the minimum amount needed so the selected item is fully drawn.
    /// </summary>
    private void EnsureSelectedVisible()
    {
        if (_selected < 0)
            return; // Early exit: No item selected.

        if (_selected < _top)
        {
            _top = _selected;
            return; // Item already visible.
        }

        // Scroll down just enough to show the selection.
        //     (Terminates because CountFitting is always >= 0).
        while (_selected >= _top + CountFitting(_top))
            _top++;
    }


    protected override void Draw(Surface s)
    {
        s.Fill(' ', Style);

        int y = 0;
        int count = VisibleCount();
        for (int i = _top; i < _top + count; i++)
        {
            var item = _items[i];
            item.Invalidate();
            item.Render(s, new Point(0, y));
            y += item.Height;
        }
    }
}
