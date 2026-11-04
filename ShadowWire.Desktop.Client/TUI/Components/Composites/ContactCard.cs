using ShadowWire.Desktop.Client.TUI.Components.Core;
using ShadowWire.Desktop.Client.TUI.Components.Primitives;
using ShadowWire.Shared.Users;
using System.Drawing;

namespace ShadowWire.Desktop.Client.TUI.Components.Composites;

public class ContactCard : BaseComponent, IFocusable
{
    public Fingerprint Fingerprint => _contact.Fingerprint;
    private Contact _contact;

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

    public bool IsFocused { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


    public ContactCard(Contact contact, int width) : base(Point.Empty, new Size(width, 3))
    {
        _contact = contact;

        _nickname = new Text(new Point(0, 0), new Size(32, 1))
        {
            Content      = contact.Nickname,
            OverflowType = Overflow.Ellipsis,
        };

        var fingerprintBytes = contact.Fingerprint.Span;
        var fingerprintString = $"{Convert.ToHexStringLower(fingerprintBytes[..4])}…{Convert.ToHexStringLower(fingerprintBytes[^4..])}";
        _fingerprint = new Text(new Point(40, 0), new Size(32, 1))
        {
            Content      = fingerprintString,
            OverflowType = Overflow.Ellipsis,
        };

        _unreadMessageCountText = new Text(new Point(64, 0), new Size(5, 1))
        {
            OverflowType = Overflow.Clip,
        };
        _latestMessage = new Text(new Point(0, 1), new Size(5, 1))
        {
            OverflowType = Overflow.Ellipsis,
        };
    }


    protected override void Draw(Surface s)
    {
        if (base.IsDirty)
        {
            s.Fill(' ', Style);

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
