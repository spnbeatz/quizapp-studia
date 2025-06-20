using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Models;

namespace projekt.Forms.Components
{
    public class SummaryQuestion : UserControl
    {
        private AnsweredQuestion _answeredQuestion;
        private TableLayoutPanel _mainLayout;
        private Label _questionLabel;
        private FlowLayoutPanel _answersPanel;
        private double _qScore;
        private Label _score;

        public SummaryQuestion(AnsweredQuestion answeredQuestion, double qScore)
        {
            _answeredQuestion = answeredQuestion;
            _qScore = qScore;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            _mainLayout = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(20),
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle,
                RowCount = 3,
                Width = 500
            };

            _questionLabel = new Label
            {
                Text = _answeredQuestion.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false,
                Width = 460,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _answersPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Margin = new Padding(0, 10, 0, 0)
            };

            _score = new Label
            {
                Text = $"Wynik: {_qScore:0.##} / 1",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                AutoSize = true,
                Margin = new Padding(5, 10, 0, 0)
            };

            foreach (var ans in _answeredQuestion.Answers)
            {
                Control answerControl = _answeredQuestion.QuestionType == "single"
                    ? new RadioButton()
                    : new CheckBox();

                answerControl.Text = ans.Text;
                answerControl.AutoSize = true;
                if (answerControl is RadioButton rb)
                    rb.Checked = ans.IsSelected;
                else if (answerControl is CheckBox cb)
                    cb.Checked = ans.IsSelected;

                if (ans.IsCorrect && ans.IsSelected)
                    answerControl.ForeColor = Color.LightGreen;
                    
                else if (!ans.IsCorrect && ans.IsSelected)
                    answerControl.ForeColor = Color.IndianRed;
                else
                    answerControl.BackColor = Color.Transparent;

                _answersPanel.Controls.Add(answerControl);
            }

            _mainLayout.Controls.Add(_questionLabel, 0, 0);
            _mainLayout.Controls.Add(_answersPanel, 0, 1);
            _mainLayout.Controls.Add(_score, 0, 2);

            this.Controls.Add(_mainLayout);
        }
    }
}
