using System;
using System.Linq;
using System.Windows.Forms;

namespace FifteenPuzzle
{
    public partial class Form1 : Form
    {
        private Button emptyButton;

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Инициализация пустой кнопки
            emptyButton = new Button { Visible = false };
            tableLayoutPanel1.Controls.Add(emptyButton, 3, 3); // В нижний правый угол

            // Создание кнопок с номерами
            for (int i = 0; i < 15; i++)
            {
                Button button = new Button
                {
                    Text = (i + 1).ToString(),
                    Dock = DockStyle.Fill
                };
                button.Click += Button_Click;
                tableLayoutPanel1.Controls.Add(button, i % 4, i / 4);
            }

            Shuffle();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                SwapWithEmpty(clickedButton);
            }
        }

        private void SwapWithEmpty(Button button)
        {
            int buttonRow = tableLayoutPanel1.GetRow(button);
            int buttonColumn = tableLayoutPanel1.GetColumn(button);
            int emptyRow = tableLayoutPanel1.GetRow(emptyButton);
            int emptyColumn = tableLayoutPanel1.GetColumn(emptyButton);

            // Проверка, находится ли кнопка рядом с пустой ячейкой
            if ((Math.Abs(buttonRow - emptyRow) == 1 && buttonColumn == emptyColumn) ||
                (Math.Abs(buttonColumn - emptyColumn) == 1 && buttonRow == emptyRow))
            {
                tableLayoutPanel1.SetCellPosition(emptyButton, new TableLayoutPanelCellPosition(buttonColumn, buttonRow));
                tableLayoutPanel1.SetCellPosition(button, new TableLayoutPanelCellPosition(emptyColumn, emptyRow));
            }

            if (IsSolved())
            {
                MessageBox.Show("Поздравляем, вы выиграли!");
            }
        }

        private void Shuffle()
        {
            Random rand = new Random();
            var buttons = tableLayoutPanel1.Controls.OfType<Button>().Where(b => b != emptyButton).ToList();

            // Перемешивание позиций кнопок
            for (int i = buttons.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                var button1 = buttons[i];
                var button2 = buttons[j];

                var button1Position = tableLayoutPanel1.GetCellPosition(button1);
                var button2Position = tableLayoutPanel1.GetCellPosition(button2);

                tableLayoutPanel1.SetCellPosition(button1, button2Position);
                tableLayoutPanel1.SetCellPosition(button2, button1Position);
            }

            // Перемещение пустой кнопки в нижний правый угол
            tableLayoutPanel1.SetCellPosition(emptyButton, new TableLayoutPanelCellPosition(3, 3));

            // Повторное перемешивание, если результат - решаемое состояние
            while (IsSolved() || !IsSolvable())
            {
                for (int i = buttons.Count - 1; i > 0; i--)
                {
                    int j = rand.Next(i + 1);
                    var button1 = buttons[i];
                    var button2 = buttons[j];

                    var button1Position = tableLayoutPanel1.GetCellPosition(button1);
                    var button2Position = tableLayoutPanel1.GetCellPosition(button2);

                    tableLayoutPanel1.SetCellPosition(button1, button2Position);
                    tableLayoutPanel1.SetCellPosition(button2, button1Position);
                }
            }
        }

        private bool IsSolvable()
        {
            int[] puzzle = new int[16];
            int index = 0;

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    var control = tableLayoutPanel1.GetControlFromPosition(col, row);
                    if (control is Button button && button != emptyButton)
                    {
                        puzzle[index++] = int.Parse(button.Text);
                    }
                    else
                    {
                        puzzle[index++] = 0; // Пустое место
                    }
                }
            }

            int inversions = 0;
            for (int i = 0; i < 15; i++)
            {
                for (int j = i + 1; j < 16; j++)
                {
                    if (puzzle[i] > 0 && puzzle[j] > 0 && puzzle[i] > puzzle[j])
                    {
                        inversions++;
                    }
                }
            }

            // Найдите строку пустой клетки (0 - верхняя, 3 - нижняя)
            int emptyRow = Array.IndexOf(puzzle, 0) / 4;

            // Пятнашки решаемы, если четность инверсий + четность строки пустой клетки четна
            return (inversions + emptyRow) % 2 == 0;
        }

        private bool IsSolved()
        {
            int count = 1;
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    var control = tableLayoutPanel1.GetControlFromPosition(col, row);
                    if (control is Button button && button != emptyButton)
                    {
                        if (button.Text != count.ToString())
                        {
                            return false;
                        }
                        count++;
                    }
                }
            }
            return true;
        }
    }
}
