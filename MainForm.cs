using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class MainForm : Form
    {
        private DataTable table;
        private TableLayoutPanel tablePanel;
        private int lastSelectedRow = -1;
        private string selectedRowId = null;
        private string currentTableName = "resident";

        public MainForm()
        {
            InitializeComponent();

            // Создаем панель для кнопок переключения таблиц
            Panel tableSwitchPanel = new Panel();
            tableSwitchPanel.Name = "tableSwitchPanel";
            tableSwitchPanel.Dock = DockStyle.Bottom;
            tableSwitchPanel.Height = 60; // Высота панели
            tableSwitchPanel.BackColor = Color.LightGray;

            // Находим все кнопки переключения таблиц (button1 - button11)
            var tableButtons = this.Controls.OfType<Button>()
                .Where(b => b.Name.StartsWith("button") && int.TryParse(b.Name.Substring(6), out _))
                .OrderBy(b => b.Name)
                .ToList();

            // Перемещаем кнопки на новую панель
            int x = 10;
            foreach (var button in tableButtons)
            {
                button.Parent = tableSwitchPanel;
                button.Location = new Point(x, 15);
                button.Size = new Size(100, 30);
                x += 110;
            }

            this.Controls.Add(tableSwitchPanel);
            tableSwitchPanel.BringToFront(); // Чтобы была поверх других элементов
        }

        private async void BtnParallelTest_Click(object sender, EventArgs e)
        {
            await RunParallelDemo();
        }

        private async Task RunParallelDemo()
        {
            var results = await Task.Run(() =>
            {
                var parallelResults = new List<string>();

                Parallel.Invoke(
                    () => {
                        Thread.Sleep(1000);
                        parallelResults.Add("Задача 1 завершена");
                    },
                    () => {
                        Thread.Sleep(800);
                        parallelResults.Add("Задача 2 завершена");
                    },
                    () => {
                        Thread.Sleep(600);
                        parallelResults.Add("Задача 3 завершена");
                    },
                    () => {
                        Thread.Sleep(400);
                        parallelResults.Add("Задача 4 завершена");
                    }
                );

                return parallelResults;
            });

            MessageBox.Show("Результаты параллельного выполнения:\n" + string.Join("\n", results));
        }

        private void CreateEditableTable()
        {
            if (tablePanel != null)
            {
                this.Controls.Remove(tablePanel);
                tablePanel.Dispose();
            }

            tablePanel = new TableLayoutPanel();
            tablePanel.Location = new Point(10, 10);
            tablePanel.AutoSize = true;
            tablePanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            tablePanel.ColumnCount = table.Columns.Count;
            tablePanel.RowCount = table.Rows.Count + 2;

            for (int col = 0; col < table.Columns.Count; col++)
            {
                tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            }

            for (int col = 0; col < table.Columns.Count; col++)
            {
                Label header = new Label();
                header.Text = table.Columns[col].ColumnName;
                header.Font = new Font("Arial", 9, FontStyle.Bold);
                header.TextAlign = ContentAlignment.MiddleCenter;
                header.Dock = DockStyle.Fill;
                tablePanel.Controls.Add(header, col, 0);
            }

            for (int row = 0; row < table.Rows.Count; row++)
            {
                tablePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

                for (int col = 0; col < table.Columns.Count; col++)
                {
                    var columnName = table.Columns[col].ColumnName;

                    // Делаем только id_client и id_booking нередактируемыми (Label вместо TextBox)
                    if (columnName == "id_client")
                    {
                        Label label = new Label();
                        label.Text = table.Rows[row][col].ToString();
                        label.Dock = DockStyle.Fill;
                        label.Margin = new Padding(3);
                        label.TextAlign = ContentAlignment.MiddleLeft;
                        label.BackColor = SystemColors.Control;

                        string rowId = table.Rows[row][0].ToString();
                        label.Tag = rowId;

                        tablePanel.Controls.Add(label, col, row + 1);
                    }
                    else
                    {
                        TextBox textBox = new TextBox();
                        textBox.Text = table.Rows[row][col].ToString();
                        textBox.Dock = DockStyle.Fill;
                        textBox.Margin = new Padding(3);

                        string rowId = table.Rows[row][0].ToString();
                        textBox.Tag = rowId;

                        textBox.Enter += (s, e) => {
                            selectedRowId = rowId;
                            HighlightRow(rowId);
                        };
                        textBox.Click += (s, e) => {
                            selectedRowId = rowId;
                            HighlightRow(rowId);
                        };

                        textBox.TextChanged += TextBox_TextChanged;
                        tablePanel.Controls.Add(textBox, col, row + 1);
                    }
                }
            }

            tablePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            Font buttonFont = new Font("Arial", 9, FontStyle.Regular);

            Button btnSave = new Button();
            btnSave.Text = "Сохранить изменения";
            btnSave.Click += BtnSave_Click;
            btnSave.Size = new Size(150, 30);
            btnSave.Dock = DockStyle.Fill;
            btnSave.ForeColor = Color.Gray;
            btnSave.BackColor = Color.LightGreen;
            btnSave.TextAlign = ContentAlignment.MiddleCenter;
            btnSave.Font = buttonFont; // Добавлено
            tablePanel.Controls.Add(btnSave, 0, table.Rows.Count + 1);

            Button btnParallelTest = new Button();
            btnParallelTest.Text = "Тест параллелизма";
            btnParallelTest.Size = new Size(150, 30);
            btnParallelTest.Dock = DockStyle.Fill;
            btnParallelTest.Click += BtnParallelTest_Click;
            btnParallelTest.ForeColor = Color.Yellow;
            btnParallelTest.BackColor = Color.RoyalBlue;
            btnParallelTest.TextAlign = ContentAlignment.MiddleCenter;
            btnParallelTest.Font = buttonFont; // Добавлено
            tablePanel.Controls.Add(btnParallelTest, 1, table.Rows.Count + 1);

            Button btnDelete = new Button();
            btnDelete.Text = "Удалить строку";
            btnDelete.Size = new Size(150, 30);
            btnDelete.BackColor = Color.LightCoral;
            btnDelete.ForeColor = Color.DarkRed;
            btnDelete.Dock = DockStyle.Fill;
            btnDelete.Click += BtnDelete_Click;
            btnDelete.TextAlign = ContentAlignment.MiddleCenter;
            btnDelete.Font = buttonFont; // Добавлено
            tablePanel.Controls.Add(btnDelete, 2, table.Rows.Count + 1);

            Button btnAdd = new Button();
            btnAdd.Text = "Добавить запись";
            btnAdd.Size = new Size(150, 30);
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.ForeColor = Color.DarkGreen;
            btnAdd.Dock = DockStyle.Fill;
            btnAdd.TextAlign = ContentAlignment.MiddleCenter;
            btnAdd.Click += BtnAdd_Click;
            btnAdd.Font = buttonFont; // Добавлено
            tablePanel.Controls.Add(btnAdd, 3, table.Rows.Count + 1);

            if (!string.IsNullOrEmpty(selectedRowId))
            {
                HighlightRow(selectedRowId);
            }

            this.Controls.Add(tablePanel);
            this.Refresh();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string rowId = textBox.Tag?.ToString();

            if (!string.IsNullOrEmpty(rowId))
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    if (table.Rows[i][0].ToString() == rowId)
                    {
                        int col = tablePanel.GetColumn(textBox);
                        if (col >= 0 && col < table.Columns.Count)
                        {
                            table.Rows[i][col] = textBox.Text;
                        }
                        break;
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DB db = new DB();
                MySqlDataAdapter adapter = new MySqlDataAdapter($"SELECT * FROM {currentTableName}", db.GetConnection());

                MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(adapter);
                adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                adapter.InsertCommand = commandBuilder.GetInsertCommand();
                adapter.DeleteCommand = commandBuilder.GetDeleteCommand();

                int rowsAffected = adapter.Update(table);
                MessageBox.Show($"Успешно обновлено записей: {rowsAffected}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}\n\nДетали: {ex.InnerException?.Message}");
            }
        }

        private void HighlightRow(string rowId)
        {
            foreach (Control control in tablePanel.Controls)
            {
                if (control is TextBox textBox && textBox.Tag?.ToString() == rowId)
                {
                    textBox.BackColor = Color.LightYellow;
                }
                else if (control is TextBox textBox2)
                {
                    textBox2.BackColor = SystemColors.Window;
                }
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(selectedRowId))
                {
                    int rowIndex = -1;
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        if (table.Rows[i][0].ToString() == selectedRowId)
                        {
                            rowIndex = i;
                            break;
                        }
                    }

                    if (rowIndex >= 0)
                    {
                        DialogResult result = MessageBox.Show(
                            $"Вы уверены, что хотите удалить строку с ID {selectedRowId} из таблицы {currentTableName}?",
                            "Подтверждение удаления",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (result == DialogResult.Yes)
                        {
                            await DeleteRowFromDatabase(rowIndex, selectedRowId);
                            selectedRowId = null;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не удалось найти выделенную строку");
                    }
                }
                else
                {
                    MessageBox.Show("Сначала выделите строку, кликнув на любую ячейку");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }
        }

        private async Task DeleteRowFromDatabase(int rowIndex, string primaryKeyValue)
        {
            try
            {
                bool success = await Task.Run(() =>
                {
                    try
                    {
                        DB db = new DB();
                        using (MySqlConnection connection = db.GetConnection())
                        {
                            connection.Open();
                            string primaryKeyColumn = table.Columns[0].ColumnName;

                            if (currentTableName == "resident")
                            {
                                using (MySqlTransaction transaction = connection.BeginTransaction())
                                {
                                    try
                                    {
                                        string[] deleteQueries = {
                                            @"DELETE p FROM payment p INNER JOIN booking b ON p.id_booking = b.id_booking WHERE b.id_client = @clientId",
                                            @"DELETE f FROM feedback f INNER JOIN booking b ON f.id_booking = b.id_booking WHERE b.id_client = @clientId",
                                            @"DELETE bs FROM book_service bs INNER JOIN booking b ON bs.id_booking = b.id_booking WHERE b.id_client = @clientId",
                                            "DELETE FROM booking WHERE id_client = @clientId",
                                            $"DELETE FROM resident WHERE {primaryKeyColumn} = @value"
                                        };

                                        foreach (string query in deleteQueries)
                                        {
                                            using (MySqlCommand cmd = new MySqlCommand(query, connection, transaction))
                                            {
                                                if (query.Contains("@clientId"))
                                                    cmd.Parameters.AddWithValue("@clientId", primaryKeyValue);
                                                if (query.Contains("@value"))
                                                    cmd.Parameters.AddWithValue("@value", primaryKeyValue);
                                                cmd.ExecuteNonQuery();
                                            }
                                        }

                                        transaction.Commit();
                                        return true;
                                    }
                                    catch
                                    {
                                        transaction.Rollback();
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                string deleteQuery = $"DELETE FROM {currentTableName} WHERE {primaryKeyColumn} = @value";
                                using (MySqlCommand cmd = new MySqlCommand(deleteQuery, connection))
                                {
                                    cmd.Parameters.AddWithValue("@value", primaryKeyValue);
                                    int rowsAffected = cmd.ExecuteNonQuery();
                                    return rowsAffected > 0;
                                }
                            }
                        }
                    }
                    catch
                    {
                        return false;
                    }
                });

                if (success)
                {
                    await RefreshDataTableAsync();
                    MessageBox.Show($"Данные из таблицы {currentTableName} успешно удалены!");
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении данных");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var addForm = new Form())
                {
                    addForm.Text = $"Добавление новой записи в {currentTableName}";
                    addForm.Size = new Size(400, 400);
                    addForm.StartPosition = FormStartPosition.CenterParent;
                    addForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    addForm.MaximizeBox = false;
                    addForm.MinimizeBox = false;

                    var panel = new TableLayoutPanel();
                    panel.Dock = DockStyle.Fill;
                    panel.ColumnCount = 2;
                    panel.Padding = new Padding(10);

                    var textBoxes = new List<TextBox>();

                    // Определяем, какие столбцы показывать для ввода
                    var allowedColumns = new List<DataColumn>();
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        var column = table.Columns[i];
                        // Для таблицы booking ВКЛЮЧАЕМ id_client в форму добавления
                        if (currentTableName != "booking" || column.ColumnName != "id_booking")
                        {
                            allowedColumns.Add(column);
                        }
                    }

                    panel.RowCount = allowedColumns.Count;
                    panel.ColumnCount = 2;

                    for (int i = 0; i < panel.RowCount; i++)
                    {
                        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
                    }
                    panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
                    panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

                    for (int i = 0; i < allowedColumns.Count; i++)
                    {
                        var column = allowedColumns[i];

                        var label = new Label();
                        label.Text = column.ColumnName + ":";
                        label.Dock = DockStyle.Fill;
                        label.Margin = new Padding(0, 5, 5, 5);
                        panel.Controls.Add(label, 0, i);

                        var textBox = new TextBox();
                        textBox.Dock = DockStyle.Fill;
                        textBox.Margin = new Padding(0, 5, 0, 5);
                        textBox.Tag = column;

                        // Альтернатива PlaceholderText для старых версий .NET
                        if (column.ColumnName != null)
                        {
                            string swag = column.ColumnName;
                            // Способ 1: Установить текст по умолчанию (будет виден пока пользователь не начнет ввод)
                            textBox.Text = $"Введите {swag}";
                            textBox.ForeColor = Color.Gray;

                            // Обработчики событий для имитации placeholder
                            textBox.Enter += (s, ev) =>
                            {
                                if (textBox.Text == $"Введите {swag}")
                                {
                                    textBox.Text = "";
                                    textBox.ForeColor = SystemColors.WindowText;
                                }
                            };

                            textBox.Leave += (s, ev) =>
                            {
                                if (string.IsNullOrWhiteSpace(textBox.Text))
                                {
                                    textBox.Text = $"Введите {swag}";
                                    textBox.ForeColor = Color.Gray;
                                }
                            };
                        }

                        panel.Controls.Add(textBox, 1, i);
                        textBoxes.Add(textBox);
                    }

                    // Добавляем подсказки внизу формы
                    var hintLabel = new Label();
                    hintLabel.Dock = DockStyle.Bottom;
                    hintLabel.Height = 60;
                    hintLabel.TextAlign = ContentAlignment.MiddleLeft;
                    hintLabel.Padding = new Padding(10, 0, 0, 0);
                    hintLabel.ForeColor = Color.Blue;
                    hintLabel.Font = new Font(hintLabel.Font, FontStyle.Italic);

                    if (currentTableName == "payment" || currentTableName == "feedback" || currentTableName == "book_service")
                    {
                        hintLabel.Text = "Примечание: ID бронирования должен существовать в таблице booking";
                    }
                    else if (currentTableName == "booking")
                    {
                        hintLabel.Text = "Примечание: ID клиента должен существовать в таблице resident";
                    }
                    else if (currentTableName == "staff")
                    {
                        hintLabel.Text = "Примечание: ID должности должен существовать в таблице position. Если должности нет, будет предложено создать новую.";
                    }

                    addForm.Controls.Add(hintLabel);

                    var btnOk = new Button();
                    btnOk.Text = "Добавить";
                    btnOk.DialogResult = DialogResult.OK;
                    btnOk.BackColor = Color.LightGreen;

                    var btnCancel = new Button();
                    btnCancel.Text = "Отмена";
                    btnCancel.DialogResult = DialogResult.Cancel;

                    var buttonPanel = new FlowLayoutPanel();
                    buttonPanel.FlowDirection = FlowDirection.RightToLeft;
                    buttonPanel.Dock = DockStyle.Bottom;
                    buttonPanel.Height = 40;
                    buttonPanel.Padding = new Padding(10);
                    buttonPanel.Controls.Add(btnOk);
                    buttonPanel.Controls.Add(btnCancel);

                    addForm.Controls.Add(panel);
                    addForm.Controls.Add(buttonPanel);

                    addForm.AcceptButton = btnOk;
                    addForm.CancelButton = btnCancel;

                    if (addForm.ShowDialog() == DialogResult.OK)
                    {
                        // Проверяем, не остался ли placeholder текст
                        foreach (var textBox in textBoxes)
                        {
                            var column = (DataColumn)textBox.Tag;
                            if (textBox.Text == $"Введите {column.ColumnName}")
                            {
                                textBox.Text = "";
                            }
                        }

                        if (ValidateAddForm(textBoxes))
                        {
                            await AddRecordToDatabase(textBoxes, allowedColumns);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}");
            }
        }

        private bool ValidateAddForm(List<TextBox> textBoxes)
        {
            foreach (var textBox in textBoxes)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    var column = (DataColumn)textBox.Tag;
                    MessageBox.Show($"Поле '{column.ColumnName}' должно быть заполнено!");
                    textBox.Focus();
                    return false;
                }
            }
            return true;
        }

        // Метод для проверки существования значения в родительской таблице
        private async Task<bool> CheckForeignKeyExists(string foreignKeyTable, string foreignKeyColumn, string value)
        {
            return await Task.Run(() =>
            {
                try
                {
                    DB db = new DB();
                    using (MySqlConnection connection = db.GetConnection())
                    {
                        connection.Open();
                        string query = $"SELECT COUNT(*) FROM {foreignKeyTable} WHERE {foreignKeyColumn} = @value";
                        Console.WriteLine($"Проверка FK: {query}, значение: {value}");

                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@value", value);
                            long count = (long)cmd.ExecuteScalar();
                            Console.WriteLine($"Результат проверки: {count} записей найдено");
                            return count > 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при проверке foreign key: {ex.Message}");
                    MessageBox.Show($"Ошибка при проверке foreign key: {ex.Message}");
                    return false;
                }
            });
        }

        // УПРОЩЕННЫЙ метод для добавления новой записи в родительскую таблицу
        private async Task<bool> AddToParentTable(string tableName, Dictionary<string, object> values)
        {
            try
            {
                return await Task.Run(() =>
                {
                    try
                    {
                        DB db = new DB();
                        using (MySqlConnection connection = db.GetConnection())
                        {
                            connection.Open();

                            if (tableName == "room_type")
                            {
                                string insertQuery = "INSERT INTO room_type (id_type, name, description) VALUES (@id_type, @name, @description)";

                                using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                                {
                                    command.Parameters.AddWithValue("@id_type", values["id_type"]);
                                    command.Parameters.AddWithValue("@name", values["name"]);
                                    command.Parameters.AddWithValue("@description", values["description"]);

                                    int result = command.ExecuteNonQuery();
                                    return result > 0;
                                }
                            }
                            else if (tableName == "position")
                            {
                                string insertQuery = "INSERT INTO position (id_position, position_name, salary) VALUES (@id_position, @position_name, @salary)";

                                using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                                {
                                    command.Parameters.AddWithValue("@id_position", values["id_position"]);
                                    command.Parameters.AddWithValue("@position_name", values["position_name"]);
                                    command.Parameters.AddWithValue("@salary", values["salary"]);

                                    int result = command.ExecuteNonQuery();
                                    return result > 0;
                                }
                            }

                            return false;
                        }
                    }
                    catch (MySqlException mysqlEx)
                    {
                        MessageBox.Show($"MySQL ошибка при добавлении в {tableName}: {mysqlEx.Message}\n\nКод ошибки: {mysqlEx.Number}");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при добавлении в {tableName}: {ex.Message}");
                        return false;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Общая ошибка: {ex.Message}");
                return false;
            }
        }

        // Метод для показа диалога добавления в родительскую таблицу
        private bool ShowAddToParentTableDialog(string parentTable, string foreignKeyValue, out Dictionary<string, object> newValues)
        {
            newValues = new Dictionary<string, object>();

            using (var addParentForm = new Form())
            {
                addParentForm.Text = $"Добавление новой записи в {parentTable}";
                addParentForm.Size = new Size(500, 300);
                addParentForm.StartPosition = FormStartPosition.CenterParent;
                addParentForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                addParentForm.MaximizeBox = false;
                addParentForm.MinimizeBox = false;

                var panel = new TableLayoutPanel();
                panel.Dock = DockStyle.Fill;
                panel.ColumnCount = 2;
                panel.Padding = new Padding(10);

                var textBoxes = new Dictionary<string, TextBox>();
                var labels = new List<string>();

                if (parentTable == "room_type")
                {
                    labels.AddRange(new[] { "id_type", "name", "description" });
                }
                else if (parentTable == "position")
                {
                    labels.AddRange(new[] { "id_position", "position_name", "salary" });
                }

                panel.RowCount = labels.Count;

                for (int i = 0; i < labels.Count; i++)
                {
                    panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
                }
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

                for (int i = 0; i < labels.Count; i++)
                {
                    var labelName = labels[i];

                    var label = new Label();
                    label.Text = labelName + ":";
                    label.Dock = DockStyle.Fill;
                    label.Margin = new Padding(0, 5, 5, 5);
                    panel.Controls.Add(label, 0, i);

                    var textBox = new TextBox();
                    textBox.Dock = DockStyle.Fill;
                    textBox.Margin = new Padding(0, 5, 0, 5);

                    // Автозаполнение ID если передан foreignKeyValue
                    if ((labelName == "id_type" || labelName == "id_position") && !string.IsNullOrEmpty(foreignKeyValue))
                    {
                        textBox.Text = foreignKeyValue;
                        textBox.ReadOnly = true;
                        textBox.BackColor = SystemColors.Control;
                    }

                    panel.Controls.Add(textBox, 1, i);
                    textBoxes[labelName] = textBox;
                }

                var btnOk = new Button();
                btnOk.Text = "Добавить";
                btnOk.DialogResult = DialogResult.OK;
                btnOk.BackColor = Color.LightGreen;

                var btnCancel = new Button();
                btnCancel.Text = "Отмена";
                btnCancel.DialogResult = DialogResult.Cancel;

                var buttonPanel = new FlowLayoutPanel();
                buttonPanel.FlowDirection = FlowDirection.RightToLeft;
                buttonPanel.Dock = DockStyle.Bottom;
                buttonPanel.Height = 40;
                buttonPanel.Padding = new Padding(10);
                buttonPanel.Controls.Add(btnOk);
                buttonPanel.Controls.Add(btnCancel);

                addParentForm.Controls.Add(panel);
                addParentForm.Controls.Add(buttonPanel);

                addParentForm.AcceptButton = btnOk;
                addParentForm.CancelButton = btnCancel;

                if (addParentForm.ShowDialog() == DialogResult.OK)
                {
                    foreach (var kvp in textBoxes)
                    {
                        if (string.IsNullOrWhiteSpace(kvp.Value.Text))
                        {
                            MessageBox.Show($"Поле '{kvp.Key}' должно быть заполнено!");
                            return false;
                        }

                        // Преобразуем значения в правильные типы
                        if (kvp.Key == "id_type" || kvp.Key == "id_position")
                        {
                            if (int.TryParse(kvp.Value.Text.Trim(), out int intValue))
                            {
                                newValues[kvp.Key] = intValue;
                            }
                            else
                            {
                                MessageBox.Show($"Поле '{kvp.Key}' должно содержать числовое значение!");
                                return false;
                            }
                        }
                        else if (kvp.Key == "salary")
                        {
                            if (decimal.TryParse(kvp.Value.Text.Trim(), out decimal decimalValue))
                            {
                                newValues[kvp.Key] = decimalValue;
                            }
                            else
                            {
                                MessageBox.Show($"Поле 'salary' должно содержать числовое значение!");
                                return false;
                            }
                        }
                        else
                        {
                            newValues[kvp.Key] = kvp.Value.Text.Trim();
                        }
                    }
                    return true;
                }
            }

            return false;
        }

        // Обновленный метод добавления записей
        private async Task AddRecordToDatabase(List<TextBox> textBoxes, List<DataColumn> allowedColumns)
        {
            try
            {
                int rowsAffected = await Task.Run(() =>
                {
                    try
                    {
                        DB db = new DB();

                        var columnNames = new List<string>();
                        var parameterNames = new List<string>();
                        var parameters = new Dictionary<string, object>();

                        // Отладочная информация
                        Console.WriteLine($"Добавление в таблицу: {currentTableName}");
                        Console.WriteLine("Колонки для добавления:");
                        foreach (var column in allowedColumns)
                        {
                            Console.WriteLine($" - {column.ColumnName} (тип: {column.DataType})");
                        }

                        for (int i = 0; i < allowedColumns.Count; i++)
                        {
                            var column = allowedColumns[i];
                            var textValue = textBoxes[i].Text.Trim();

                            columnNames.Add($"`{column.ColumnName}`");
                            parameterNames.Add($"@{column.ColumnName}");

                            // Проверка foreign key для id_type в таблице room
                            if (column.ColumnName == "id_type" && currentTableName == "room")
                            {
                                if (!int.TryParse(textValue, out int idTypeValue))
                                {
                                    throw new Exception($"Некорректное значение для id_type: {textValue}");
                                }

                                bool exists = CheckForeignKeyExists("room_type", "id_type", textValue).Result;
                                if (!exists)
                                {
                                    var result = MessageBox.Show(
                                        $"Значение {textValue} не существует в таблице room_type.\nХотите добавить новый тип комнаты?",
                                        "Добавление нового типа комнаты",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question);

                                    if (result == DialogResult.Yes)
                                    {
                                        if (ShowAddToParentTableDialog("room_type", textValue, out Dictionary<string, object> newRoomTypeValues))
                                        {
                                            MessageBox.Show("Добавляем новый тип комнаты...", "Пожалуйста подождите");
                                            bool added = AddToParentTable("room_type", newRoomTypeValues).Result;
                                            if (!added)
                                            {
                                                throw new Exception("Не удалось добавить новый тип комнаты.");
                                            }
                                            else
                                            {
                                                MessageBox.Show("Новый тип комнаты успешно добавлен!", "Успех");
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Добавление нового типа комнаты отменено");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception($"Значение {textValue} не существует в таблице room_type");
                                    }
                                }
                            }

                            // Проверка foreign key для id_position в таблице staff
                            if (column.ColumnName == "id_position" && currentTableName == "staff")
                            {
                                if (!int.TryParse(textValue, out int idPositionValue))
                                {
                                    throw new Exception($"Некорректное значение для id_position: {textValue}");
                                }

                                bool exists = CheckForeignKeyExists("position", "id_position", textValue).Result;
                                if (!exists)
                                {
                                    var result = MessageBox.Show(
                                        $"Должность с ID {textValue} не существует в таблице position.\nХотите добавить новую должность?",
                                        "Добавление новой должности",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question);

                                    if (result == DialogResult.Yes)
                                    {
                                        if (ShowAddToParentTableDialog("position", textValue, out Dictionary<string, object> newPositionValues))
                                        {
                                            MessageBox.Show("Добавляем новую должность...", "Пожалуйста подождите");
                                            bool added = AddToParentTable("position", newPositionValues).Result;
                                            if (!added)
                                            {
                                                throw new Exception("Не удалось добавить новую должность.");
                                            }
                                            else
                                            {
                                                MessageBox.Show("Новая должность успешно добавлена!", "Успех");
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Добавление новой должности отменено");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception($"Должность с ID {textValue} не существует в таблице position");
                                    }
                                }
                            }

                            // Проверка foreign key для id_booking
                            if (column.ColumnName == "id_booking" &&
                                (currentTableName == "payment" || currentTableName == "feedback" || currentTableName == "book_service"))
                            {
                                if (!int.TryParse(textValue, out int idBookingValue))
                                {
                                    throw new Exception($"Некорректное значение для id_booking: {textValue}");
                                }

                                bool exists = CheckForeignKeyExists("booking", "id_booking", textValue).Result;
                                if (!exists)
                                {
                                    throw new Exception($"Бронирование с ID {textValue} не существует в таблице booking. Сначала создайте бронирование.");
                                }
                            }

                            // Проверка foreign key для id_client в таблице booking
                            if (column.ColumnName == "id_client" && currentTableName == "booking")
                            {
                                if (!int.TryParse(textValue, out int idClientValue))
                                {
                                    throw new Exception($"Некорректное значение для id_client: {textValue}");
                                }

                                bool exists = CheckForeignKeyExists("resident", "id_client", textValue).Result;
                                if (!exists)
                                {
                                    throw new Exception($"Клиент с ID {textValue} не существует в таблице resident. Сначала создайте клиента.");
                                }
                            }

                            // Обработка типов данных
                            if (column.DataType == typeof(DateTime))
                            {
                                if (DateTime.TryParse(textValue, out DateTime dateValue))
                                {
                                    parameters.Add($"@{column.ColumnName}", dateValue.ToString("yyyy-MM-dd"));
                                }
                                else
                                {
                                    throw new Exception($"Некорректный формат даты для {column.ColumnName}. Используйте формат ДД.ММ.ГГГГ");
                                }
                            }
                            else if (column.DataType == typeof(int) || column.ColumnName.EndsWith("_id") || column.ColumnName.StartsWith("id_"))
                            {
                                if (int.TryParse(textValue, out int intValue))
                                {
                                    parameters.Add($"@{column.ColumnName}", intValue);
                                }
                                else if (string.IsNullOrEmpty(textValue))
                                {
                                    parameters.Add($"@{column.ColumnName}", DBNull.Value);
                                }
                                else
                                {
                                    throw new Exception($"Некорректное числовое значение для {column.ColumnName}");
                                }
                            }
                            else if (column.DataType == typeof(decimal))
                            {
                                if (decimal.TryParse(textValue, out decimal decimalValue))
                                {
                                    parameters.Add($"@{column.ColumnName}", decimalValue);
                                }
                                else
                                {
                                    throw new Exception($"Некорректное денежное значение для {column.ColumnName}");
                                }
                            }
                            else
                            {
                                parameters.Add($"@{column.ColumnName}", textValue);
                            }
                        }

                        string insertQuery = $"INSERT INTO {currentTableName} ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", parameterNames)})";
                        Console.WriteLine($"SQL запрос: {insertQuery}");

                        using (MySqlConnection connection = db.GetConnection())
                        {
                            connection.Open();
                            Console.WriteLine("Подключение к БД установлено");

                            using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                            {
                                foreach (var param in parameters)
                                {
                                    command.Parameters.AddWithValue(param.Key, param.Value);
                                    Console.WriteLine($"Параметр: {param.Key} = {param.Value}");
                                }

                                int result = command.ExecuteNonQuery();
                                Console.WriteLine($"Запись добавлена, затронуто строк: {result}");
                                return result;
                            }
                        }
                    }
                    catch (MySqlException mysqlEx)
                    {
                        Console.WriteLine($"MySQL ошибка: {mysqlEx.Message}");
                        Console.WriteLine($"Код ошибки: {mysqlEx.Number}");
                        throw new Exception($"Ошибка MySQL ({mysqlEx.Number}): {mysqlEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Общая ошибка: {ex.Message}");
                        throw new Exception($"Ошибка базы данных: {ex.Message}");
                    }
                });

                if (rowsAffected > 0)
                {
                    await RefreshDataTableAsync();
                    MessageBox.Show($"Запись успешно добавлена в таблицу {currentTableName}!");
                }
                else
                {
                    MessageBox.Show("Не удалось добавить запись");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}");
            }
        }

        private async Task RefreshDataTableAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    DB db = new DB();
                    table.Clear();
                    MySqlDataAdapter adapter = new MySqlDataAdapter($"SELECT * FROM {currentTableName}", db.GetConnection());
                    adapter.Fill(table);
                });

                await Task.Run(() =>
                {
                    this.Invoke(new Action(() =>
                    {
                        CreateEditableTable();
                    }));
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}");
            }
        }

        private async Task LoadDataToEditableFormAsync(string tableName)
        {
            currentTableName = tableName;
            this.Text = $"Управление данными - {tableName}";

            var loadingForm = new Form()
            {
                Text = "Загрузка...",
                Size = new Size(300, 100),
                StartPosition = FormStartPosition.CenterParent
            };
            var label = new Label() { Text = $"Загружаем данные из {tableName}...", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            loadingForm.Controls.Add(label);
            loadingForm.Show();

            try
            {
                await Task.Run(() =>
                {
                    DB db = new DB();
                    table = new DataTable();
                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    MySqlCommand command = new MySqlCommand($"SELECT * FROM {tableName}", db.GetConnection());
                    adapter.SelectCommand = command;
                    adapter.Fill(table);
                    return table;
                });

                this.Invoke(new Action(() =>
                {
                    CreateEditableTable();
                }));
            }
            finally
            {
                loadingForm.Close();
            }
        }

        // Обработчики кнопок
        private async void button1_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string name = button.AccessibleName;
                await LoadDataToEditableFormAsync(name);
                ValidateDataParallel();
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string name = button.AccessibleName;
                await LoadDataToEditableFormAsync(name);
                ValidateDataParallel();
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string name = button.AccessibleName;
                await LoadDataToEditableFormAsync(name);
                ValidateDataParallel();
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string name = button.AccessibleName;
                await LoadDataToEditableFormAsync(name);
                ValidateDataParallel();
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string name = button.AccessibleName;
                await LoadDataToEditableFormAsync(name);
                ValidateDataParallel();
            }
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string name = button.AccessibleName;
                await LoadDataToEditableFormAsync(name);
                ValidateDataParallel();
            }
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string name = button.AccessibleName;
                await LoadDataToEditableFormAsync(name);
                ValidateDataParallel();
            }
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string name = button.AccessibleName;
                await LoadDataToEditableFormAsync(name);
                ValidateDataParallel();
            }
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string name = button.AccessibleName;
                await LoadDataToEditableFormAsync(name);
                ValidateDataParallel();
            }
        }

        private async void button10_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string name = button.AccessibleName;
                await LoadDataToEditableFormAsync(name);
                ValidateDataParallel();
            }
        }

        private async void button11_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string name = button.AccessibleName;
                await LoadDataToEditableFormAsync(name);
                ValidateDataParallel();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadDataToEditableFormAsync("resident");
        }

        private async void ValidateDataParallel()
        {
            var validationTasks = table.Rows.Cast<DataRow>()
                .Select(row => ValidateRowAsync(row))
                .ToArray();

            var validationResults = await Task.WhenAll(validationTasks);

            int validRows = validationResults.Count(r => r.IsValid);
            int invalidRows = validationResults.Count(r => !r.IsValid);

            MessageBox.Show($"Валидация завершена:\nВалидных: {validRows}\nНевалидных: {invalidRows}");
        }

        private async Task<(bool IsValid, string Message)> ValidateRowAsync(DataRow row)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(100);

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(row[i]?.ToString()))
                    {
                        return (false, $"Пустое поле в строке {row[0]}");
                    }
                }

                return (true, "OK");
            });
        }
    }
}