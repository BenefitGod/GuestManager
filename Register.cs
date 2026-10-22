using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();

            /*id_client.Text = "input";*/
        }

        /*private void passport_Enter(object sender, EventArgs e)
        {
            if (id_client.Text == "input")
            {
                id_client.Text = "";
            }
        }

        private void passport_Leave(object sender, EventArgs e)
        {
            if (id_client.Text == "")
            {
                id_client.Text = "input";
            }
        }*/

        public Boolean isUserExists()
        {
            DB db = new DB();

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM resident WHERE passport = @uL", db.GetConnection());
            command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = (passport.Text);


            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                MessageBox.Show("UZE EST");
                return true;

            }
            else
            {
                MessageBox.Show("Nema takogo");
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fio1 = fio.Text;
            string phone = phone_number.Text;
            string pass = passport.Text;

            if (isUserExists())
                return;

            DB db = new DB();

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("INSERT INTO resident (fio, phone_number, passport) VALUES " +
                "(@fio, @phone, @pass)", db.GetConnection());
             
            command.Parameters.Add("@fio", MySqlDbType.VarChar).Value = fio1;
            command.Parameters.Add("@phone", MySqlDbType.VarChar).Value = phone;
            command.Parameters.Add("@pass", MySqlDbType.VarChar).Value = pass;

            db.openConnection();

            if (command.ExecuteNonQuery() == 1)
                MessageBox.Show("acc");
            else 
                MessageBox.Show("notacc");

            db.closeConnection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }
    }
}
