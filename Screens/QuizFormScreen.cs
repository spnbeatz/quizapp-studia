using System;
using System.Drawing;
using System.Windows.Forms;
using projekt.Events;
using projekt.Forms.Components;
using projekt.Interfaces;
using projekt.Models;
using projekt.Routing;
using projekt.Styling;
using projekt.Utils;

namespace projekt.Screens
{
    public class QuizFormScreen : UserControl, IRoutable
    {
        private Label label;
        private FloatingLabelTextBox quizName;
        private FloatingLabelTextBox quizDescription;
        private Button confirmButton;
        private Button cancelButton;
        private FlowLayoutPanel leftPanel;
        private FlowLayoutPanel mainPanel;
        private FlowLayoutPanel rightPanel;
        private TableLayoutPanel headerPanel;
        private Panel bodyPanel;

        private TableLayoutPanel buttonsPanel;
        private Button addQuestionButton;
        private FlowLayoutPanel questionsPanel;
        private int? editingQuizId = null;


        private IQuizService _quizService;
        private Router _router;
        private IAppStyle Style => AppStyle.Current;

        public event EventHandler? QuizAddedEvent;

        private int questionCount = 1;

        public QuizFormScreen(IQuizService quizService, Router router)
        {
            _quizService = quizService;
            _router = router;   
            this.Dock = DockStyle.Fill;
            this.AutoScroll = true;
            this.Padding = new Padding(10, 0, 10, 0);

            InitializeComponent();

            confirmButton.Click += (s, e) => SendQuizData();

            addQuestionButton.Click += (s, e) =>
            {
                questionCount++;
                AddQuestionControl(questionCount);
            };

            AppStyle.StyleChanged += (s, e) => UpdateStyle();
            UpdateStyle();
        }

        private void InitializeComponent()
        {
            headerPanel = new TableLayoutPanel
            {
                ColumnCount = 3,
                AutoSize = true,
                Dock = DockStyle.Top,
                Padding = new Padding(0, 0, 10, 0),
            };
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            label = new Label
            {
                Text = "Stwórz Quiz",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                TextAlign = ContentAlignment.BottomLeft,
            };

            buttonsPanel = new TableLayoutPanel
            {
                ColumnCount = 3,
                AutoSize = true,
                Width = 300,
                Dock = DockStyle.Right,
            };
            buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            confirmButton = new Button
            {
                Width = 40,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
            };
            confirmButton.FlatAppearance.BorderSize = 0;
            confirmButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            confirmButton.FlatAppearance.MouseDownBackColor = Color.Transparent;

            cancelButton = new Button
            {
                Width = 40,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
            };

            cancelButton.Click += (s, e) => OnCancel_Click();

            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cancelButton.FlatAppearance.MouseDownBackColor = Color.Transparent;

            buttonsPanel.Controls.Add(confirmButton, 0, 0);
            buttonsPanel.Controls.Add(cancelButton, 2, 0);

            headerPanel.Controls.Add(label, 0, 0);
            headerPanel.Controls.Add(buttonsPanel, 2, 0);

            bodyPanel = new Panel
            {
                Padding = new Padding(10, 45, 0, 0),
                Dock = DockStyle.Fill,
                AutoSize = true,
            };

            leftPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Left,
                FlowDirection = FlowDirection.TopDown,
            };

            quizName = new FloatingLabelTextBox
            {
                LabelText = "Nazwa quizu",
                Width = 380,
                Height = 50,
            };

            quizDescription = new FloatingLabelTextBox
            {
                LabelText = "Opis quizu",
                Width = 380,
                Height = 100,
                Multiline = true,
                InputHeight = 100,
            };

            leftPanel.Controls.Add(quizName);
            leftPanel.Controls.Add(quizDescription);

            rightPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0, 10, 15, 0),
            };

            rightPanel.HorizontalScroll.Enabled = false;
            rightPanel.HorizontalScroll.Visible = false;
            rightPanel.HorizontalScroll.Maximum = 0;

            questionsPanel = new FlowLayoutPanel
            {
                Width = 495,
                AutoSize = true,
                BackColor = Color.Transparent,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
            };

            addQuestionButton = new Button
            {
                AutoSize = true,
                Width = 490,
                Height = 30,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                Text = "Dodaj pytanie",
            };
            addQuestionButton.FlatAppearance.BorderSize = 1;

            rightPanel.Controls.Add(questionsPanel);
            rightPanel.Controls.Add(addQuestionButton);

            bodyPanel.Controls.Add(leftPanel);
            bodyPanel.Controls.Add(rightPanel);

            this.Controls.Add(headerPanel);
            this.Controls.Add(bodyPanel);

            AddQuestionControl(questionCount);
        }

        private void OnCancel_Click()
        {
            var result = MessageBox.Show(
                "Czy na pewno chcesz porzucić zmiany?",
                "Potwierdzenie",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                ResetForms();
                _router.Back();
            }
            else
            {
                // Użytkownik kliknął "Nie"
            }
        }

        private void ResetForms()
        {
            quizName.TextValue = "";
            quizDescription.TextValue = "";
            questionsPanel.Controls.Clear();
            questionCount = 1;
            AddQuestionControl(questionCount);
        }


        private void AddQuestionControl(int count)
        {
            var question = new CreatingQuizQuestion();
            question.QuestionPlaceholder = "Pytanie nr " + count.ToString();
            question.RemoveClicked += (s, e) =>
            {
                questionsPanel.Controls.Remove((Control)s);
                (s as Control)?.Dispose();
                ReindexAnswers();
            };
            questionsPanel.Controls.Add(question);
        }

        private void ReindexAnswers()
        {
            int index = 1;
            foreach (CreatingQuizAnswer answerControl in questionsPanel.Controls.OfType<CreatingQuizAnswer>())
            {
                answerControl.AnswerText = "Odpowiedź " + index;
                index++;
            }
            questionCount = index - 1;
        }

        private Quiz GetQuizData()
        {
            List<Question> questionList = new List<Question>();

            foreach (CreatingQuizQuestion quizQuestion in questionsPanel.Controls.OfType<CreatingQuizQuestion>())
            {
                List<Answer> answerList = quizQuestion.GetAnswers();
                int correctCount = answerList.Count(a => a.IsCorrect);

                if(correctCount == 0)
                {
                    MessageBox.Show("Bład");
                    return null;
                }

                questionList.Add(new Question
                {
                    Text = quizQuestion.QuestionValue,
                    QuestionType = correctCount > 1 ? "multiple" : "single",
                    Answers = answerList

                });
            }

            Quiz quizData = new Quiz
            {
                Title = quizName.TextValue,
                Description = quizDescription.TextValue,
                Level = "easy",
                Questions = questionList
            };

            return quizData;
        }

        private async Task SendQuizData()
        {
            var result = MessageBox.Show("Czy na pewno chcesz zapisać quiz?", "Potwierdzenie zapisu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Label statusLabel = new Label
                {
                    Text = "Zapisywanie...",
                    AutoSize = true,
                    ForeColor = Color.DarkBlue,
                    Location = new Point(20, 20),
                    Font = new Font("Segoe UI", 10, FontStyle.Italic)
                };
                this.Controls.Add(statusLabel);
                statusLabel.BringToFront();

                Quiz quizData = GetQuizData();

                if (quizData == null) return;

                try
                {
                    if(editingQuizId != null)
                    {
                        await _quizService.UpdateQuizAsync(quizData, editingQuizId);
                    } else
                    {
                        await _quizService.CreateQuizAsync(quizData);
                    }
                    

                    MessageBox.Show("Quiz został zapisany pomyślnie!", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    QuizEvents.NotifyQuizAdded();
                    _router.Navigate("dashboard");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas zapisu: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    ResetForms();
                    this.Controls.Remove(statusLabel);
                }
            }

        }

        private void PopulateQuizData(Quiz quiz)
        {
            quizName.TextValue = quiz.Title;
            quizDescription.TextValue = quiz.Description;

            questionsPanel.Controls.Clear();
            questionCount = 0;

            foreach (var question in quiz.Questions)
            {
                questionCount++;
                var questionControl = new CreatingQuizQuestion();
                questionControl.QuestionValue = question.Text;
                questionControl.QuestionPlaceholder = "Pytanie nr " + questionCount;

                questionControl.SetAnswers(question.Answers);

                questionControl.RemoveClicked += (s, e) =>
                {
                    questionsPanel.Controls.Remove((Control)s);
                    (s as Control)?.Dispose();
                    ReindexAnswers();
                };

                questionsPanel.Controls.Add(questionControl);
            }
        }

        private void UpdateStyle()
        {
            this.BackColor = Style.Background;
            this.ForeColor = Style.Foreground;

            label.ForeColor = Style.Foreground;

            addQuestionButton.ForeColor = Style.Foreground;
            addQuestionButton.FlatAppearance.BorderColor = Style.Foreground;

            confirmButton.Image = Style.confirmButton;

            cancelButton.Image = Style.cancelButton;
        }


        public async void OnNavigatedTo(object parameter) {
            if (parameter is int quizId)
            {
                editingQuizId = quizId;
                label.Text = "Edytuj Quiz";

                // Pobierz dane quizu i wypełnij formularz
                Quiz existingQuiz = await _quizService.GetQuizByIdAsync(quizId);
                PopulateQuizData(existingQuiz);
            }
            else
            {
                editingQuizId = null;
                label.Text = "Stwórz Quiz";
            }
        }

    }
}

