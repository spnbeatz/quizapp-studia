using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Forms.Components;
using projekt.Interfaces;
using projekt.Styling;
using projekt.Models;
using projekt.Routing;

namespace projekt.Screens
{
    public class SearchScreen : UserControl, IRoutable
    {
        private FloatingLabelTextBox _searchInput;
        private TableLayoutPanel _mainLayout;
        private TableLayoutPanel _headerLayout;
        private Button _searchButton;
        private SearchList _searchList;

        private int _inputWidth = 400;

        private readonly ISearchService _searchService;
        private Router _router;
        private IAppStyle Style => AppStyle.Current;
        public SearchScreen(ISearchService searchService, Router router)
        {
            _searchService = searchService;
            _router = router;

            InitializeComponent();
            AppStyle.StyleChanged += (s, e) => UpdateStyle();
            UpdateStyle();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;

            _mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2
            };

            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _headerLayout = new TableLayoutPanel
            {
                Height = 70,
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 0, 10, 0)
            };


            _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));


            _searchInput = new FloatingLabelTextBox
            {
                Width = 1000,
                LabelText = "Szukaj",
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.None
            };

            _searchButton = new Button
            {
                Width = 60,
                Height = 60,
                FlatStyle = FlatStyle.Flat,
            };

            _searchButton.Click += (s, e) => OnSearchClick();

            _searchButton.FlatAppearance.BorderSize = 0;

            _headerLayout.Controls.Add(_searchInput, 0, 0);
            _headerLayout.Controls.Add(_searchButton, 1, 0);

            _searchList = new SearchList(_router)
            {
                Dock = DockStyle.Fill,
            };

            _mainLayout.Controls.Add(_headerLayout, 0, 0);
            _mainLayout.Controls.Add(_searchList, 0, 1);

            this.Controls.Add(_mainLayout);
        }

        private async void OnSearchClick()
        {
            // MessageBox.Show($"I am searching {_searchInput.TextValue}");
            List<SearchIndexItem> searchItems = await _searchService.SearchAsync(_searchInput.TextValue);
            _searchList.RenderItems(searchItems);
        }

        private void UpdateStyle()
        {
            _searchButton.Image = Style.searchButton;
        }
        public void OnNavigatedTo(object parameter) { }
    }
}
