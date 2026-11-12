using ShadowWire.Desktop.Client.TUI.Components.Composites;
using ShadowWire.Desktop.Client.TUI.Components.Core;
using ShadowWire.Desktop.Client.TUI.Components.Primitives;
using ShadowWire.Desktop.Client.TUI.Events;
using ShadowWire.Desktop.Client.TUI.Themes;
using ShadowWire.Shared.Users;
using System.Drawing;

namespace ShadowWire.Desktop.Client.TUI.Scenes;

public sealed class Home : IScene
{
    private readonly EventQueue _events;
    private readonly Theme _theme;
    private readonly ContactManager _contacts;

    private Surface _rootSurface;
    private BaseComponent[] _componentsRef;

    // TODO: Make dictionary, <Fingerprint, ContactCard>
    private List<ContactCard> _contactCards = new();
    private SelectableList<ContactCard> _list; // TODO: Select Contact card composite component.
    private TextInput _input;
    private Border _border;


    public Home(EventQueue events, Theme theme, ContactManager contacts)
    {
        _events   = events;
        _theme    = theme;
        _contacts = contacts;

        int screenHeight = Console.WindowHeight;
        int screenWidth  = Console.WindowWidth;

        _rootSurface = new Surface(screenWidth, screenHeight);

        // Flush style
        _rootSurface.Fill(' ', _theme.Base);
        _rootSurface.Flush();


        // Components
        _list = new SelectableList<ContactCard>(new Point(1, 1), new Size(screenWidth - 2, screenHeight - 3))
        {
            Style = _theme.Base
        };
        PopulateContactList();

        _input = new TextInput(new Point(1, (screenHeight - 2)), screenWidth - 2)
        {
            Style     = _theme.Input,
            MaxLength = screenWidth - 2,
        };

        _border = new Border(_rootSurface)
        {
            Line = BorderLine.Rounded
        };


        _componentsRef = [ _list, _input, _border ];
    }

    private void PopulateContactList()
    {
        bool IsOddIndex()
            => (_contactCards.Count & 1) != 0;

        foreach (var contact in _contacts.GetAll())
        {
            var contactCard = new ContactCard(_theme, contact, _list.Width)
            {
                Style = IsOddIndex() ? _theme.Home.Row : _theme.Home.RowAlt
            };

            _contactCards.Add(contactCard);
            _list.Add(contactCard);
        }
    }


    // TODO: Update with new comps
    public bool Handle(UiEvent e)
    {
        bool consumed = false;

        if (_input.IsFocused)
            consumed |= _input.Handle(e);

        consumed |= _list.Handle(e);


        if (consumed)
            return true;

        if ((e is KeyEvent k) && (k.KeyInfo.Key == ConsoleKey.Enter))
        {
            if (!_input.TryTakeText(out string text))
                return false;

            _input.IsFocused = false;
            _rootSurface.HideCursor();

            _rootSurface.Write(1, 0, $"\"{text.Replace("\n", "\\n")}\"");
        }

        return false;
    }

    public void Render()
    {
        // Must be done before any render calls
        //     (Render calls change dirty state)
        bool anyDirty = _componentsRef.Any(comp => comp.IsDirty);


        // Redraw
        _list.Render(_rootSurface);
        _input.Render(_rootSurface);
        _border.Render(_rootSurface);


        // Flush to Console
        if (anyDirty)
            _rootSurface.Flush();
    }
}
