using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SocialPlatformLibrary;

namespace ReactionControl;

/// <summary>
/// A row of clickable reaction icons, one per <see cref="ReactionType"/>.
/// Raises <see cref="ReactionSelected"/> when the user picks one. The control holds no
/// business logic itself — the host calls Platform.SetReaction and sets
/// <see cref="CurrentReaction"/> back based on the real result from the platform.
/// </summary>
public partial class ReactionPicker : UserControl
{
    // All reaction types, drawn left to right in this order.
    private static readonly ReactionType[] AllReactions = (ReactionType[])Enum.GetValues(typeof(ReactionType));

    // Reused format so each glyph is centered in its icon circle.
    private static readonly StringFormat CenteredFormat = new()
    {
        Alignment = StringAlignment.Center,
        LineAlignment = StringAlignment.Center
    };

    private ReactionType? currentReaction;
    private int iconSize = 28;
    private int spacing = 6;

    /// <summary>Diameter, in pixels, of each reaction icon. Configurable in the designer.</summary>
    [Category("Appearance")]
    [Description("Diameter, in pixels, of each reaction icon.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int IconSize
    {
        get => iconSize;
        set { iconSize = value; UpdateSize(); }
    }

    /// <summary>Space, in pixels, between adjacent reaction icons. Configurable in the designer.</summary>
    [Category("Appearance")]
    [Description("Space, in pixels, between adjacent reaction icons.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int Spacing
    {
        get => spacing;
        set { spacing = value; UpdateSize(); }
    }

    /// <summary>The reaction currently shown as selected (filled in). Not designer-visible —
    /// the host sets this after reading the real state back from the platform, since the
    /// control itself has no idea what the "correct" reaction is.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ReactionType? CurrentReaction
    {
        get => currentReaction;
        set { currentReaction = value; Invalidate(); }
    }

    /// <summary>Raised when the user clicks a reaction icon, with the type they picked. This is
    /// a custom event specific to this control, not a re-purposed standard Click event.</summary>
    public event EventHandler<ReactionType>? ReactionSelected;

    public ReactionPicker()
    {
        InitializeComponent();
        Cursor = Cursors.Hand;
        UpdateSize();
    }

    // Resizes the control to exactly fit all icons given the current IconSize/Spacing.
    private void UpdateSize()
    {
        Width = AllReactions.Length * (iconSize + spacing) - spacing;
        Height = iconSize;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        // Emoji glyphs need Segoe UI Emoji specifically — the default control font won't
        // have color emoji glyphs.
        using var emojiFont = new Font("Segoe UI Emoji", iconSize * 0.5f, GraphicsUnit.Pixel);

        for (int i = 0; i < AllReactions.Length; i++)
        {
            var reaction = AllReactions[i];
            var rect = new Rectangle(i * (iconSize + spacing), 0, iconSize, iconSize);
            bool isSelected = reaction == currentReaction;

            // Selected reaction gets a filled, colored circle; others just an outline.
            if (isSelected)
                g.FillEllipse(new SolidBrush(ReactionColor(reaction)), rect);
            else
                g.DrawEllipse(Pens.Gray, rect);

            // Emoji already carry their own color, so text color here only matters
            // as a fallback if the font can't render the glyph.
            var textColor = isSelected ? Color.White : Color.Black;
            g.DrawString(ReactionGlyph(reaction), emojiFont, new SolidBrush(textColor), rect, CenteredFormat);
        }
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);

        for (int i = 0; i < AllReactions.Length; i++)
        {
            var rect = new Rectangle(i * (iconSize + spacing), 0, iconSize, iconSize);
            if (rect.Contains(e.Location))
            {
                ReactionSelected?.Invoke(this, AllReactions[i]);
                return;
            }
        }
    }

    // Emoji glyph shown for each reaction type.
    private static string ReactionGlyph(ReactionType type) => type switch
    {
        ReactionType.Like => "👍",
        ReactionType.Love => "❤️",
        ReactionType.Haha => "😆",
        ReactionType.Wow => "😮",
        ReactionType.Sad => "😢",
        ReactionType.Angry => "😠",
        _ => "?"
    };

    // Fill color used when a reaction is the selected one.
    private static Color ReactionColor(ReactionType type) => type switch
    {
        ReactionType.Like => Color.DodgerBlue,
        ReactionType.Love => Color.Crimson,
        ReactionType.Angry => Color.OrangeRed,
        _ => Color.Goldenrod // Haha, Wow, Sad share a neutral accent color
    };
}