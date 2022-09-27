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
    public partial class EditDeleteProjects : Form
    {
        
        string action = "Edit";
        Project project;
        public EditDeleteProjects()
        {
            InitializeComponent();
        }
        public int ProjectToEditDelete { get; set; }
        public ICrossDataFormLoadSync ReloadToSync { get; set; }
        private void EditDeleteProjects_Load(object sender, EventArgs e)
        {
            DataShowing();
        }
        private void DataShowing()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand scmd = new SqlCommand("SELECT * FROM projects WHERE projectid =@i", connection))
                {
                    scmd.Parameters.AddWithValue("@i", this.ProjectToEditDelete);
                    connection.Open();
                    var dr = scmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetInt32(0).ToString();
                        textBox2.Text = dr.GetString(1);
                        textBox3.Text = dr.GetDecimal(2).ToString("0.00");
                        checkBox1.Checked = dr.GetBoolean(3);
                        textBox4.Text = dr.GetInt32(4).ToString();
                    }
                    connection.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.action = "Edit";
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.ConString))
            {
                connection.Open();
                using (SqlTransaction stran = connection.BeginTransaction())
                {

                    using (SqlCommand scmd = new SqlCommand(@"UPDATE  projects  
                                            SET  projectname=@n, budget=@b, isRunning= @r, employeeid=@ei 
                                            WHERE projectid=@i", connection, stran))
                    {
                        scmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        scmd.Parameters.AddWithValue("@n", textBox2.Text);
                        scmd.Parameters.AddWithValue("@b", textBox3.Text);
                        scmd.Parameters.AddWithValue("@r", checkBox1.Checked);
                        scmd.Parameters.AddWithValue("@ei", textBox4.Text);                        
                        try
                        {
                            if (scmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                project = new Project
                                {
                                    ProjectId = int.Parse(textBox1.Text),
                                    ProjectName = textBox2.Text,
                                    Budget =decimal.Parse( textBox3.Text),
                                    IsRunning = checkBox1.Checked,
                                    EmployeeId = int.Parse(textBox4.Text),                                    
                                };
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

        private void button4_Click(object sender, EventArgs e)
        {
            this.action = "Delete";
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.ConString))
            {
                connection.Open();
                using (SqlTransaction stran = connection.BeginTransaction())
                {

                    using (SqlCommand scmd = new SqlCommand(@"DELETE  projects  
                                            WHERE projectid=@i", connection, stran))
                    {
                        scmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));




                        try
                        {
                            if (scmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Deleted", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void button3_Click(object sender, EventArgs e)
        {
            DataShowing();
        }

        private void EditDeleteProjects_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.action == "edit")
                this.ReloadToSync.UpdateProject(project);
            else
                this.ReloadToSync.RemoveProject(Int32.Parse(this.textBox1.Text));
        }
    }
    
}
