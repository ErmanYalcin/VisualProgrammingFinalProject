using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisualProgrammingFinalProject.Model;

namespace VisualProgrammingFinalProject
{
    public partial class CafeteriaManagementForm : Form
    {
        public CafeteriaManagementForm()
        {
            InitializeComponent();
        }

        private void CafeteriaManagementForm_Load(object sender, EventArgs e)
        {
            LoadCafeteriaItems();
        }

        private void LoadCafeteriaItems()
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var items = context.CafeteriaItems
                                   .Include(i => i.CafeteriaPurchases)
                                   .Include(i => i.ParentRestrictions)
                                   .Select(i => new
                                   {
                                       i.ItemID,
                                       i.ItemName,
                                       i.Price,
                                       i.Stock
                                   })
                                   .ToList();

                dataGridViewCafeteriaItems.DataSource = items;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var item = new CafeteriaItem
                {
                    ItemName = textBoxItemName.Text,
                    Price = decimal.Parse(textBoxPrice.Text),
                    Stock = int.Parse(textBoxStock.Text)
                };

                context.CafeteriaItems.Add(item);
                context.SaveChanges();
                LoadCafeteriaItems();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewCafeteriaItems.SelectedRows.Count > 0)
            {
                using (var context = new SchoolManagementSystemEntities1())
                {
                    var row = dataGridViewCafeteriaItems.SelectedRows[0];
                    int itemId = (int)row.Cells["ItemID"].Value;
                    var itemToUpdate = context.CafeteriaItems.FirstOrDefault(i => i.ItemID == itemId);

                    if (itemToUpdate != null)
                    {
                        itemToUpdate.ItemName = textBoxItemName.Text;
                        itemToUpdate.Price = decimal.Parse(textBoxPrice.Text);
                        itemToUpdate.Stock = int.Parse(textBoxStock.Text);

                        context.SaveChanges();
                        LoadCafeteriaItems();
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewCafeteriaItems.SelectedRows.Count > 0)
            {
                using (var context = new SchoolManagementSystemEntities1())
                {
                    var row = dataGridViewCafeteriaItems.SelectedRows[0];
                    int itemId = (int)row.Cells["ItemID"].Value;
                    var itemToDelete = context.CafeteriaItems.FirstOrDefault(i => i.ItemID == itemId);

                    if (itemToDelete != null)
                    {
                        context.CafeteriaItems.Remove(itemToDelete);
                        context.SaveChanges();
                        LoadCafeteriaItems();
                    }
                }
            }
        }

        private void dataGridViewCafeteriaItems_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewCafeteriaItems.SelectedRows.Count > 0)
            {
                var row = dataGridViewCafeteriaItems.SelectedRows[0];
                textBoxItemName.Text = row.Cells["ItemName"].Value.ToString();
                textBoxPrice.Text = row.Cells["Price"].Value.ToString();
                textBoxStock.Text = row.Cells["Stock"].Value.ToString();
            }
        }
    }
}
