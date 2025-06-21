using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Models;

namespace projekt.Forms.Components
{
    public class QuizStatsPanel : TableLayoutPanel
    {
        private Label _timesSolved;
        private Label _earnedPoints;
        private Label _averageScore;

        private QuizStats _stats;
        public QuizStatsPanel(QuizStats stats)
        {
            _stats = stats;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.ColumnCount = 3;
            this.Height = 30;

            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));


            _timesSolved = new Label
            {
                Dock = DockStyle.Fill,
                Text = $"Rozwiązano {_stats.TimesSolved} razy",
                Font = new Font("Segoe UI", 10F)
            };

            _earnedPoints = new Label
            {
                Dock = DockStyle.Fill,
                Text = $"Zdobyto łącznie {_stats.EarnedPoints:F2} na {_stats.TotalPoints} punktów",
                Font = new Font("Segoe UI", 10F)
            };

            _averageScore = new Label
            {
                Dock = DockStyle.Fill,
                Text = $"Średni wynik: {_stats.AverageScore:F2}",
                Font = new Font("Segoe UI", 10F)
            };

            this.Controls.Add( _timesSolved, 0, 0 );
            this.Controls.Add(_earnedPoints, 1 ,0 );
            this.Controls.Add(_averageScore, 2, 0 );  
        }
    }
}
