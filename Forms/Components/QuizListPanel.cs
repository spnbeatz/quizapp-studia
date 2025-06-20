using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Events;
using projekt.Interfaces;
using projekt.Models;
using projekt.Routing;
using projekt.Screens;
using projekt.Utils;

namespace projekt.Forms.Components
{
    public class QuizListPanel : FlowLayoutPanel
    {

        private readonly Router _router;
        private readonly IQuizService _quizService;

        private List<QuizSummaryItem> _quizList = new();
        public QuizListPanel(IQuizService quizService, Router router, int? userId)
        {
            _quizService = quizService;
            _router = router;

            InitializeComponent();
            QuizEvents.QuizAdded += (s, e) => LoadQuizzesAsync(userId);
            QuizEvents.QuizRemoved += (s, e) => RemoveQuiz(e.QuizId);
            LoadQuizzesAsync(userId);
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.FlowDirection = FlowDirection.LeftToRight;
            this.WrapContents = false;
            this.AutoScroll = true;
            this.Padding = new Padding(10);
        }

        private async Task LoadQuizzesAsync(int? userId)
        {
            try
            {
                _quizList = await _quizService.GetListItemData(userId);

                this.Controls.Clear();


                foreach (var quiz in _quizList)
                {
                    var item = new QuizListItem(_router, quiz.AuthorId)
                    {
                        Title = quiz.Label,
                        Description = quiz.Description,
                        AuthorName = quiz.Username,
                        QuestionCount = $"{quiz.QuestionCount} pytań",
                        Width = 400,
                        Height = 400,
                        quizId = quiz.Id,
                    };
                    this.Controls.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas ładowania quizów:\n" + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RemoveQuiz(int quizId)
        {
            var confirm = MessageBox.Show(
                $"Czy na pewno chcesz usunąć quiz?",
                "Potwierdzenie usunięcia",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes) {
                bool result = await _quizService.DeleteQuizAsync(quizId);
            } 

            
        }
    }
}
