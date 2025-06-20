using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Interfaces;
using projekt.Routing;
using projekt.Styling;

namespace projekt.Forms.Components
{
    
    public class SearchItem : Button
    {
        private int _contentId;
        private string _name;
        private string _type;

        private TableLayoutPanel _mainLayout;
        private Label _typeLabel;
        private Label _nameLabel;

        private Router _router;
        private IAppStyle Style => AppStyle.Current;

        public SearchItem(Router router, int contentId, string name, string type)
        {
            _router = router;
            _contentId = contentId;
            _name = name.Length > 50 ? name.Substring(0, 50) + "..." : name;
            _type = type;

            InitializeComponent();

            AppStyle.StyleChanged += (s, e) => UpdateStyle();
            UpdateStyle();
        }

        private void InitializeComponent()
        {
            this.Width = 600;
            this.Height = 40;
            this.FlatStyle = FlatStyle.Flat;
            this.Cursor = Cursors.Hand;

            this.FlatAppearance.BorderSize = 0;

            _mainLayout = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                ColumnCount = 3,
            };

            _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _typeLabel = new Label
            {
                Text = _type,
                Font = new Font("Sagoe UI", 10F),
                Height = 20,
                Padding = new Padding(10, 0, 10, 0)
            };

            _nameLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = _name,
                Font = new Font("Sagoe UI", 10F),
                Height = 20,
                Padding = new Padding(10, 0, 10, 0)
            };

            _mainLayout.Controls.Add(_typeLabel, 0, 0);
            _mainLayout.Controls.Add(_nameLabel, 1, 0);

            this.Click += SearchItem_Click;
            _mainLayout.Click += SearchItem_Click;
            _typeLabel.Click += SearchItem_Click;
            _nameLabel.Click += SearchItem_Click;

            this.Controls.Add(_mainLayout);

        }

        private void SearchItem_Click(object? sender, EventArgs e)
        {
            _router.Navigate(_type == "user" ? "profile" : "solvequiz", _contentId);
        }

        private void UpdateStyle()
        {
            this.BackColor = Style.ElementBackground;
            _nameLabel.ForeColor = Style.Foreground;
            _typeLabel.ForeColor = Style.Foreground;
        }
    }
}
