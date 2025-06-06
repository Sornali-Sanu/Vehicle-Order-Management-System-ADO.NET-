using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehicleOrderMasterDetail.ViewModel;

namespace VehicleOrderMasterDetail
{
    public partial class ReportForm : Form
    {
        private List<VehicleViewModel> localList;
        public ReportForm(List<VehicleViewModel> list)
        {
            InitializeComponent();
            localList = list;
        }
        public ReportForm()
        {
            InitializeComponent();
        }

        private void ReportViwer_Load(object sender, EventArgs e)
        {
            VehicleOrderManagement rpt = new VehicleOrderManagement();
            rpt.SetDataSource(localList);
            ReportViwer.ReportSource = rpt;
            ReportViwer.Refresh();

        }
    }
}
