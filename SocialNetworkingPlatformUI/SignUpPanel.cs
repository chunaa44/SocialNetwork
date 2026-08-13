using System;
using System.Drawing;
using System.Windows.Forms;
using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;

namespace SocialNetworkingPlatformUI;

/// <summary>
/// Sign-up form.
/// All persistence goes through <see cref="Platform"/>, never SQL directly.
/// </summary>
public class SignUpPanel : Panel
{
    private readonly Platform _platform;
    private readonly Action<User> _onSignedUp;

    private readonly TextBox _txtName;
    private readonly TextBox _txtEmail;
    private readonly TextBox _txtPassword;
    private readonly Label _lblStatus;

    public SignUpPanel(Platform platform, Action<User> onSignedUp, Action onSwitchToSignIn)
    {
        _platform = platform;
        _onSignedUp = onSignedUp;
        BackColor = Color.White;

        var title = new Label
        {
            Text = "Create an account",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(40, 40)
        };

        var lblName = new Label { Text = "Name", Location = new Point(40, 100), AutoSize = true };
        _txtName = new TextBox { Location = new Point(40, 120), Width = 260 };

        var lblEmail = new Label { Text = "Email", Location = new Point(40, 150), AutoSize = true };
        _txtEmail = new TextBox { Location = new Point(40, 170), Width = 260 };

        var lblPassword = new Label { Text = "Password", Location = new Point(40, 200), AutoSize = true };
        _txtPassword = new TextBox { Location = new Point(40, 220), Width = 260, UseSystemPasswordChar = true };

        var btnSignUp = new Button { Text = "Sign Up", Location = new Point(40, 260), Width = 120, Height = 32 };
        btnSignUp.Click += BtnSignUp_Click;

        var lnkSignIn = new LinkLabel
        {
            Text = "Already have an account? Log in",
            Location = new Point(40, 305),
            AutoSize = true
        };
        lnkSignIn.LinkClicked += (_, _) => onSwitchToSignIn();

        _lblStatus = new Label
        {
            ForeColor = Color.Firebrick,
            Location = new Point(40, 335),
            AutoSize = true,
            MaximumSize = new Size(300, 0)
        };

        Controls.Add(title);
        Controls.Add(lblName);
        Controls.Add(_txtName);
        Controls.Add(lblEmail);
        Controls.Add(_txtEmail);
        Controls.Add(lblPassword);
        Controls.Add(_txtPassword);
        Controls.Add(btnSignUp);
        Controls.Add(lnkSignIn);
        Controls.Add(_lblStatus);
    }

    private void BtnSignUp_Click(object? sender, EventArgs e)
    {
        try
        {
            // UserService validates name/email/password and UserRepoSQLite
            // enforces a UNIQUE index on Email, so both bad input and a
            // duplicate email are caught below.
            var user = _platform.CreateUser(new UserDTO(_txtName.Text.Trim(), _txtEmail.Text.Trim(), _txtPassword.Text));
            _onSignedUp(user);
        }
        catch (ArgumentException ex)
        {
            _lblStatus.Text = ex.Message;
        }
        catch (Exception ex) when (ex.Message.Contains("UNIQUE constraint failed"))
        {
            _lblStatus.Text = "An account with that email already exists.";
        }
    }
}