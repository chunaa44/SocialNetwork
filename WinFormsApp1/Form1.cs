using ReactionControl;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

        LikeControl like = new LikeControl();
        like.Location = new Point(100, 100);

        like.LikeChanged += Like_LikeChanged;

        this.Controls.Add(like);
    }

    private void Like_LikeChanged(object sender, EventArgs e)
    {
        LikeControl lc = (LikeControl)sender;
        Text = "Likes: " + lc.LikeCount;
    }
}
