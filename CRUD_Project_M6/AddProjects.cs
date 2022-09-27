using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUD_Project_M6
{
    public partial class AddProjects : Form
    {
        List<Project> project = new List<Project>();
        public AddProjects()
        {
            InitializeComponent();
        }
        public ICrossDataFormLoadSync SysncForm { get; set; }
        private void AddProjects_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.GetNewProjectId().ToString();
        }
        private int GetNewProjectId()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand scmd = new SqlCommand("SELECT ISNULL(MAX(projectid), 0) FROM projects", connection))
                {
                    connection.Open();
                    int id = (int)scmd.ExecuteScalar();
                    connection.Close();
                    return id + 1;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.ConString))
            {
                connection.Open();
                using (SqlTransaction stran = connection.BeginTransaction())
                {

                    using (SqlCommand scmd = new SqlCommand(@"INSERT INTO projects 
                                            (projectid, projectname, budget,isRunning, employeeid) VALUES
                                            (@i, @n, @b, @r, @e)", connection, stran))
                    {
                        scmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        scmd.Parameters.AddWithValue("@n", textBox2.Text);
                        scmd.Parameters.AddWithValue("@b", decimal.Parse(textBox3.Text));
                        scmd.Parameters.AddWithValue("@r", checkBox1.Text);
                        scmd.Parameters.AddWithValue("@e", textBox5.Text);
                        try
                        {
                            if (scmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                project.Add(new Project
                                {
                                    ProjectId = int.Parse(textBox1.Text),
                                    ProjectName = textBox2.Text,
                                    Budget = decimal.Parse(textBox3.Text),
                                    IsRunning = checkBox1.Checked,
                                    EmployeeId = int.Parse(textBox5.Text),
                                }); ;
                                stran.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            stran.Rollback();
                        }
                        finally
                        {
                            if (connection.State == ConnectionState.Open)
                            {
                                connection.Close();
                            }
                        }

                    }
                        
                }

            }
        }

        private void AddProjects_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SysncForm.ReloadProject(this.project);
        }
    }
 }
