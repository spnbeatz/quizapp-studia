using projekt.Utils;
using projekt.Forms.Components;
using projekt.Interfaces;
using projekt.Routing;
using projekt.Styling;



namespace projekt.Screens
{
    public class RegisterScreen : UserControl, IRoutable
    {
        private FloatingLabelTextBox txtUsername;
        private FloatingLabelTextBox txtPassword;
        private FloatingLabelTextBox txtConfirmPassword;
        private StyledButton btnRegister;
        private Label lblMessage;
        private Button btnToLogin;
        private Panel container;
        private Router _router;

        private IAppStyle Style => AppStyle.Current;

        private readonly IUserService _userService;
        public RegisterScreen(Router router, IUserService userService)
        {
            _router = router;
            _userService = userService;
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
                Text = "Rejestracja",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(24, 30)
            };

            lblMessage = new Label
            {
                ForeColor = Color.Red,
                AutoSize = true,
                Location = new Point(30, 85),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                MaximumSize = new Size(320, 0)
            };

            txtUsername = new FloatingLabelTextBox
            {
                LabelText = "Nazwa użytkownika",
                Location = new Point(30, 100),
                Width = 320,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.None,

            };

            txtPassword = new FloatingLabelTextBox
            {
                LabelText = "Hasło",
                Location = new Point(30, 160),
                Width = 320,
                Font = new Font("Segoe UI", 10F),
                UseSystemPasswordChar = true,
                BorderStyle = BorderStyle.None
            };

            txtConfirmPassword = new FloatingLabelTextBox
            {
                LabelText = "Powtórz hasło",
                Location = new Point(30, 220),
                Width = 320,
                Font = new Font("Segoe UI", 10F),
                UseSystemPasswordChar = true,
                BorderStyle = BorderStyle.None
            };

            btnRegister = new StyledButton
            {
                Text = "Zarejestruj się",
                Location = new Point(30, 290),
                Width = 320,
                Height = 40,
                CardBackColor = Color.Gray,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                CornerRadius = 5
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;



            btnToLogin = new Button
            {
                Text = "Masz już konto? Zaloguj się",
                Location = new Point(25, 340),
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(33, 150, 243),
                Font = new Font("Segoe UI", 10, FontStyle.Underline),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                TabStop = false
            };
            btnToLogin.FlatAppearance.BorderSize = 0;
            btnToLogin.Click += (s, e) => _router.Navigate("login");

            container.Controls.AddRange(new Control[] {
            lblTitle, txtUsername, txtPassword, txtConfirmPassword,
            btnRegister, lblMessage, btnToLogin
            });

            Controls.Add(container);
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.TextValue.Trim();
            string password = txtPassword.TextValue;
            string confirmPassword = txtConfirmPassword.TextValue;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblMessage.Text = "Wprowadź wszystkie dane.";
                return;
            }

            if (password != confirmPassword)
            {
                lblMessage.Text = "Hasła się nie zgadzają.";
                return;
            }

            string hashedPassword = PasswordHasher.HashPassword(password);
            bool success = _userService.RegisterUser(username, hashedPassword);
            if (success)
            {
                lblMessage.ForeColor = Color.Green;
                lblMessage.Text = "Rejestracja zakończona sukcesem!";
                _router.Navigate("login");
            }
            else
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "Użytkownik o takiej nazwie już istnieje.";
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
