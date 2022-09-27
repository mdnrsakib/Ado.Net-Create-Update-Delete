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
    public partial class AddEmployee : Form
    {
        string filePath = "";
        List<Employee> Employees = new List<Employee>();
        public AddEmployee()
        {
            InitializeComponent();
        }
        public ICrossDataFormLoadSync ReloadFrom { get; set; }
        private void AddEmployee_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.GetNewEmployeeId().ToString();
        }
        private int GetNewEmployeeId()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand Scmd = new SqlCommand("SELECT ISNULL(MAX(employeeid), 0) FROM employees", connection))
                {
                    connection.Open();
                    int id = (int)Scmd.ExecuteScalar();
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

                    using (SqlCommand Scmd = new SqlCommand(@"INSERT INTO employees 
                                            (employeeid, name, joiningdate, salary, address,phone, isWorking, picture) VALUES
                                            (@i, @n, @d, @s, @a, @p, @w,@b)", connection, stran))
                    {
                        Scmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        Scmd.Parameters.AddWithValue("@n", textBox2.Text);
                        Scmd.Parameters.AddWithValue("@d", dateTimePicker1.Value);
                        Scmd.Parameters.AddWithValue("@s", decimal.Parse(textBox3.Text));
                        Scmd.Parameters.AddWithValue("@a", textBox4.Text);
                        Scmd.Parameters.AddWithValue("@p", textBox5.Text);
                        Scmd.Parameters.AddWithValue("@w", checkBox1.Checked);
                        string ext = Path.GetExtension(this.filePath);
                        string fileName = $"{Guid.NewGuid()}{ext}";
                        string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                        File.Copy(filePath, savePath, true);
                        Scmd.Parameters.AddWithValue("@b", fileName);

                        try
                        {
                            if (Scmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Employees.Add(new Employee
                                {
                                    EmployeeId = int.Parse(textBox1.Text),
                                    Name = textBox2.Text,
                                    JoinDate = dateTimePicker1.Value,
                                    Salary = decimal.Parse(textBox3.Text),
                                    Address = textBox4.Text,
                                    Phone = textBox5.Text,
                                    IsWorking = checkBox1.Checked,
                                    Picture = fileName
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.label7.Text = Path.GetFileName(this.filePath);
                this.pictureBox1.Image = Image.FromFile(this.filePath);
            }
        }

        private void AddEmployee_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ReloadFrom.ReloadData(this.Employees);
        }
    }
}
