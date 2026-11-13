using ShadowWire.Desktop.Client.TUI.Components.Core;
using ShadowWire.Desktop.Client.TUI.Components.Primitives;
using ShadowWire.Desktop.Client.TUI.Themes;
using ShadowWire.Shared.Users;
using System.Drawing;
using System.Runtime.InteropServices.JavaScript;

namespace ShadowWire.Desktop.Client.TUI.Components.Composites;

// TODO: Fix colour background issue (choosing default over, use background)
// TODO: Missing "you" prefix
// TODO: Missing timestamp
// TODO: Update fingerprint handling
public class ContactCard : BaseComponent, ISelectable
{
    private readonly Theme _theme;

    public Fingerprint Fingerprint => _contact.Fingerprint;
    private readonly Contact _contact;

    private Text _nickname;
    private Text _fingerprint;

    public string LatestMessage
    {
        get => _latestMessage.Content;
        set
        {
            if (string.Equals(value, _latestMessage.Content))
                return;

            _latestMessage.Content = value;
            _latestMessage.Invalidate();
        }
    }
    private Text _latestMessage;

    public int UnreadMessageCount
    {
        get;
        set
        {
            if (value < 0)
                return;
            if (value == field)
                return;

            field = value;

            if (field > 0)
            {
                _unreadMessageCountText.Content = (field < 100) ? field.ToString() : "99+";
                _unreadMessageCountText.Invalidate();
            }
            else
            {
                // Note: with a count of zero the component is not rendered.
                _unreadMessageCountText.Content = "";
                Invalidate();
            }
        }
    } = 0;
    private Text _unreadMessageCountText;

    public override bool IsDirty
    {
        get
        {
            return base.IsDirty
                || _nickname.IsDirty
                || _fingerprint.IsDirty
                || _unreadMessageCountText.IsDirty;
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
                return;

            _isSelected = value;
            base.Invalidate();
        }
    }
    private bool _isSelected = false;

    public Style SelectedStyle
    {
        get => _selectedStyle;
        set
        {
            if (_selectedStyle == value)
                return;

            _selectedStyle = value;
            base.Invalidate();
        }
    }
    private Style _selectedStyle;


    public ContactCard(Theme theme, Contact contact, int width) : base(Point.Empty, new Size(width, 3))
    {
        _theme   = theme;
        _contact = contact;

        _selectedStyle = _theme.Home.RowSelected;

        // Components
        _nickname = new Text(new Point(1, 0), new Size(32, 1))
        {
            Content = contact.Nickname,
            OverflowType = Overflow.Ellipsis,
        };

        var fingerprintBytes = contact.Fingerprint.Span;
        var fingerprintString = $"{Convert.ToHexStringLower(fingerprintBytes[..4])}…{Convert.ToHexStringLower(fingerprintBytes[^4..])}";
        _fingerprint = new Text(new Point(40, 0), new Size(32, 1))
        {
            Content = fingerprintString,
            OverflowType = Overflow.Ellipsis,
        };

        _unreadMessageCountText = new Text(new Point(64, 0), new Size(5, 1))
        {
            OverflowType = Overflow.Clip,
            Style = _theme.Home.UnreadBadge,
        };
        _latestMessage = new Text(new Point(0, 1), new Size(5, 1))
        {
            OverflowType = Overflow.Ellipsis,
        };
    }


    private void UpdateStyles()
    {
        _nickname.Style      = (UnreadMessageCount > 0) ? _theme.Home.ContactNameUnread    : _theme.Home.ContactName;
        _latestMessage.Style = (UnreadMessageCount > 0) ? _theme.Home.MessagePreviewUnread : _theme.Home.MessagePreview;
    }

    protected override void Draw(Surface s)
    {
        if (base.IsDirty)
        {
            s.Fill(' ', (IsSelected ? SelectedStyle : Style) );

            UpdateStyles();

            _nickname.Invalidate();
            _fingerprint.Invalidate();
            _latestMessage.Invalidate();
            _unreadMessageCountText.Invalidate();
        }

        _nickname.Render(s);
        _fingerprint.Render(s);
        _latestMessage.Render(s);

        if (UnreadMessageCount > 0)
            _unreadMessageCountText.Render(s);
    }
}
