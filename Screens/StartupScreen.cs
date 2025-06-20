using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Interfaces;
using projekt.Routing;
using projekt.Utils;

namespace projekt.Screens
{
    public class StartupScreen : UserControl, IRoutable
    {
        private readonly Router _router;

        public StartupScreen(Router router)
        {
            _router = router;
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;

            var loadingLabel = new Label
            {
                Text = "Ładowanie...",
                Font = new Font("Segoe UI", 16),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            this.Controls.Add(loadingLabel);
        }

        public void OnNavigatedTo(object parameter)
        {
            if (Session.IsAuthenticated)
                _router.Navigate("dashboard");
            else
                _router.Navigate("login");
        }
    }
}
