using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Q2.Models;

namespace Q2
{
    public partial class MainWindow : Window
    {
        private int? filterCategoryId = null;
        private decimal? filterFrom = null;
        private decimal? filterTo = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();
            LoadProducts();
        }

        private void LoadCategories()
        {
            using (var context = new Prn212PeQ201Context())
            {
                List<Category> categories = context.Categories.OrderBy(c => c.Name).ToList();
                cboCategory.ItemsSource = categories;

                List<Category> filterList = new List<Category>();
                filterList.Add(new Category { Id = 0, Name = "All" });
                filterList.AddRange(categories);
                cboFilterCategory.ItemsSource = filterList;
                cboFilterCategory.SelectedIndex = 0;
            }
        }

        private void LoadProducts()
        {
            using (var context = new Prn212PeQ201Context())
            {
                var query = context.Products.Include(p => p.Lookup).AsQueryable();
                if (filterCategoryId != null)
                {
                    query = query.Where(p => p.LookupId == filterCategoryId);
                }
                if (filterFrom != null)
                {
                    query = query.Where(p => p.Amount >= filterFrom);
                }
                if (filterTo != null)
                {
                    query = query.Where(p => p.Amount <= filterTo);
                }
                dgProducts.ItemsSource = query.OrderBy(p => p.Id).ToList();
            }
            dgProducts.SelectedItem = null;
            ClearEditor();
        }

        private void ClearEditor()
        {
            txtId.Text = "";
            txtName.Text = "";
            txtAmount.Text = "";
            cboCategory.SelectedIndex = -1;
            dpWorkDate.Text = "";
            chkIsActive.IsChecked = false;
        }

        private bool TryParseNumber(string text, out decimal result)
        {
            result = 0;
            text = text.Trim();
            if (!Regex.IsMatch(text, @"^[+-]?[0-9]+(\.[0-9]{1,2})?$"))
            {
                return false;
            }
            return decimal.TryParse(text, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out result);
        }

        private void dgProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product? product = dgProducts.SelectedItem as Product;
            if (product == null)
            {
                ClearEditor();
                return;
            }
            txtId.Text = product.Id.ToString();
            txtName.Text = product.Name;
            cboCategory.SelectedValue = product.LookupId;
            txtAmount.Text = product.Amount.ToString("F2", CultureInfo.InvariantCulture);
            dpWorkDate.Text = product.WorkDate.ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-dd");
            chkIsActive.IsChecked = product.IsActive;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            decimal? from = null;
            decimal? to = null;

            if (txtPriceFrom.Text.Trim() != "")
            {
                if (!TryParseNumber(txtPriceFrom.Text, out decimal value))
                {
                    MessageBox.Show("Price from is invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                from = value;
            }
            if (txtPriceTo.Text.Trim() != "")
            {
                if (!TryParseNumber(txtPriceTo.Text, out decimal value))
                {
                    MessageBox.Show("Price to is invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                to = value;
            }
            if (from != null && to != null && from > to)
            {
                MessageBox.Show("Price from must be less than or equal to Price to.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cboFilterCategory.SelectedIndex > 0)
            {
                filterCategoryId = (int)cboFilterCategory.SelectedValue;
            }
            else
            {
                filterCategoryId = null;
            }
            filterFrom = from;
            filterTo = to;
            LoadProducts();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            cboFilterCategory.SelectedIndex = 0;
            txtPriceFrom.Text = "";
            txtPriceTo.Text = "";
            filterCategoryId = null;
            filterFrom = null;
            filterTo = null;
            LoadProducts();
        }

        private bool ValidateEditor(out string name, out int categoryId, out decimal amount, out DateOnly workDate)
        {
            name = txtName.Text.Trim();
            categoryId = 0;
            amount = 0;
            workDate = new DateOnly();

            if (name.Length < 1 || name.Length > 60)
            {
                MessageBox.Show("Name must contain 1 to 60 characters.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Category is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            categoryId = (int)cboCategory.SelectedValue;
            using (var context = new Prn212PeQ201Context())
            {
                int id = categoryId;
                if (!context.Categories.Any(c => c.Id == id))
                {
                    MessageBox.Show("Category does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            if (!TryParseNumber(txtAmount.Text, out amount))
            {
                MessageBox.Show("Amount is invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (amount < 0 || amount > 10000)
            {
                MessageBox.Show("Amount must be from 0 to 10000.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrEmpty(dpWorkDate.Text))
            {
                MessageBox.Show("WorkDate is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!DateOnly.TryParse(dpWorkDate.Text, out workDate))
            {
                MessageBox.Show("WorkDate is invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (workDate < new DateOnly(2026, 1, 1) || workDate > new DateOnly(2026, 12, 31))
            {
                MessageBox.Show("WorkDate must be from 2026-01-01 to 2026-12-31.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateEditor(out string name, out int categoryId, out decimal amount, out DateOnly workDate))
            {
                return;
            }

            using (var context = new Prn212PeQ201Context())
            {
                Product product = new Product();
                product.Name = name;
                product.LookupId = categoryId;
                product.Amount = amount;
                product.WorkDate = workDate;
                product.IsActive = chkIsActive.IsChecked == true;
                context.Products.Add(product);
                context.SaveChanges();
            }

            MessageBox.Show("Product added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadProducts();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Product? selected = dgProducts.SelectedItem as Product;
            if (selected == null)
            {
                MessageBox.Show("Please select a product to update.", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateEditor(out string name, out int categoryId, out decimal amount, out DateOnly workDate))
            {
                return;
            }

            using (var context = new Prn212PeQ201Context())
            {
                Product? product = context.Products.FirstOrDefault(p => p.Id == selected.Id);
                if (product == null)
                {
                    MessageBox.Show("Product not found.", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadProducts();
                    return;
                }

                product.Name = name;
                product.LookupId = categoryId;
                product.Amount = amount;
                product.WorkDate = workDate;
                product.IsActive = chkIsActive.IsChecked == true;
                context.SaveChanges();
            }

            MessageBox.Show("Product updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadProducts();
        }
    }
}
