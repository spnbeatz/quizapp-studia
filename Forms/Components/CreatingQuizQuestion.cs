using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using projekt.Forms.Components;
using projekt.Interfaces;
using projekt.Models;
using projekt.Styling;


namespace projekt.Forms.Components
{

    internal class CreatingQuizQuestion : FlowLayoutPanel
    {
        private int answerCount = 1;

        private FloatingLabelTextBox questionTextBox;
        private ComboBox questionTypeComboBox;
        private FlowLayoutPanel answersPanel;
        private Button addAnswerButton;

        public event EventHandler RemoveClicked;

        public string QuestionPlaceholder
        {
            get => questionTextBox.LabelText;
            set => questionTextBox.LabelText = value;
        }

        public string QuestionValue
        {
            get => questionTextBox.TextValue;
            set => questionTextBox.TextValue = value;
        }

        public string TypeValue {
            get => questionTypeComboBox.SelectedValue.ToString();
            set => questionTypeComboBox.SelectedValue = value;
        }
        private IAppStyle Style => AppStyle.Current;



        public CreatingQuizQuestion()
        {
            this.AutoSize = true;
            this.Width = 485;
            this.FlowDirection = FlowDirection.TopDown;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Padding = new Padding(10);
            this.Margin = new Padding(0,0,0,10);
            
            

            questionTextBox = new FloatingLabelTextBox
            {
                LabelText = "",
                Width = this.Width - 20,
                Height = 50,

            };

            answersPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false,
                Width = this.Width,
            };

            addAnswerButton = new Button
            {
                Text = "Dodaj odpowiedź",
                AutoSize = true,
                Width = this.Width - 20,
                Height = 30,
                               
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                FlatStyle = FlatStyle.Flat,
            };

            addAnswerButton.FlatAppearance.BorderSize = 1;
            addAnswerButton.FlatAppearance.BorderColor = Color.Gray;
            addAnswerButton.Click += (s, e) =>
            {
                answerCount++;
                AddAnswerControl(answerCount);
            };

            this.Controls.Add(questionTextBox);
            this.Controls.Add(answersPanel);
            this.Controls.Add(addAnswerButton);

            AddAnswerControl(answerCount);
            AppStyle.StyleChanged += (s, e) => UpdateStyle();
            UpdateStyle();
        }

        private void AddAnswerControl(int count)
        {
            var answerControl = new CreatingQuizAnswer();
            answerControl.AnswerText = "Odpowiedź " + count.ToString();
            answerControl.RemoveClicked += (s, e) =>
            {
                answersPanel.Controls.Remove((Control)s);
                (s as Control)?.Dispose();

                // Aktualizuj numerację po usunięciu
                ReindexAnswers();
            };
            answersPanel.Controls.Add(answerControl);
        }

        private void ReindexAnswers()
        {
            int index = 1;
            foreach (CreatingQuizAnswer answerControl in answersPanel.Controls.OfType<CreatingQuizAnswer>())
            {
                answerControl.AnswerText = "Odpowiedź " + index;
                index++;
            }
            answerCount = index - 1; // Aktualna liczba odpowiedzi
        }

        public List<Answer> GetAnswers()
        {
            return answersPanel.Controls
                .OfType<CreatingQuizAnswer>()
                .Select(ac => new Answer
                {
                    Text = ac.Value,
                    IsCorrect = ac.IsCorrect
                })
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .ToList();
        }

        public void SetAnswers(List<Answer> answers)
        {
            answersPanel.Controls.Clear();
            answerCount = 0;

            foreach (var answer in answers)
            {
                answerCount++;
                var answerControl = new CreatingQuizAnswer
                {
                    AnswerText = "Odpowiedź " + answerCount,
                    Value = answer.Text,
                    IsCorrect = answer.IsCorrect
                };

                answerControl.RemoveClicked += (s, e) =>
                {
                    answersPanel.Controls.Remove((Control)s);
                    (s as Control)?.Dispose();
                    ReindexAnswers();
                };

                answersPanel.Controls.Add(answerControl);
            }
        }


        private void UpdateStyle()
        {
            this.BackColor = Style.Background;
            this.ForeColor = Style.Foreground;
        }
    }
}
