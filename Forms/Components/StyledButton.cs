using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class StyledButton : Button
{
    public int CornerRadius { get; set; } = 15;
    public Color BorderColor { get; set; } = Color.Transparent;
    public int BorderSize { get; set; } = 1;
    public Color CardBackColor { get; set; } = Color.White;

    public StyledButton()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint |
                 ControlStyles.DoubleBuffer, true);

        BackColor = Color.Transparent;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

        using (GraphicsPath path = GetRoundedRectPath(rect, CornerRadius))
        {
            using (SolidBrush brush = new SolidBrush(CardBackColor))
            {
                e.Graphics.FillPath(brush, path);
            }
            if (BorderSize > 0)
            {
                using (Pen borderPen = new Pen(BorderColor, BorderSize))
                {
                    e.Graphics.DrawPath(borderPen, path);
                }
            }

            TextRenderer.DrawText(
                e.Graphics,
                this.Text,
                this.Font,
                rect,
                this.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
            );

            this.Region = new Region(path);
 
        }
    }

    private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
    {
        int diameter = radius * 2;
        GraphicsPath path = new GraphicsPath();
        path.StartFigure();
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}


