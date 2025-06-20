// komponent przycisku start quizu

using System;
using System.Drawing;
using System.Windows.Forms;
using projekt.Events;
using projekt.Interfaces;
using projekt.Routing;
using projekt.Styling;
using projekt.Utils;

namespace projekt.Forms.Components
{
    internal class QuizListItem : Panel
    {
        private TableLayoutPanel _layout;
        private TableLayoutPanel _textContainer;
        private Label _title;
        private Label _description;
        private Label _authorName;
        private Label _questionCount;
        private TableLayoutPanel _buttonsContainer;
        private Button _playButton;
        private Button _editButton;
        private Button _deleteButton;

        public int quizId;
        private int _authorId;
        private IAppStyle Style => AppStyle.Current;
        private Router _router;


        public string Title
        {
            get => _title.Text;
            set => _title.Text = value?.Length > 50 ? value.Substring(0, 50) + "..." : value;
        }

        public string Description
        {
            get => _description.Text;
            set => _description.Text = value;
        }

        public string AuthorName
        {
            get => _authorName.Text;
            set => _authorName.Text = value;
        }

        public string QuestionCount
        {
            get => _questionCount.Text;
            set => _questionCount.Text = value;
        }

        public QuizListItem(Router router, int authorId)
        {
            _router = router;
            _authorId = authorId;
            InitializeComponent();

            AppStyle.StyleChanged += (s, e) => UpdateStyle();
            UpdateStyle();
        }

        private void InitializeComponent()
        {
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Padding = new Padding(10, 10, 10, 0);
            _layout = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
            };

            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

            _textContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                RowCount = 2,
            };

            _textContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            _textContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _textContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            _textContainer.ColumnCount = 1;
            _textContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _title = new Label
            {
                Text = "Tytuł",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                AutoSize = true,
                // MaximumSize = new Size(400, 30),
            };

            _description = new Label
            {
                Text = "Opis quizu...",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.WhiteSmoke,
                Dock = DockStyle.Fill,
                AutoSize = true,
                // MaximumSize = new Size(400, 40),
            };

            _authorName = new Label
            {
                Text = "username",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.WhiteSmoke,
                Dock = DockStyle.Fill,
                AutoSize = true,
            };

            _textContainer.Controls.Add(_title, 0, 0);
            _textContainer.Controls.Add(_description, 0, 1);
            _textContainer.Controls.Add(_authorName, 0, 2);

            _buttonsContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3,
            };

            _buttonsContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _buttonsContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _buttonsContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _buttonsContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            _buttonsContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _buttonsContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            _playButton = new Button
            {
                Width = 20,
                Height = 20,
                FlatStyle = FlatStyle.Flat,
                
            };

            _editButton = new Button
            {
                Width = 20,
                Height = 20,
                FlatStyle = FlatStyle.Flat,
            };

            _deleteButton = new Button
            {
                Width = 20,
                Height = 20,
                FlatStyle = FlatStyle.Flat,
            };

            _questionCount = new Label
            {
                Text = "10 pytan",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight
            };

            _buttonsContainer.Controls.Add(_playButton, 2, 0);
            _buttonsContainer.Controls.Add(_questionCount, 1, 2);
            _playButton.Click += (s, e) => _router.Navigate("solvequiz", quizId);

            if(_authorId == Session.CurrentUser.Id)
            {


                _buttonsContainer.Controls.Add(_editButton, 0, 0);
                _buttonsContainer.Controls.Add(_deleteButton, 1, 0);
                _editButton.Click += (s, e) => EditButtonClick();
                _deleteButton.Click += (s, e) => DeleteButtonClick();

                _editButton.FlatAppearance.BorderSize = 0;
                _deleteButton.FlatAppearance.BorderSize = 0;
            }


            _playButton.FlatAppearance.BorderSize = 0;
            

            _layout.Controls.Add(_textContainer, 0, 0);
            _layout.Controls.Add(_buttonsContainer, 1, 0);



            this.Controls.Add(_layout);
        }

        private void EditButtonClick()
        {
            _router.Navigate("quizform", quizId);
        }

        private void DeleteButtonClick()
        {
            QuizEvents.NotifyQuizRemoved(quizId);
        }

        private void UpdateStyle()
        {
            _title.ForeColor = Style.Foreground;
            _description.ForeColor = Style.Foreground;
            _authorName.ForeColor = Style.Foreground;
            _playButton.Image = Style.playButton;
            _editButton.Image = Style.editButton;
            _deleteButton.Image = Style.deleteButton;
        }
    }


}

