using System;
using System.Drawing;
using System.Windows.Forms;

namespace projekt.Forms.Components
{
    public class CreatingQuizAnswer : UserControl
    {
        private CheckBox correctCheckBox;
        private FloatingLabelTextBox answerTextBox;
        private Button removeButton;
        

        public event EventHandler RemoveClicked;

        public bool IsCorrect
        {
            get => correctCheckBox.Checked;
            set => correctCheckBox.Checked = value;
        }

        public string AnswerText
        {
            get => answerTextBox.LabelText;
            set => answerTextBox.LabelText = value;
        }

        public string Value
        {
            get => answerTextBox.TextValue;
            set => answerTextBox.TextValue = value;
        }

        public CreatingQuizAnswer()
        {
            this.Height = 55;
            this.Width = 460;

            correctCheckBox = new CheckBox
            {
                Location = new Point(445, 15),
                Size = new Size(15, 15),
                
            };

            answerTextBox = new FloatingLabelTextBox
            {

                Width = this.Width - 45,
                Height = 50,
            };

            removeButton = new Button
            {
                Text = "🗑️",
                Font = new Font("Segoe UI Emoji", 8),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(16, 25),
                BackColor = Color.Transparent,
                ForeColor = Color.Red,
                Cursor = Cursors.Hand,
                TabStop = false,
                Location = new Point(444, 30),
            };
            removeButton.FlatAppearance.BorderSize = 0;
            removeButton.Click += RemoveButton_Click;
            removeButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            removeButton.FlatAppearance.MouseDownBackColor = Color.Transparent;

            this.Controls.Add(correctCheckBox);
            this.Controls.Add(answerTextBox);
            this.Controls.Add(removeButton);
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            RemoveClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
