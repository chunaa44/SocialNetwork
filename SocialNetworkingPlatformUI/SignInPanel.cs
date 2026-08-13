using System;
using System.Drawing;
using System.Windows.Forms;
using SocialPlatformLibrary;

namespace SocialNetworkingPlatformUI;

/// <summary>
/// Sign-in form. Form1 swaps this out for <see cref="SignUpPanel"/> 
/// when the user asks to. All
/// persistence goes through <see cref="Platform"/>, never SQL directly.
/// </summary>
public class SignInPanel : Panel
{
    private readonly Platform _platform;
    private readonly Action<User> _onSignedIn;

    private readonly TextBox _txtEmail;
    private readonly TextBox _txtPassword;
    private readonly Label _lblStatus;

    public SignInPanel(Platform platform, Action<User> onSignedIn, Action onSwitchToSignUp)
    {
        _platform = platform;
        _onSignedIn = onSignedIn;
        BackColor = Color.White;

        var title = new Label
        {
            Text = "Social Network",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(40, 40)
        };

        var lblEmail = new Label { Text = "Email", Location = new Point(40, 100), AutoSize = true };
        _txtEmail = new TextBox { Location = new Point(40, 120), Width = 260 };

        var lblPassword = new Label { Text = "Password", Location = new Point(40, 150), AutoSize = true };
        _txtPassword = new TextBox { Location = new Point(40, 170), Width = 260, UseSystemPasswordChar = true };

        var btnLogIn = new Button { Text = "Log In", Location = new Point(40, 210), Width = 120, Height = 32 };
        btnLogIn.Click += BtnLogIn_Click;

        var lnkSignUp = new LinkLabel
        {
            Text = "Need an account? Sign up",
            Location = new Point(40, 255),
            AutoSize = true
        };
        lnkSignUp.LinkClicked += (_, _) => onSwitchToSignUp();

        _lblStatus = new Label
        {
            ForeColor = Color.Firebrick,
            Location = new Point(40, 285),
            AutoSize = true,
            MaximumSize = new Size(300, 0)
        };

        Controls.Add(title);
        Controls.Add(lblEmail);
        Controls.Add(_txtEmail);
        Controls.Add(lblPassword);
        Controls.Add(_txtPassword);
        Controls.Add(btnLogIn);
        Controls.Add(lnkSignUp);
        Controls.Add(_lblStatus);
    }

    private void BtnLogIn_Click(object? sender, EventArgs e)
    {
        try
        {
            // Platform.Login returns null on bad credentials (no exception),
            // so we turn that into an ArgumentException here to show a message.
            var user = _platform.Login(_txtEmail.Text.Trim(), _txtPassword.Text)
                ?? throw new ArgumentException("Incorrect email or password.");
            _onSignedIn(user);
        }
        catch (ArgumentException ex)
        {
            _lblStatus.Text = ex.Message;
        }
    }
}