using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using projekt.Config;
using projekt.Forms.Components;
using projekt.Interfaces;
using projekt.Routing;
using projekt.Styling;
using projekt.Utils;

namespace projekt
{
    public class App : Form
    {
        private Panel mainPanel;
        private readonly Router _router;
        private Navigation navigationPanel;
        private IAppStyle Style => AppStyle.Current;
        public App(IServiceProvider services)
        {

            _router = services.GetRequiredService<Router>();
            InitializeWindow();

            
            _router.Initialize(mainPanel, services);
            _router.Navigate("startup");

            Session.OnAuthenticationChanged += (s, e) => UpdateLayout();
            AppStyle.StyleChanged += (s, e) => UpdateStyle();
            UpdateStyle();
            // StyleHelper.ApplyStyle(this);

            UpdateLayout();
        }

        private void InitializeWindow()
        {
            Text = "Quiz App";
            Size = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0,15,0,0)
            };

            navigationPanel = new Navigation(_router)
            {
                Dock = DockStyle.Left,
                Width = 40,
                BackColor = Color.FromArgb(30, 30, 30)
            };

            Controls.Add(mainPanel);
            Controls.Add(navigationPanel);
        }

        private void UpdateLayout()
        {
            if (Session.IsAuthenticated)
            {
                navigationPanel.Visible = true;
                _router.Navigate("dashboard");
                navigationPanel.SetActiveButton(navigationPanel.Controls.OfType<Button>().FirstOrDefault());
            }
            else
            {
                navigationPanel.Visible = false;
                _router.Navigate("login");
            }
        }

        private void UpdateStyle()
        {
            this.BackColor = Style.Background;
            this.ForeColor = Style.Foreground;
        }
    }
}

