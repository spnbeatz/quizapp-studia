using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Models;

namespace projekt.Forms.Components
{

    public class QuestionItem : UserControl
    {
        private TableLayoutPanel questionContainer;
        private Label questionLabel;
        private FlowLayoutPanel answersPanel;
        public Question _question;
        public List<Answer> answerList;

        private List<Control> _answerControls = new List<Control>();
        public QuestionItem(Question question)
        {
            _question = question;
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            this.AutoSize = true;

            questionContainer = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(50,20, 50, 20),
                Margin = new Padding(10),
                Width = 400,
                RowCount = 2,
            };

            questionContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            questionContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            questionLabel = new Label
            {
                Text = _question.Text,
                Width = 380,
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                MinimumSize = new Size(380, 0),
                MaximumSize = new Size(380, 0)

            };

            answersPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Margin = new Padding(0, 10, 0, 0),
            };

            Random rng = new Random();
            answerList = _question.Answers.OrderBy(a => rng.Next()).ToList();

            foreach (Answer answer in answerList)
            {
                Control answerControl = _question.QuestionType == "single"
                    ? new RadioButton()
                    : new CheckBox();

                answerControl.Text = answer.Text;
                answerControl.Tag = answer;
                answerControl.AutoSize = true;
                _answerControls.Add(answerControl);
                answersPanel.Controls.Add(answerControl);
            }

            questionContainer.Controls.Add(questionLabel, 0, 0);
            questionContainer.Controls.Add(answersPanel, 0, 1);

            this.Controls.Add(questionContainer);
        }

        public bool HasAnyAnswerSelected()
        {
            return _answerControls.Any(c =>
                (c is RadioButton rb && rb.Checked) ||
                (c is CheckBox cb && cb.Checked));
        }

        public List<Answer> GetSelectedAnswers()
        {
            var selected = new List<Answer>();

            foreach (Control c in _answerControls)
            {
                if ((c is RadioButton rb && rb.Checked) ||
                    (c is CheckBox cb && cb.Checked))
                {
                    if (c.Tag is Answer answer)
                    {
                        selected.Add(answer);
                    }
                }
            }

            return selected;
        }

        public bool IsAnswerCorrect()
        {
            var selected = GetSelectedAnswers();
            var correct = _question.Answers.Where(a => a.IsCorrect).ToList();

            return selected.Count == correct.Count &&
                   selected.All(a => correct.Any(c => c.Text == a.Text));
        }

        public double GetScore()
        {
            var selected = GetSelectedAnswers();
            var correctAnswers = _question.Answers.Where(a => a.IsCorrect).ToList();
            var incorrectAnswers = _question.Answers.Where(a => !a.IsCorrect).ToList();

            if (_question.QuestionType == "single")
            {
                return selected.Count == 1 && selected[0].IsCorrect ? 1.0 : 0.0;
            }

            int correctlySelected = selected.Count(a => a.IsCorrect);
            int incorrectlySelected = selected.Count(a => !a.IsCorrect);
            int totalCorrect = correctAnswers.Count;

            double score = (double)correctlySelected / totalCorrect;

            return Math.Max(0, Math.Min(score, 1.0));
        }


    }

}
