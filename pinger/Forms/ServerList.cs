using System;
using System.Data.Sql;

namespace uping.Forms
{
    public partial class ServerList : TemplateFormList
    {
        public ServerList()
        {
            InitializeComponent();
        }

        private void ServerList_Load(object sender, EventArgs e)
        {
            dgv.AutoGenerateColumns = true;
            dgv.DataSource = SqlDataSourceEnumerator.Instance.GetDataSources();
            dgv.Refresh();
        }


    }
}
