using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Interfaces;
using projekt.Models;
using projekt.Routing;
using projekt.Styling;
using projekt.Forms.Components;

namespace projekt.Screens
{
    public class SolveQuizScreen : UserControl, IRoutable
    {
        private TableLayoutPanel _headerPanel;
        private TableLayoutPanel _rootLayout;

        private FlowLayoutPanel _mainPanel;
        private Label _titleLabel;
        private Button _finishQuiz;
        private Quiz _quizData;

        private readonly IQuizService _quizService;
        private readonly Router _router;
        private IAppStyle Style => AppStyle.Current;

        private Button _escapeButton;

        public SolveQuizScreen(IQuizService quizService, Router router)
        {
            _quizService = quizService;
            _router = router;
            InitializeComponent();
            AppStyle.StyleChanged += (s, e) => UpdateStyle();
            UpdateStyle();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.Padding = new Padding(20);

            _rootLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
            };

            _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _mainPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
            };

            _headerPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
            };

            _headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 95F));
            _headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));


            _titleLabel = new Label
            {
                Font = new Font("Sagoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                
            };

            

            _escapeButton = new Button
            {
                Width = 40,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
            };

            _escapeButton.FlatAppearance.BorderSize = 0;
            _escapeButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            _escapeButton.FlatAppearance.MouseDownBackColor = Color.Transparent;

            _escapeButton.Click += (s, e) => _router.Back();



            _headerPanel.Controls.Add(_titleLabel, 0, 0);
            _headerPanel.Controls.Add(new Panel(), 1, 0);
            _headerPanel.Controls.Add(_escapeButton, 2, 0);

            _rootLayout.Controls.Add(_headerPanel, 0, 0);
            _rootLayout.Controls.Add(_mainPanel, 0, 2);

            this.Controls.Add(_rootLayout);
        }

        private void ConfirmSolvingQuiz()
        {
            var result = MessageBox.Show(
                "Czy na pewno chcesz zakończyć rozwiązywanie quizu?",
                "Potwierdzenie",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                GetQuizSummary();
            }
        }

        private void GetQuizSummary()
        {
            double totalScore = 0;
            int totalQuestions = 0;

            var answers = new List<AnsweredQuestion>();

            foreach (QuestionItem item in _mainPanel.Controls.OfType<QuestionItem>())
            {
                var q = BuildSummary(item);
                answers.Add(q);
            }

            SummaryData summary = new SummaryData()
            {
                Id = _quizData.Id,
                Title = _quizData.Title,
                UserId = _quizData.UserId,
                Description = _quizData.Description,
                Level = _quizData.Level,
                Answers = answers,
            };

            _router.Navigate("summary", summary);
        }

        public AnsweredQuestion BuildSummary(QuestionItem item)
        {
            var question = item._question;
            var selectedAnswers = item.GetSelectedAnswers();

            return new AnsweredQuestion
            {
                Text = question.Text,
                QuestionType = question.QuestionType,
                Answers = item.answerList.Select(answer => new AnsweredAnswer
                {
                    Text = answer.Text,
                    IsCorrect = answer.IsCorrect,
                    IsSelected = selectedAnswers.Any(sa => sa.Text == answer.Text)
                }).ToList()
            };
        }



        private async Task LoadDataAsync(int quizId)
        {
            try
            {
                _mainPanel.Controls.Clear();
                _quizData = await _quizService.GetQuizByIdAsync(quizId);
                _titleLabel.Text = _quizData.Title;
                if (_quizData.Questions != null)
                {
                    foreach (Models.Question question in _quizData.Questions)
                    {
                        _mainPanel.Controls.Add(new QuestionItem(question));
                    }
                }


            }
            catch (Exception ex) 
            {
            }
            finally
            {
                _mainPanel.Controls.Add(_finishQuiz = new Button
                {
                    Text = "Zakończ",
                    Width = 490,
                    FlatStyle = FlatStyle.Flat,
                    Height = 50,
                });

                _finishQuiz.Click += (s, e) => ConfirmSolvingQuiz();
            }
        }

        private async Task GetQuizStatsAsync(int quizId)
        {
            var stats = await _quizService.GetQuizStats(quizId);

            var existingControl = _rootLayout.GetControlFromPosition(0, 1);
            if (existingControl != null)
            {
                _rootLayout.Controls.Remove(existingControl);
                existingControl.Dispose();
            }
            if (stats != null)
            {
                _rootLayout.Controls.Add(new QuizStatsPanel(stats), 0, 1);
            }
            

        }

        public async void OnNavigatedTo(object parameter)
        {

            if (parameter is int quizId)
            {
                await LoadDataAsync(quizId);
                await GetQuizStatsAsync(quizId);
            }
        }

        private void UpdateStyle() {
            this.BackColor = Style.Background;
            _titleLabel.ForeColor = Style.Foreground;
            _escapeButton.Image = Style.cancelButton;
        }
    }
}
