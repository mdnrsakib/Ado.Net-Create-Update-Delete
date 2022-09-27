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
    public partial class FromMain : Form,ICrossDataFormLoadSync
    {
        DataSet das;
        BindingSource esEmployees = new BindingSource();
        BindingSource esProjects = new BindingSource();
        public FromMain()
        {
            InitializeComponent();
        }
        public void DataLoad()
        {
            das = new DataSet();
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM employees", connection))
                {
                    sda.Fill(das, "employees");
                    das.Tables["employees"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < das.Tables["employees"].Rows.Count; i++)
                    {
                        das.Tables["employees"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), das.Tables["employees"].Rows[i]["picture"].ToString()));
                    }
                    sda.SelectCommand.CommandText = "SELECT * FROM projects";
                    sda.Fill(das, "projects");
                    DataRelation rel = new DataRelation("FK_EMPLOYEE_PROJECT",
                        das.Tables["employees"].Columns["employeeid"],
                        das.Tables["projects"].Columns["employeeid"]);
                    das.Relations.Add(rel);

                    das.AcceptChanges();
                }
            }
        }
         private void DataBind()
         {
            esEmployees.DataSource = das;
            esEmployees.DataMember = "employees";
            esProjects.DataSource = esEmployees;
            esProjects.DataMember = "FK_EMPLOYEE_PROJECT";
            this.dataGridView1.DataSource = esProjects;
            lblName.DataBindings.Add(new Binding("Text", esEmployees, "name"));
            dbDate.DataBindings.Add(new Binding("Text", esEmployees, "joiningdate",true));
            lblSalary.DataBindings.Add(new Binding("Text", esEmployees, "salary"));
            lblAddress.DataBindings.Add(new Binding("Text", esEmployees, "address"));
            lblPhone.DataBindings.Add(new Binding("Text", esEmployees, "phone"));
            checkBox1.DataBindings.Add(new Binding("Checked", esEmployees, "isWorking"));
            pictureBox1.DataBindings.Add(new Binding("Image", esEmployees, "image", true));
        }

        private void FromMain_Load(object sender, EventArgs e)
        {
            this.dataGridView1.AutoGenerateColumns = false;
            DataLoad();
            DataBind();
            showNavigation();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AddEmployee() { ReloadFrom = this }.ShowDialog();
        }

        public void ReloadData(List<Employee> employees)
        {
            foreach (var e in employees)
            {
                DataRow dr = das.Tables["employees"].NewRow();
                dr[0] = e.EmployeeId;
                dr["name"] = e.Name;
                dr["joiningdate"] = e.JoinDate;
                dr["salary"] = e.Salary;
                dr["address"] = e.Address;
                dr["phone"] =e.Phone;
                dr["isWorking"] = e.IsWorking;
                dr["picture"] = e.Picture;
                dr["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), e.Picture));
                das.Tables["employees"].Rows.Add(dr);

            }
            das.AcceptChanges();
            esEmployees.MoveLast();
        }

        public void UpdateEmployee(Employee employees)
        {
            for (var i = 0; i < das.Tables["employees"].Rows.Count; i++)
            {
                if ((int)das.Tables["employees"].Rows[i]["employeeid"] == employees.EmployeeId)
                {
                    das.Tables["employees"].Rows[i]["name"] = employees.Name;
                    das.Tables["employees"].Rows[i]["joiningdate"] = employees.JoinDate;
                    das.Tables["employees"].Rows[i]["salary"] = employees.Salary;
                    das.Tables["employees"].Rows[i]["address"] = employees.Address;
                    das.Tables["employees"].Rows[i]["phone"] = employees.Phone;
                    das.Tables["employees"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), employees.Picture));
                    break;
                }
            }
            das.AcceptChanges();
        }

        public void RemoveEmployee(int id)
        {
            for (var i = 0; i < das.Tables["employees"].Rows.Count; i++)
            {
                if ((int)das.Tables["employees"].Rows[i]["employeeid"] == id)
                {
                    das.Tables["employees"].Rows.RemoveAt(i);
                    break;
                }
            }
            das.AcceptChanges();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            esEmployees.MoveLast();
            showNavigation();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (esEmployees.Position < esEmployees.Count - 1)
            {
                esEmployees.MoveNext();
                showNavigation();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (esEmployees.Position > 0)
            {
                esEmployees.MovePrevious();
                showNavigation();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            esEmployees.MoveFirst();
            showNavigation();
        }
        private void showNavigation()
        {
            this.lblOf.Text = (esEmployees.Position + 1).ToString();
            this.lblTotal.Text = esEmployees.Count.ToString();
        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = (int)(this.esEmployees.Current as DataRowView).Row[0];
            new EditEmployees { EmployeeToEditDelete = id, ReloadingFrom = this }.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            new AddEmployee() { ReloadFrom = this }.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int id = (int)(this.esEmployees.Current as DataRowView).Row[0];
            new EditEmployees { EmployeeToEditDelete = id, ReloadingFrom = this }.ShowDialog();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            new AddProjects { SysncForm = this }.ShowDialog();
        }

        public void ReloadProject(List<Project> project)
        {
            foreach (var e in project)
            {
                DataRow dr = das.Tables["projects"].NewRow();
                dr[0] = e.ProjectId;
                dr["projectname"] = e.ProjectName;
                dr["budget"] = e.Budget;
                dr["isRunning"] = e.IsRunning;
                dr["employeeid"] = e.EmployeeId;
                das.Tables["projects"].Rows.Add(dr);

            }
            das.AcceptChanges();
            esEmployees.MoveLast();
        }

        public void UpdateProject(Project p)
        {
            for (var i = 0; i < das.Tables["projects"].Rows.Count; i++)
            {
                if ((int)das.Tables["projects"].Rows[i]["projectid"] == p.ProjectId)
                {
                    das.Tables["employees"].Rows[i]["name"] = p.ProjectName;
                    das.Tables["employees"].Rows[i]["joiningdate"] = p.Budget;
                    das.Tables["employees"].Rows[i]["salary"] = p.IsRunning;
                    das.Tables["employees"].Rows[i]["address"] = p.EmployeeId;
                    break;
                }
            }
            das.AcceptChanges();
        }

        public void RemoveProject(int id)
        {
            for (var i = 0; i < das.Tables["projects"].Rows.Count; i++)
            {
                if ((int)das.Tables["projects"].Rows[i]["projectid"] == id)
                {
                    das.Tables["projects"].Rows.RemoveAt(i);
                    break;
                }
            }
            das.AcceptChanges();
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            int id = (int)(this.esProjects.Current as DataRowView).Row[0];
            new EditDeleteProjects {ProjectToEditDelete  = id, ReloadToSync = this }.ShowDialog();
        }

        private void employeesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormEmployeeRpt().ShowDialog();
        }
        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void employeesProjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormEmployeeGroupRpt().ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
