using CRUD_Project_M6.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUD_Project_M6
{
    public partial class FormEmployeeRpt : Form
    {
        public FormEmployeeRpt()
        {
            InitializeComponent();
        }

        private void FormEmployeeRpt_Load(object sender, EventArgs e)
        {
            DataSet dset = new DataSet();
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM employees", connection))
                {
                    sda.Fill(dset, "employeei");
                    dset.Tables["employeei"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < dset.Tables["employeei"].Rows.Count; i++)
                    {
                        dset.Tables["employeei"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), dset.Tables["employeei"].Rows[i]["picture"].ToString()));
                    }
                    EmployeeRpt rpt = new EmployeeRpt();
                    rpt.SetDataSource(dset);
                    crystalReportViewer1.ReportSource = rpt;
                    rpt.Refresh();
                    crystalReportViewer1.Refresh();
                }
            }
        }
    }
}
