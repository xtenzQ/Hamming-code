using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgCrypt
{
    public delegate void ShowFrm();

    public partial class Matrix : Form
    {

        public event ShowFrm evtFrm;

        public Matrix()
        {
            InitializeComponent();

            if (!(Form1.G == null || Form1.G.Length == 0))
            {
                numericUpDown1.Value = Form1.n;
                numericUpDown2.Value = Form1.k;

                this.dataGridView1.ColumnCount = Form1.G.GetLength(1);

                for (int r = 0; r < Form1.G.GetLength(0); r++)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(this.dataGridView1);

                    for (int c = 0; c < Form1.G.GetLength(1); c++)
                    {
                        row.Cells[c].Value = Form1.G[r, c];
                    }

                    this.dataGridView1.Rows.Add(row);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clearAll();
            if (!(Convert.ToInt32(numericUpDown1.Value) <= Math.Pow(2, Convert.ToInt32(numericUpDown1.Value) - Convert.ToInt32(numericUpDown2.Value)) - 1))
            {
                MessageBox.Show("Нарушено условие n = k + m <= 2^m - 1!");
            }
            else
            {
                for (int i = 0; i < numericUpDown1.Value; i++)
                {
                    dataGridView1.Columns.Add(i.ToString(), i.ToString());
                    ((DataGridViewTextBoxColumn)dataGridView1.Columns[i]).MaxInputLength = 1;
                }
                for (int j = 0; j < numericUpDown2.Value; j++)
                {
                    dataGridView1.Rows.Add();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clearAll();

        }

        private void clearAll()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress += new KeyPressEventHandler(CheckKey);
        }

        private void CheckKey(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '0' ||
                e.KeyChar == '1')
            {
                return;
            }
            e.Handled = true;
        }

        private bool checkForNulls()
        {
            foreach (DataGridViewRow rw in this.dataGridView1.Rows)
            {
                for (int i = 0; i < rw.Cells.Count; i++)
                {
                    if (rw.Cells[i].Value == null || rw.Cells[i].Value == DBNull.Value || String.IsNullOrWhiteSpace(rw.Cells[i].Value.ToString()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (checkForNulls())
            {
                if (MessageBox.Show("Матрица заполнена не до конца! Заполнить пустые ячейки нулями?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (DataGridViewRow rw in this.dataGridView1.Rows)
                    {
                        for (int i = 0; i < rw.Cells.Count; i++)
                        {
                            if (rw.Cells[i].Value == null || rw.Cells[i].Value == DBNull.Value || String.IsNullOrWhiteSpace(rw.Cells[i].Value.ToString()))
                            {
                                rw.Cells[i].Value = 0;
                            }
                        }
                    }
                }
                else
                {
                    // user clicked no
                }
            }
            else
            {
                int cols = Convert.ToInt32(numericUpDown1.Value);
                int rows = Convert.ToInt32(numericUpDown2.Value);

                if (!(Convert.ToInt32(numericUpDown1.Value) <= Math.Pow(2, Convert.ToInt32(numericUpDown1.Value) - Convert.ToInt32(numericUpDown2.Value)) - 1))
                {
                    MessageBox.Show("Нарушено условие n = k + m <= 2^m - 1!");
                }
                else
                {
                    Form1.G = new int[rows, cols];
                    Form1.k = rows;
                    Form1.n = cols;

                    for (int x = 0; x < rows; x++)
                        for (int i = 0; i < cols; i++)
                            Form1.G[x, i] = Convert.ToInt32(dataGridView1.Rows[x].Cells[i].Value);

                    this.Close();
                }
            }
        }

        private void setDefault()
        {
            clearAll();
            int[,] Gg =
            {
                {1, 0, 0, 1, 1, 0, 0, 0, 0},
                {1, 1, 0, 1, 0, 1, 0, 0, 0},
                {1, 1, 1, 1, 0, 0, 1, 0, 0},
                {1, 1, 1, 0, 0, 0, 0, 1, 0},
                {0, 1, 1, 1, 0, 0, 0, 0, 1}
            };

            numericUpDown1.Value = Gg.GetLength(1);
            numericUpDown2.Value = Gg.GetLength(0);

            this.dataGridView1.ColumnCount = Gg.GetLength(1);

            for (int r = 0; r < Gg.GetLength(0); r++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.dataGridView1);

                for (int c = 0; c < Gg.GetLength(1); c++)
                {
                    row.Cells[c].Value = Gg[r, c];
                }

                this.dataGridView1.Rows.Add(row);
            }

            /*
            for (int i = 0; i < Gg.GetLength(0); i++)
            {
                dataGridView1.Columns.Add(i.ToString(), i.ToString());
            }

            for (int rowIndex = 0; rowIndex < Gg.GetLength(0); ++rowIndex)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.dataGridView1);

                for (int columnIndex = 0; columnIndex < Gg.GetLength(1); ++columnIndex)
                {
                    row.Cells[columnIndex].Value = Gg[rowIndex, columnIndex];
                }
                dataGridView1.Rows.Add(row);
            }*/
        }

        private void Matrix_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!(Form1.G == null || Form1.G.Length == 0))
            {
                if (evtFrm != null)
                {
                    evtFrm();
                }
            }
        }

        public void manageB()
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Установить значение по умолчанию?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                setDefault();
            }
            else
            {
                // user clicked no
            }
        }

        private void dataGridView1_EditingControlShowing_1(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress += new KeyPressEventHandler(CheckKey);
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

        }
    }
}
