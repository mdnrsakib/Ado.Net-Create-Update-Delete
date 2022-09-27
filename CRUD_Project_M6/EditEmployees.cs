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
    public partial class EditEmployees : Form
    {
        string filePath, oldFile, fileName;
        string action = "Edit";
        Employee employee;
        public EditEmployees()
        {
            InitializeComponent();
        }
        public int EmployeeToEditDelete { get; set; }
        public ICrossDataFormLoadSync ReloadingFrom { get; set; }

        private void button3_Click(object sender, EventArgs e)
        {
            DataShowing();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.action = "Edit";
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.ConString))
            {
                connection.Open();
                using (SqlTransaction stran = connection.BeginTransaction())
                {

                    using (SqlCommand scmd = new SqlCommand(@"UPDATE  employees  
                                            SET  name=@n, joiningdate=@d, salary= @s, address=@a,phone=@p,isWorking=@w, picture=@pi 
                                            WHERE employeeid=@i", connection, stran))
                    {
                        scmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        scmd.Parameters.AddWithValue("@n", textBox2.Text);
                        scmd.Parameters.AddWithValue("@d", dateTimePicker1.Value);
                        scmd.Parameters.AddWithValue("@s", decimal.Parse(textBox3.Text));
                        scmd.Parameters.AddWithValue("@a", textBox4.Text);
                        scmd.Parameters.AddWithValue("@p", textBox5.Text);
                        scmd.Parameters.AddWithValue("@w", checkBox1.Checked);
                        if (!string.IsNullOrEmpty(this.filePath))
                        {
                            string ext = Path.GetExtension(this.filePath);
                            fileName = $"{Guid.NewGuid()}{ext}";
                            string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                            File.Copy(filePath, savePath, true);
                            scmd.Parameters.AddWithValue("@pi", fileName);
                        }
                        else
                        {
                            scmd.Parameters.AddWithValue("@pi", oldFile);
                        }


                        try
                        {
                            if (scmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                employee = new Employee
                                {
                                    EmployeeId = int.Parse(textBox1.Text),
                                    Name = textBox2.Text,
                                    JoinDate = dateTimePicker1.Value,
                                    Salary = decimal.Parse(textBox3.Text),
                                    Address = textBox4.Text,
                                    Phone = textBox5.Text,
                                    IsWorking = checkBox1.Checked,
                                    Picture = filePath == "" ? oldFile : fileName
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.label7.Text = Path.GetFileName(this.filePath);
                this.pictureBox1.Image = Image.FromFile(this.filePath);
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

                    using (SqlCommand scmd = new SqlCommand(@"DELETE  employees  
                                            WHERE employeeid=@i DELETE FROM [employees]
                                    WHERE employeeid = @i;", connection, stran))
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

        private void EditEmployees_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.action == "edit")
                this.ReloadingFrom.UpdateEmployee(employee);
            else
                this.ReloadingFrom.RemoveEmployee(Int32.Parse(this.textBox1.Text));
        }
        private void EditEmployees_Load(object sender, EventArgs e)
        {
            DataShowing();
        }
        private void DataShowing()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand scmd = new SqlCommand("SELECT * FROM employees WHERE employeeid =@i", connection))
                {
                    scmd.Parameters.AddWithValue("@i", this.EmployeeToEditDelete);
                    connection.Open();
                    var dr = scmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetInt32(0).ToString();
                        textBox2.Text = dr.GetString(1);
                        dateTimePicker1.Value = dr.GetDateTime(2);
                        textBox3.Text = dr.GetDecimal(3).ToString("0.00");
                        textBox4.Text = dr.GetString(4);
                        textBox5.Text = dr.GetString(5);
                        checkBox1.Checked = dr.GetBoolean(6);
                        oldFile = dr.GetString(7).ToString();
                        pictureBox1.Image = Image.FromFile(Path.Combine(@"..\..\Pictures", dr.GetString(7).ToString()));
                    }
                    connection.Close();
                }
            }

        }
    }
}
