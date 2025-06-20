using System;
using System.Drawing;
using System.Windows.Forms;
using projekt.Interfaces;
using projekt.Styling;

namespace projekt.Forms.Components
{
    public class FloatingLabelTextBox : UserControl
    {
        private Label floatingLabel;
        private TextBox textBox;
        private Panel textBoxContainer;

        private System.Windows.Forms.Timer animationTimer;
        private bool animatingUp;
        private float animationProgress;
        private const int animationDuration = 150;
        private DateTime animationStartTime;

        private Point labelStartPos = new Point(8, 20);
        private Point labelEndPos = new Point(8, 2);
        private float fontStartSize = 10f;
        private float fontEndSize = 8f;

        private IAppStyle Style => AppStyle.Current;

        private Color _labelBackColor = Color.White;
        public Color LabelBackColor
        {
            get => _labelBackColor;
            set
            {
                _labelBackColor = value;
                if (floatingLabel != null)
                    floatingLabel.BackColor = value;
            }
        }

        public string LabelText
        {
            get => floatingLabel.Text;
            set => floatingLabel.Text = value;
        }

        public Color TextBack
        {
            get => textBox.BackColor;
            set => textBox.BackColor = value;
        }

        public string TextValue
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }

        public bool Multiline
        {
            get => textBox.Multiline;
            set
            {
                textBox.Multiline = value;
                AdjustSizeForMultiline();
            }
        }

        public int InputHeight
        {
            get => textBox.Height;
            set
            {
                textBox.Height = value;
                AdjustControlHeight();
                UpdateTextBoxLayout();
            }
        }

        public char PasswordChar
        {
            get => textBox.PasswordChar;
            set => textBox.PasswordChar = value;
        }

        public bool UseSystemPasswordChar
        {
            get => textBox.UseSystemPasswordChar;
            set => textBox.UseSystemPasswordChar = value;
        }

        public FloatingLabelTextBox()
        {
            this.Width = 290;
            this.DoubleBuffered = true;
            this.BackColor = Color.Transparent;
            this.ForeColor = Color.DodgerBlue;

            textBoxContainer = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(this.Width, this.Height),
                BackColor = Color.Transparent,
            };

            textBox = new TextBox
            {
                Location = new Point(10, 20),
                Width = this.Width - 20,
                Height = 25,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 12F),
                AcceptsReturn = true,
                Multiline = false,
                ScrollBars = ScrollBars.None
            };
            textBox.GotFocus += TextBox_GotFocus;
            textBox.LostFocus += TextBox_LostFocus;
            textBox.TextChanged += TextBox_TextChanged;

            floatingLabel = new Label
            {
                Text = "Label",
                Location = labelStartPos,
                AutoSize = true,
                BackColor = this.BackColor,
                Font = new Font("Segoe UI", fontStartSize),
                BorderStyle = BorderStyle.None,
                Padding = new Padding(2, 0, 2, 0),
                Cursor = Cursors.IBeam
            };

            floatingLabel.MouseDown += (s, e) => textBox.Focus();

            textBoxContainer.Controls.AddRange(new Control[]
            {
                floatingLabel, textBox
            });

            this.Controls.Add(textBoxContainer);

            floatingLabel.BringToFront();

            animationTimer = new System.Windows.Forms.Timer
            {
                Interval = 1
            };
            animationTimer.Tick += AnimationTimer_Tick;

            this.Height = 50;

            UpdateTextBoxLayout();
            UpdateLabelPosition();

            AppStyle.StyleChanged += (s, e) => UpdateStyle();
            UpdateStyle();
        }

        private void AdjustSizeForMultiline()
        {
            if (Multiline)
            {
                InputHeight = 70;
                this.Height = InputHeight + 40;
            }
            else
            {
                InputHeight = 25;
                this.Height = 50;
            }
            UpdateTextBoxLayout();
        }

        private void AdjustControlHeight()
        {
            if (Multiline)
            {
                this.Height = InputHeight + 40;
            }
            else
            {
                this.Height = 50;
            }
        }

        private void UpdateTextBoxLayout()
        {
            if (textBoxContainer == null || textBox == null)
                return;

            textBoxContainer.Width = this.Width;
            textBoxContainer.Height = this.Height;

            textBox.Width = this.Width - 20;
            textBox.Location = new Point(10, this.Height - InputHeight - 10);
            textBox.Height = InputHeight;

            Invalidate();
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateTextBoxLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Color borderColor = Color.Gray;
            int borderRadius = 3;

            int borderHeight = this.Height - 10;
            int yOffset = this.Height - borderHeight;

            Rectangle borderRect = new Rectangle(
                0,
                yOffset,
                this.Width - 1,
                borderHeight - 1
            );

            using (var pen = new Pen(borderColor, 1.5f))
            using (var path = RoundedRect(borderRect, borderRadius))
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.DrawPath(pen, path);
            }
        }

        private System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();

            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            StartAnimation(true);
            this.Invalidate();
        }

        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.Text))
                StartAnimation(false);
            this.Invalidate();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                if (!textBox.Focused)
                {
                    if (animatingUp || animationProgress > 0)
                        StartAnimation(false);
                }
            }
            else
            {
                if (!(animatingUp && animationProgress == 1f))
                    StartAnimation(true);
            }
        }

        private void StartAnimation(bool up)
        {
            if (animatingUp == up && animationTimer.Enabled)
                return;

            animatingUp = up;
            animationStartTime = DateTime.Now;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            var elapsed = (DateTime.Now - animationStartTime).TotalMilliseconds;
            animationProgress = (float)(elapsed / animationDuration);

            if (animationProgress >= 1f)
            {
                animationProgress = 1f;
                animationTimer.Stop();
            }

            float progress = animatingUp ? animationProgress : 1f - animationProgress;

            int x = labelStartPos.X;
            int y = (int)(labelStartPos.Y + (labelEndPos.Y - labelStartPos.Y) * progress);
            float fontSize = fontStartSize + (fontEndSize - fontStartSize) * progress;

            floatingLabel.Location = new Point(x, y);
            floatingLabel.Font = new Font(floatingLabel.Font.FontFamily, fontSize);

            if (progress >= 0.95f)
            {
                floatingLabel.Padding = new Padding(4, 2, 4, 2);
                floatingLabel.BackColor = _labelBackColor;
            }
            else
            {
                floatingLabel.Padding = new Padding(2, 0, 2, 0);
                floatingLabel.BackColor = this.BackColor;
            }

            floatingLabel.Invalidate();
        }

        private void UpdateLabelPosition()
        {
            if (!string.IsNullOrEmpty(textBox.Text) || textBox.Focused)
                SetLabelPositionUp();
            else
                SetLabelPositionDown();
        }

        private void SetLabelPositionUp()
        {
            floatingLabel.Location = labelEndPos;
            floatingLabel.Font = new Font(floatingLabel.Font.FontFamily, fontEndSize);
            floatingLabel.BackColor = _labelBackColor;
        }

        private void SetLabelPositionDown()
        {
            floatingLabel.Location = labelStartPos;
            floatingLabel.Font = new Font(floatingLabel.Font.FontFamily, fontStartSize);
            floatingLabel.BackColor = _labelBackColor;
        }

        private void UpdateStyle()
        {
            this.TextBack = Style.Background;
            this.LabelBackColor = Style.Background;
            this.ForeColor = Style.Foreground;
            floatingLabel.ForeColor = Style.Foreground;
            textBox.ForeColor = Style.Foreground;
        }
    }
}
