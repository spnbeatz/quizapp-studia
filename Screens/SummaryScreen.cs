using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Interfaces;
using projekt.Models;
using projekt.Forms.Components;
using projekt.Routing;

namespace projekt.Screens
{
    public class SummaryScreen : UserControl, IRoutable
    {
        private FlowLayoutPanel _mainPanel;
        private Label _scoreLabel;
        private Label _quizNameLabel;
        private Button _backButton;
        private SummaryData _summaryData;
        private Button _confirmButton;
        private Router _router;
        private double _score;

        private readonly IQuizService _quizService;

        public SummaryScreen(Router router, IQuizService quizService)
        {
            _router = router;
            _quizService = quizService;
            this.Dock = DockStyle.Fill;
            this.AutoSize = true;
            this.AutoScroll = true;
        }

        private void InitializeComponent()
        {

            this.Controls.Clear();
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            _mainPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,

            };

            _quizNameLabel = new Label
            {
                Text = $"{_summaryData.Title} - summary",
                Font = new Font("Sagoe UI", 20, FontStyle.Bold),
                AutoSize = true,

            };

            _mainPanel.Controls.Add( _quizNameLabel );

            double total = _summaryData.Answers.Count;

            foreach (var q in _summaryData.Answers)
            {
                double qScore = CalculateScore(q);
                _score += qScore;

                var summaryItem = new SummaryQuestion(q, qScore);
                _mainPanel.Controls.Add(summaryItem);
            }

            _scoreLabel = new Label
            {
                Text = $"Twój wynik: {_score:0.##} / {total}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Padding = new Padding(10)
            };

            _confirmButton = new Button
            {
                Text = "Ok",
                Width = 500,
                FlatStyle = FlatStyle.Flat,
                Height = 50
            };

            _confirmButton.Click += (s, e) =>
            {
                BackToQuizzes();
            };

            _mainPanel.Controls.Add(_scoreLabel);
            _mainPanel.Controls.Add (_confirmButton);



            this.Controls.Add(_mainPanel);
        }

        private double CalculateScore(AnsweredQuestion q)
        {
            var correct = q.Answers.Where(a => a.IsCorrect).ToList();
            var selected = q.Answers.Where(a => a.IsSelected).ToList();

            if (q.QuestionType == "single")
            {
                return selected.Count == 1 && selected[0].IsCorrect ? 1.0 : 0.0;
            }

            int correctSelected = selected.Count(a => a.IsCorrect);
            int totalCorrect = correct.Count;

            return Math.Max(0, (double)correctSelected / totalCorrect);
        }

        private void BackToQuizzes()
        {
            this.Controls.Clear();
            _router.Navigate("dashboard");
        }

        public async void OnNavigatedTo(object parameter) {
            if(parameter is SummaryData summary)
            {
                _summaryData = summary;
                InitializeComponent();
                await _quizService.UpdateQuizStatistics(summary, _score);
            }
        }
    }

}
