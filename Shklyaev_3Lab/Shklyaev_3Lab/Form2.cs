using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Shklyaev_3Lab
{
    public partial class Form2 : Form
    {
        Tovar Tovar;
        public Form2()
        {
            InitializeComponent();
        }

        public Form2(int index)
        {
            InitializeComponent();
            Tovar = Program.logic.GetList()[index];
            Data_tovar.Text = Tovar.getTovar();
            Data_date.Value = Tovar.getDate();
            Data_balance.Text = Tovar.getBalance().ToString();
            if (Tovar.type_record == 1)
            {
                Data_description.Text = ((Description)Tovar).getDescription();
            }
            else
            {
                radioButton2.Checked = true;
                Data_inedible.Checked = ((Overdue)Tovar).getInedible();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Save_btn_Click(object sender, EventArgs e)
        {
            string d_tovar = Data_tovar.Text;
            DateTime date = Data_date.Value;
            int balance;
            try { balance = int.Parse(Data_balance.Text); }
            catch
            {
                MessageBox.Show("Недопустимые символы в поле balance!");
                return;
            }
            string description = Data_description.Text;
            bool inedible = Data_inedible.Checked;
            if (Data_description.Enabled)
            {
                if (d_tovar == "" || description == "")
                {
                    MessageBox.Show("Вы не заполнили одно или несколько полей!");
                    return;
                }
            }
            else
            {
                if (d_tovar == "")
                {
                    MessageBox.Show("Вы не заполнили одно поле!");
                    return;
                }
            }
            if (Tovar == null)
            {
                // Создание записи
                if (Data_description.Enabled)
                {
                    Tovar new_record = new Description(0, false, 1, d_tovar, date,  balance, description);
                    Program.logic.Save(new_record);
                }
                else
                {
                    Tovar new_record = new Overdue(0, false, 2, d_tovar, date, balance, inedible);
                    Program.logic.Save(new_record);
                }
            }
            else
            {
                // Обновление записи
                if (Data_description.Enabled)
                {
                    Tovar new_record = new Description(Tovar.id, false, 1, d_tovar, date, balance, description);
                    Program.logic.Save(new_record);
                }
                else
                {
                    Tovar new_record = new Overdue(Tovar.id, false, 2, d_tovar, date, balance, inedible);
                    Program.logic.Save(new_record);
                }
            }

            MessageBox.Show("Успешно!");
            this.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Data_description.Enabled = false;
            Data_inedible.Enabled = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Data_description.Enabled = true;
            Data_inedible.Enabled = false;
        }

        private void Data_tovar_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
