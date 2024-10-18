using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace SortowanieApp
{
    public partial class MainWindow : Window
    {
        private List<string> _lines;

        public MainWindow()
        {
            InitializeComponent();
            _lines = new List<string>();
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _lines = File.ReadAllLines(openFileDialog.FileName).ToList();
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            if (_lines.Count == 0)
            {
                MessageBox.Show("Proszę najpierw wybrać plik.");
                return;
            }

            ISortStrategy sortStrategy = GetSortStrategy();
            var stopwatch = Stopwatch.StartNew();
            sortStrategy.Sort(_lines);
            stopwatch.Stop();

            ShowSummary(_lines.Count, stopwatch.Elapsed);
            DisplaySortedLines();
            SaveSortedLines();
        }

        private ISortStrategy GetSortStrategy()
        {
            if (SortMethodComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "Bubble Sort":
                        return new BubbleSort();
                    case "Quick Sort":
                        return new QuickSort();
                    case "Insertion Sort":
                        return new InsertionSort();
                }
            }
            return new QuickSort(); // Domyślnie
        }

        private void ShowSummary(int count, TimeSpan elapsed)
        {
            MessageBox.Show($"Posortowano elementów: {count}\nCzas sortowania: {elapsed.TotalMilliseconds} ms");
        }

        private void DisplaySortedLines()
        {
            SortedOutputTextBox.Text = string.Join(Environment.NewLine, _lines);
        }

        private void SaveSortedLines()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllLines(saveFileDialog.FileName, _lines);
            }
        }
    }

    public interface ISortStrategy
    {
        void Sort(List<string> items);
    }

    public class BubbleSort : ISortStrategy
    {
        public void Sort(List<string> items)
        {
            int n = items.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (Compare(items[j], items[j + 1]) > 0)
                    {
                        var temp = items[j];
                        items[j] = items[j + 1];
                        items[j + 1] = temp;
                    }
                }
            }
        }

        private int Compare(string a, string b)
        {
            bool aIsNumber = double.TryParse(a, out double aNum);
            bool bIsNumber = double.TryParse(b, out double bNum);

            if (aIsNumber && bIsNumber)
            {
                return aNum.CompareTo(bNum); // Porównaj jako liczby
            }
            else
            {
                return string.Compare(a, b); // Porównaj jako tekst
            }
        }
    }

    public class QuickSort : ISortStrategy
    {
        public void Sort(List<string> items)
        {
            QuickSortHelper(items, 0, items.Count - 1);
        }

        private void QuickSortHelper(List<string> items, int low, int high)
        {
            if (low < high)
            {
                int pi = Partition(items, low, high);
                QuickSortHelper(items, low, pi - 1);
                QuickSortHelper(items, pi + 1, high);
            }
        }

        private int Partition(List<string> items, int low, int high)
        {
            var pivot = items[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (Compare(items[j], pivot) < 0)
                {
                    i++;
                    var temp = items[i];
                    items[i] = items[j];
                    items[j] = temp;
                }
            }

            var temp1 = items[i + 1];
            items[i + 1] = items[high];
            items[high] = temp1;

            return i + 1;
        }

        private int Compare(string a, string b)
        {
            bool aIsNumber = double.TryParse(a, out double aNum);
            bool bIsNumber = double.TryParse(b, out double bNum);

            if (aIsNumber && bIsNumber)
            {
                return aNum.CompareTo(bNum);
            }
            else
            {
                return string.Compare(a, b);
            }
        }
    }

    public class InsertionSort : ISortStrategy
    {
        public void Sort(List<string> items)
        {
            for (int i = 1; i < items.Count; i++)
            {
                var key = items[i];
                int j = i - 1;

                while (j >= 0 && Compare(items[j], key) > 0)
                {
                    items[j + 1] = items[j];
                    j--;
                }
                items[j + 1] = key;
            }
        }

        private int Compare(string a, string b)
        {
            bool aIsNumber = double.TryParse(a, out double aNum);
            bool bIsNumber = double.TryParse(b, out double bNum);

            if (aIsNumber && bIsNumber)
            {
                return aNum.CompareTo(bNum);
            }
            else
            {
                return string.Compare(a, b);
            }
        }
    }
}
