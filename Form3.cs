using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace CaparasAct_3
{
    public partial class Form3 : Form
    {
        // Database connection string for the inventory system
        private string dbConnectionString = "server=localhost;database=inventorydb;uid=root;pwd=;";

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            FetchAllRecords();
        }

        // Load all items from the database into the grid view
        private void FetchAllRecords()
        {
            try
            {
                using (MySqlConnection dbConnection = new MySqlConnection(dbConnectionString))
                {
                    dbConnection.Open();
                    string sqlQuery = "SELECT * FROM Items";
                    
                    using (MySqlDataAdapter sqlAdapter = new MySqlDataAdapter(sqlQuery, dbConnection))
                    {
                        DataTable tableResult = new DataTable();
                        sqlAdapter.Fill(tableResult);
                        dataGridViewInventory.DataSource = tableResult;
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Failed to retrieve inventory: " + error.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Insert a new item into the inventory database
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Validate that all fields are filled out
            if (string.IsNullOrWhiteSpace(textBoxItemName.Text) || 

                string.IsNullOrWhiteSpace(textBoxCategory.Text) ||

                string.IsNullOrWhiteSpace(textBoxQuantity.Text) || 

                string.IsNullOrWhiteSpace(textBoxPrice.Text))
            {
                MessageBox.Show("All input fields must be completed.", "Incomplete Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection dbConnection = new MySqlConnection(dbConnectionString))
                {
                    dbConnection.Open();
                    string sqlQuery = "INSERT INTO Items (ItemName, Category, Quantity, Price) VALUES (@iName, @iCat, @iQty, @iPrice)";
                    
                    using (MySqlCommand sqlCommand = new MySqlCommand(sqlQuery, dbConnection))
                    {
                        // Clean up price string formatting
                        string sanitizedPrice = textBoxPrice.Text.Replace("$", "").Replace(",", "").Trim();

                        sqlCommand.Parameters.AddWithValue("@iName", textBoxItemName.Text.Trim());
                        sqlCommand.Parameters.AddWithValue("@iCat", textBoxCategory.Text.Trim());
                        sqlCommand.Parameters.AddWithValue("@iQty", Convert.ToInt32(textBoxQuantity.Text.Trim()));
                        sqlCommand.Parameters.AddWithValue("@iPrice", Convert.ToDecimal(sanitizedPrice));
                        sqlCommand.ExecuteNonQuery();
                        MessageBox.Show("Record successfully inserted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FetchAllRecords();
                        ResetFormInputs();
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Insert operation failed: " + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Update an existing item record based on its ID
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            // Make sure an item is selected from the grid
            if (textBoxItemID.Text.Trim() == "")
            {
                MessageBox.Show("Please choose an entry from the table first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection dbConnection = new MySqlConnection(dbConnectionString))
                {
                    dbConnection.Open();
                    string sqlQuery = "UPDATE Items SET ItemName = @iName, Category = @iCat, Quantity = @iQty, Price = @iPrice WHERE ItemID = @iId";
                    
                    using (MySqlCommand sqlCommand = new MySqlCommand(sqlQuery, dbConnection))
                    {
                        string sanitizedPrice = textBoxPrice.Text.Replace("$", "").Replace(",", "").Trim();

                        sqlCommand.Parameters.AddWithValue("@iId", Convert.ToInt32(textBoxItemID.Text.Trim()));
                        sqlCommand.Parameters.AddWithValue("@iName", textBoxItemName.Text.Trim());
                        sqlCommand.Parameters.AddWithValue("@iCat", textBoxCategory.Text.Trim());
                        sqlCommand.Parameters.AddWithValue("@iQty", Convert.ToInt32(textBoxQuantity.Text.Trim()));
                        sqlCommand.Parameters.AddWithValue("@iPrice", Convert.ToDecimal(sanitizedPrice));
                        sqlCommand.ExecuteNonQuery();
                        MessageBox.Show("Record successfully modified!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FetchAllRecords();
                        ResetFormInputs();
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Update operation failed: " + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Delete an item record from the database after confirmation
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (textBoxItemID.Text.Trim() == "")
            {
                MessageBox.Show("Please choose an entry from the table to remove.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult userChoice = MessageBox.Show("Do you really want to remove this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (userChoice == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection dbConnection = new MySqlConnection(dbConnectionString))
                    {
                        dbConnection.Open();
                        string sqlQuery = "DELETE FROM Items WHERE ItemID = @iId";
                        
                        using (MySqlCommand sqlCommand = new MySqlCommand(sqlQuery, dbConnection))
                        {
                            sqlCommand.Parameters.AddWithValue("@iId", Convert.ToInt32(textBoxItemID.Text.Trim()));
                            sqlCommand.ExecuteNonQuery();

                            MessageBox.Show("Record successfully removed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            FetchAllRecords();
                            ResetFormInputs();
                        }
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show("Deletion failed: " + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Populate form textboxes when a grid row is selected
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridViewInventory.Rows[e.RowIndex];
                
                textBoxItemID.Text = selectedRow.Cells["ItemID"].Value?.ToString() ?? "";

                textBoxItemName.Text = selectedRow.Cells["ItemName"].Value?.ToString() ?? "";

                textBoxCategory.Text = selectedRow.Cells["Category"].Value?.ToString() ?? "";

                textBoxQuantity.Text = selectedRow.Cells["Quantity"].Value?.ToString() ?? "";

                textBoxPrice.Text = selectedRow.Cells["Price"].Value?.ToString() ?? "";
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            ResetFormInputs();
        }

        // Helper to clear all input text boxes
        private void ResetFormInputs()
        {
            textBoxItemID.Clear();
            textBoxItemName.Clear();
            textBoxCategory.Clear();
            textBoxQuantity.Clear();
            textBoxPrice.Clear();
        }

        // Return back to the login form
        private void buttonLogout_Click(object sender, EventArgs e)
        {
            Form2 loginWindow = new Form2();
            loginWindow.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
