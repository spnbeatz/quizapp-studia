using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using projekt.Models;
using projekt.Routing;
using projekt.Interfaces;
using projekt.Styling;
using projekt.Utils;
using projekt.Forms.Components;

namespace projekt.Screens
{
    public class DashboardScreen : UserControl, IRoutable
    {
        private Label lblWelcome;
        private QuizListPanel quizListControl;
        private Panel emptyPanel;

        private TableLayoutPanel mainTable;

        public event EventHandler? LogoutRequested;
        public event EventHandler? CreateQuizRequested;
        public event EventHandler? BrowseQuizzesRequested;

        private readonly Router _router;
        private readonly IQuizService _quizService;
        private IAppStyle Style => AppStyle.Current;

        public DashboardScreen(Router router, IQuizService quizService)
        {
            _router = router;
            _quizService = quizService;

            InitializeComponents();

            AppStyle.StyleChanged += (s, e) => UpdateStyle();
            UpdateStyle();
        }

        private void InitializeComponents()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.Gainsboro;

            mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                
            };

            mainTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 450));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));

            lblWelcome = new Label
            {
                Text = "Witaj",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                Padding = new Padding(10),
            };

            quizListControl = new QuizListPanel(_quizService, _router, null);

            emptyPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
            };

            mainTable.Controls.Add(lblWelcome, 0, 0);
            mainTable.Controls.Add(quizListControl, 0, 1);
            mainTable.Controls.Add(emptyPanel, 0, 2);

            Controls.Add(mainTable);
        }

        private void UpdateStyle()
        {
            this.BackColor = Style.Background;
            this.ForeColor = Style.Foreground;
            lblWelcome.ForeColor = Style.Foreground;
        }

        public void OnNavigatedTo(object parameter) { }
    }
}

