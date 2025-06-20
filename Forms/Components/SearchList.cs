using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Models;
using projekt.Routing;

namespace projekt.Forms.Components
{
    public class SearchList : FlowLayoutPanel
    {
        private Router _router;
        public SearchList(Router router)
        {

            _router = router;
            InitializeComponent();

        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.FlowDirection = FlowDirection.TopDown;
            this.Padding = new Padding(10);
        }
        public void RenderItems(List<SearchIndexItem> items)
        {
            this.Controls.Clear();
            foreach (SearchIndexItem item in items) {
                this.Controls.Add(new SearchItem(_router, item.EntityId, item.Content, item.EntityType)
                {
                    Width = this.Width - 20
                });
            }
        }
    }

}
