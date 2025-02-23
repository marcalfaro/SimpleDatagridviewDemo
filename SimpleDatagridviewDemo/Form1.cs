using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleDatagridviewDemo
{
    public partial class Form1 : Form
    {
        private Dictionary<int, List<string>> rowData;

        public Form1()
        {
            InitializeComponent();

            LoadJsonData();
            InitializeDataGridView();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void LoadJsonData()
        {
            string json = @"
        {
            ""0"": [""Apple"", ""Banana"", ""Cherry"", ""Mango"", ""Grapes""],
            ""1"": [""Ant"", ""Bee"", ""Butterfly"", ""Spider"", ""Grasshopper""],
            ""2"": [""USA"", ""Canada"", ""Germany"", ""Australia"", ""Japan""]
        }";

            rowData = JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(json);
        }

        private void InitializeDataGridView()
        {
            dataGridView1.ColumnCount = 1;
            dataGridView1.Columns[0].HeaderText = "Row Index";

            // Create ComboBox column
            DataGridViewComboBoxColumn comboColumn = new DataGridViewComboBoxColumn
            {
                HeaderText = "Select Item",
                Name = "ComboBoxColumn"
            };
            dataGridView1.Columns.Add(comboColumn);

            dataGridView1.RowCount = 3;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = $"Row {i + 1}";

                if (rowData.ContainsKey(i))
                {
                    var comboBoxCell = (DataGridViewComboBoxCell)dataGridView1.Rows[i].Cells[1];
                    comboBoxCell.Items.AddRange(rowData[i].ToArray());
                    comboBoxCell.Value = rowData[i][0]; // Set default value
                }
            }

            dataGridView1.EditingControlShowing += DataGridView1_EditingControlShowing;
            dataGridView1.CellValidating += DataGridView1_CellValidating;
            dataGridView1.CellEnter += DataGridView1_CellEnter;
            dataGridView1.DataError += DataGridView1_DataError;
        }

        //private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        //{
        //    if (dataGridView1.CurrentCell.ColumnIndex == 1 && e.Control is ComboBox comboBox)
        //    {
        //        int rowIndex = dataGridView1.CurrentCell.RowIndex;
        //        comboBox.DropDownStyle = ComboBoxStyle.DropDown; // Allow text input
        //        comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        //        comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;

        //        comboBox.Items.Clear();
        //        if (rowData.ContainsKey(rowIndex))
        //        {
        //            comboBox.Items.AddRange(rowData[rowIndex].ToArray());
        //        }
        //    }
        //}

        private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 1 && e.Control is ComboBox comboBox)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDown;
                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;

                comboBox.KeyDown -= ComboBox_KeyDown; // Prevent duplicate event handlers
                comboBox.KeyDown += ComboBox_KeyDown;
            }
        }

        private void ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ComboBox comboBox = sender as ComboBox;
                if (comboBox != null && !string.IsNullOrWhiteSpace(comboBox.Text))
                {
                    string newValue = comboBox.Text;
                    int rowIndex = dataGridView1.CurrentCell.RowIndex;
                    DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)dataGridView1.Rows[rowIndex].Cells[1];

                    // ✅ Close drop-down before updating value
                    comboBox.DroppedDown = false;

                    // ✅ Add new value if not already in the list
                    if (!comboBoxCell.Items.Contains(newValue))
                    {
                        comboBoxCell.Items.Add(newValue);
                    }

                    // ✅ Set the cell value immediately
                    dataGridView1.Rows[rowIndex].Cells[1].Value = newValue;
                    dataGridView1.RefreshEdit();

                    // ✅ Prevent Enter from moving to another row
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void DataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                dataGridView1.BeginEdit(true);
                if (dataGridView1.EditingControl is ComboBox comboBox)
                {
                    comboBox.DroppedDown = true;
                }
            }
        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 1) // ComboBox Column
            {
                string inputValue = e.FormattedValue.ToString();
                int rowIndex = e.RowIndex;

                if (!string.IsNullOrWhiteSpace(inputValue))
                {
                    DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)dataGridView1.Rows[rowIndex].Cells[e.ColumnIndex];

                    // If the new value is not in the ComboBox, add it
                    if (!comboBoxCell.Items.Contains(inputValue))
                    {
                        comboBoxCell.Items.Add(inputValue);  // ✅ Add new value to ComboBox dropdown
                        rowData[rowIndex].Add(inputValue);  // ✅ Store new value in data dictionary
                    }

                    // ✅ Ensure the new value is set and visible in the cell
                    dataGridView1.Rows[rowIndex].Cells[e.ColumnIndex].Value = inputValue;

                    // ✅ Refresh the DataGridView to show updates
                    dataGridView1.RefreshEdit();
                }
            }
        }


        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

    }
}
