using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace ReactionControl;

public partial class LikeControl : UserControl
{
    private bool liked = false;
    private int likeCount = 0;

    public event EventHandler LikeChanged;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Liked
    {
        get { return liked; }
        set
        {
            liked = value;
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int LikeCount
    {
        get { return likeCount; }
        set
        {
            likeCount = value;
            Invalidate();
        }
    }

    public LikeControl()
    {
        InitializeComponent();
        this.Size = new Size(50, 50);
        this.Cursor = Cursors.Hand;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;

        string heart = liked ? "♥" : "♡";

        g.DrawString(heart, Font, Brushes.Red, 0, 5);
        g.DrawString(likeCount.ToString(), Font, Brushes.Black, 20, 5);
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);

        Liked = !Liked;

        if (Liked)
            LikeCount++;
        else
            LikeCount--;

        LikeChanged?.Invoke(this, EventArgs.Empty);

        Invalidate();
    }
}
