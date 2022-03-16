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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            up_Tovars();
            Delete_btn.Enabled = false;
            Change_btn.Enabled = false;
        }

        private void up_Tovars()
        {
            Tovar.SelectedIndex = -1;
            Delete_btn.Enabled = false;
            Change_btn.Enabled = false;
            Tovar.Items.Clear();
            List<string> items = new List<string>();
            foreach (Tovar ord in Program.logic.GetList())
            {
                items.Add(ord.getTovar() + ", " + ord.getDate() +  ", " + ord.getBalance());
            }
            Tovar.Items.AddRange(items.ToArray());
        }
        private void Tovars_SelectedIndexChanged(object sender, EventArgs e)
        {
            Delete_btn.Enabled = true;
            Change_btn.Enabled = true;
        }
        private void Add_btn_Click(object sender, EventArgs e)
        {
            Form2 AddChangeForm = new Form2();
            AddChangeForm.ShowDialog();
            up_Tovars();
        }

        private void Change_btn_Click(object sender, EventArgs e)
        {
            int index = Tovar.SelectedIndex;
            Form2 AddChangeForm = new Form2(index);
            AddChangeForm.ShowDialog();
            up_Tovars();
        }

        private void Delete_btn_Click(object sender, EventArgs e)
        {
            int index = Tovar.SelectedIndex;
            Tovar acc = Program.logic.GetList()[index];
            DialogResult res = MessageBox.Show(
                "Вы точно хотите удалить данную запись?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {
                Program.logic.Delete(acc.id);
                MessageBox.Show("Запись успешна удалена!");
            }
            up_Tovars();
        }

        private void Generate_btn_Click(object sender, EventArgs e)
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            if (svd.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = svd.FileName;
            // сохраняем текст в файл
            Program.logic.generateReport(filename, date_from.Value, date_to.Value);
            MessageBox.Show("Файл сохранен");
        }
    }
}
