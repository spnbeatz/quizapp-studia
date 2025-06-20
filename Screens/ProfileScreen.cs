using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Forms.Components;
using projekt.Interfaces;
using projekt.Models;
using projekt.Routing;
using projekt.Utils;

namespace projekt.Screens
{
    public class ProfileScreen : UserControl, IRoutable
    {
        private TableLayoutPanel _mainLayout;
        private TableLayoutPanel _headerLayout;
        private Label _usernameLabel;
        private QuizListPanel _quizListPanel;
        
        private User _userData;

        private int _userId;
        private readonly IQuizService _quizService;
        private readonly IUserService _userService;
        private Router _router;
        public ProfileScreen(IQuizService quizService, IUserService userService, Router router) { 
            _quizService = quizService;
            _userService = userService;
            _router = router;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.AutoSize = true;

            _mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2
            };

            _mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _headerLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Top,
            };

            _usernameLabel = new Label
            {
                Text = "Username",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true
            };

            _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _headerLayout.Controls.Add(_usernameLabel, 0, 0);

            _mainLayout.Controls.Add(_headerLayout, 0, 0);
            this.Controls.Add(_mainLayout);
        }

        public void OnNavigatedTo(object parameter)
        {
            _userId = parameter is int id ? id : Session.CurrentUser.Id;

            _userData = _userService.GetUserById(_userId);

            _usernameLabel.Text = _userData.Username;
            
            if (_quizListPanel != null)
            {
                _mainLayout.Controls.Remove(_quizListPanel);
                _quizListPanel.Dispose();
            }
            
            _quizListPanel = new QuizListPanel(_quizService, _router, _userId);

            _mainLayout.Controls.Add(_quizListPanel, 0, 1);
        }

    }
}
