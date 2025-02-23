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
            dataGridView1.ColumnCount = 1; // Create first column
            dataGridView1.Columns[0].HeaderText = "Row Index"; // First column for row identification

            // Create and add ComboBox column
            DataGridViewComboBoxColumn comboColumn = new DataGridViewComboBoxColumn
            {
                HeaderText = "Select Item",
                Name = "ComboBoxColumn"
            };
            dataGridView1.Columns.Add(comboColumn); // Add as the second column

            dataGridView1.RowCount = 3; // 3 rows

            // Populate first column with row numbers (for clarity)
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = $"Row {i + 1}";

                // Assign dropdown items per row
                if (rowData.ContainsKey(i))
                {
                    var comboBoxCell = (DataGridViewComboBoxCell)dataGridView1.Rows[i].Cells[1];
                    comboBoxCell.Items.AddRange(rowData[i].ToArray());

                    // Set default value to avoid "value is not valid" error
                    comboBoxCell.Value = rowData[i][0];
                }
            }

            dataGridView1.EditingControlShowing += DataGridView1_EditingControlShowing;
            dataGridView1.CellEnter += DataGridView1_CellEnter; // Opens dropdown on click
            dataGridView1.DataError += DataGridView1_DataError; // Handle errors
        }

        private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 1 && e.Control is ComboBox comboBox)
            {
                int rowIndex = dataGridView1.CurrentCell.RowIndex;

                // Remove previous event handler to avoid duplicates
                comboBox.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;

                // Update ComboBox items based on row index
                comboBox.Items.Clear();
                if (rowData.ContainsKey(rowIndex))
                {
                    comboBox.Items.AddRange(rowData[rowIndex].ToArray());
                }

                // Optionally, handle selection changes
                comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            }
        }

        private void DataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            // If entering a ComboBox cell, enter edit mode to show the dropdown immediately
            if (e.ColumnIndex == 1)
            {
                dataGridView1.BeginEdit(true);
                if (dataGridView1.EditingControl is ComboBox comboBox)
                {
                    comboBox.DroppedDown = true; // Open the dropdown instantly
                }
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                Console.WriteLine("Selected: " + comboBox.SelectedItem);
            }
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Suppress default DataGridView error dialog
            e.ThrowException = false;
        }


    }
}
