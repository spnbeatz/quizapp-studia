using projekt.Utils;
using projekt.Forms.Components;
using System.Diagnostics;
using projekt.Interfaces;
using projekt.Routing;
using projekt.Styling;

namespace projekt.Screens
{
    public class LoginScreen : UserControl, IRoutable
    {
        private FloatingLabelTextBox txtUsername;
        private FloatingLabelTextBox txtPassword;
        private StyledButton btnLogin;
        private Label lblMessage;
        private Button btnToRegister;
        private Button changeStyle;
        private Panel container;
        private readonly Router _router;

        private IAppStyle Style => AppStyle.Current;

        private readonly IUserService _userService;
        public LoginScreen(Router router, IUserService userService)
        {
            _userService = userService;
            _router = router;
            InitializeComponents();
            AppStyle.StyleChanged += (s, e) => UpdateStyle();
            UpdateStyle();
        }



        private void InitializeComponents()
        {
            Dock = DockStyle.Fill;

            container = new Panel
            {
                Size = new Size(380, 460),
                Location = new Point(
                (ClientSize.Width - 380) / 2,
                (ClientSize.Height - 460) / 2),
                Anchor = AnchorStyles.None
            };

            Resize += (s, e) =>
            {
                container.Location = new Point(
                    (ClientSize.Width - container.Width) / 2,
                    (ClientSize.Height - container.Height) / 2);
            };

            Label lblTitle = new Label
            {
                Text = "Logowanie",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(24, 80)
            };

            lblMessage = new Label
            {
                ForeColor = Color.Red,
                AutoSize = true,
                Location = new Point(30, 135),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                MaximumSize = new Size(320, 0)
            };

            txtUsername = new FloatingLabelTextBox
            {
                LabelText = "Nazwa użytkownika",
                Location = new Point(30, 150),
                Width = 320,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.None,

            };

            txtPassword = new FloatingLabelTextBox
            {
                LabelText = "Hasło",
                Location = new Point(30, 210),
                Width = 320,
                Font = new Font("Segoe UI", 10F),
                UseSystemPasswordChar = true,
                BorderStyle = BorderStyle.None
            };

            btnLogin = new StyledButton
            {
                Text = "Zaloguj się",
                Location = new Point(30, 280),
                Width = 320,
                Height = 40,
                CardBackColor = Color.Gray,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                CornerRadius = 5
            };
            btnLogin.Click += BtnLogin_Click;


            btnToRegister = new Button
            {
                Text = "Nie masz konta? Zarejestruj się!",
                Location = new Point(25, 330),
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(33, 150, 243),
                Font = new Font("Segoe UI", 9F, FontStyle.Underline),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                TabStop = false
            };

            changeStyle = new Button
            {
                Text = "ChangeStyle",
                Location = new Point(25, 370),
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(33, 150, 243),
                Font = new Font("Segoe UI", 9F, FontStyle.Underline),
                Cursor = Cursors.Hand,

            };

            container.Controls.Add(changeStyle);

            changeStyle.Click += (s, e) => AppStyle.SetStyle();
            btnToRegister.FlatAppearance.BorderSize = 0;
            btnToRegister.Click += (s, e) => _router.Navigate("register");

            container.Controls.AddRange(new Control[] {
                lblTitle, txtUsername, txtPassword,
                btnLogin, lblMessage, btnToRegister
            });

            Controls.Add(container);
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.TextValue.Trim();
            string password = txtPassword.TextValue;
            Debug.WriteLine("Kliknieto");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblMessage.Text = "Wprowadź dane logowania.";
                return;
            }
            string? storedHashed = _userService.GetUserPasswordHash(username);
            Debug.WriteLine($"Pobrane haslo: {storedHashed}");
            Debug.WriteLine($"Haslo: {password}");
            if (storedHashed != null && PasswordHasher.VerifyPassword(password, storedHashed))
            {
                Debug.WriteLine("Zweryfikowano haslo");
                var user = _userService.AuthenticateUser(username, storedHashed);
                Debug.WriteLine($"Pobrano user: {user}");
                if (user != null)
                {
                    Session.CurrentUser = user;
                    _router.Navigate("dashboard");
                }
                else
                {
                    lblMessage.Text = "Nieprawidłowa nazwa użytkownika lub hasło.";
                }
            }
            else
            {
                lblMessage.Text = "Nieprawidłowa nazwa użytkownika lub hasło.";
            }


        }

        private void UpdateStyle()
        {
            this.BackColor = Style.Background;
            this.ForeColor = Style.Foreground;
        }

        public void OnNavigatedTo(object parameter) { }
    }
}
