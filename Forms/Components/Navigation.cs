using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Routing;
using projekt.Properties;
using System.Drawing;
using projekt.Styling;


namespace projekt.Forms.Components
{
    internal class Navigation : Panel
    {
        private Router _router;
        private Button _activeButton;
        private PictureBox logo;
        private Button themeBtn;
        private FlowLayoutPanel navButtonsPanel;

        public Navigation(Router router)
        {
            _router = router;

            this.Dock = DockStyle.Left;
            this.Width = 60;
            this.BackColor = Color.Transparent;
            this.Padding = new Padding(0, 20, 0, 0);

            InitializeComponent();

            AppStyle.StyleChanged += (s, e) => UpdateStyle();
            UpdateStyle();
            
        }

        private void InitializeComponent()
        {
            navButtonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            logo = new PictureBox
            {
                Image = Image.FromFile("Assets/logo.png"),
                Width = 40,
                Height = 40,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Margin = new Padding(0, 0, 0, 15)
            };
            navButtonsPanel.Controls.Add(logo);

            var btnDashboard = CreateNavButton("dashboard", Image.FromFile("Assets/dashboard.png"));
            navButtonsPanel.Controls.Add(btnDashboard);
            SetActiveButton(btnDashboard);

            var btnProfile = CreateNavButton("profile", Image.FromFile("Assets/user.png"));
            navButtonsPanel.Controls.Add(btnProfile);

            var btnSearch = CreateNavButton("search", Image.FromFile("Assets/search.png"));
            navButtonsPanel.Controls.Add(btnSearch);

            var btnCreateQuiz = CreateNavButton("quizform", Image.FromFile("Assets/addition.png"));
            navButtonsPanel.Controls.Add(btnCreateQuiz);

            this.Controls.Add(navButtonsPanel);

            themeBtn = new Button
            {
                Height = 40,
                Width = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0),
                Text = "",
                ImageAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom
            };

            themeBtn.FlatAppearance.BorderSize = 0;
            themeBtn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            themeBtn.FlatAppearance.MouseDownBackColor = Color.Transparent;

            themeBtn.Click += (s, e) => AppStyle.SetStyle();
            this.Controls.Add(themeBtn);
        }

        private Button CreateNavButton(string route, Image icon)
        {
            var btn = new Button
            {
                Height = 40,
                Width = 40,
                FlatStyle = FlatStyle.Flat,
                Image = icon,
                BackColor = Color.Transparent,
                Tag = route,
                Margin = new Padding(0),
                Padding = new Padding(0),
                Text = "",
                ImageAlign = ContentAlignment.MiddleCenter
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 70);

            btn.Click += (s, e) =>
            {
                SetActiveButton(btn);
                _router.Navigate(route);
            };

            return btn;
        }

        public void SetActiveButton(Button btn)
        {
            if (btn == themeBtn)
                return;
            if (_activeButton != null)
                _activeButton.BackColor = Color.Transparent;

            _activeButton = btn;
            _activeButton.BackColor = Color.FromArgb(100, 100, 100);
        }

        private void UpdateStyle()
        {
            themeBtn.Image = AppStyle.Current.themeButton;
        }
    }

}
