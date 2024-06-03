using System;
using System.Linq;
using System.Windows.Forms;
using System.Data.Entity;
using VisualProgrammingFinalProject.Model;

namespace VisualProgrammingFinalProject
{
    public partial class ParentManagementForm : Form
    {
        public ParentManagementForm()
        {
            InitializeComponent();
        }

        private void ParentManagementForm_Load(object sender, EventArgs e)
        {
            LoadParents();
        }

        private void LoadParents()
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var parents = context.Parents
                    .Select(p => new
                    {
                        p.ParentID,
                        p.FirstName,
                        p.LastName,
                        p.PhoneNumber,
                        p.Email,
                        p.Occupation,
                        p.Notes
                    })
                    .ToList();

                dataGridViewParents.DataSource = parents;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var parent = new Parent
                {
                    FirstName = textBoxFirstName.Text,
                    LastName = textBoxLastName.Text,
                    PhoneNumber = textBoxPhoneNumber.Text,
                    Email = textBoxEmail.Text,
                    Occupation = textBoxOccupation.Text,
                    Notes = textBoxNotes.Text
                };
                context.Parents.Add(parent);
                context.SaveChanges();
                LoadParents();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewParents.SelectedRows.Count > 0)
            {
                using (var context = new SchoolManagementSystemEntities1())
                {
                    var row = dataGridViewParents.SelectedRows[0];
                    int parentId = (int)row.Cells["ParentID"].Value;
                    var parent = context.Parents.FirstOrDefault(p => p.ParentID == parentId);

                    if (parent != null)
                    {
                        parent.FirstName = textBoxFirstName.Text;
                        parent.LastName = textBoxLastName.Text;
                        parent.PhoneNumber = textBoxPhoneNumber.Text;
                        parent.Email = textBoxEmail.Text;
                        parent.Occupation = textBoxOccupation.Text;
                        parent.Notes = textBoxNotes.Text;
                        context.SaveChanges();
                        LoadParents();
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewParents.SelectedRows.Count > 0)
            {
                using (var context = new SchoolManagementSystemEntities1())
                {
                    var row = dataGridViewParents.SelectedRows[0];
                    int parentId = (int)row.Cells["ParentID"].Value;
                    var parent = context.Parents.FirstOrDefault(p => p.ParentID == parentId);

                    if (parent != null)
                    {
                        context.Parents.Remove(parent);
                        context.SaveChanges();
                        LoadParents();
                    }
                }
            }
        }

        private void dataGridViewParents_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewParents.SelectedRows.Count > 0)
            {
                var row = dataGridViewParents.SelectedRows[0];
                textBoxFirstName.Text = row.Cells["FirstName"].Value.ToString();
                textBoxLastName.Text = row.Cells["LastName"].Value.ToString();
                textBoxPhoneNumber.Text = row.Cells["PhoneNumber"].Value.ToString();
                textBoxEmail.Text = row.Cells["Email"].Value.ToString();
                textBoxOccupation.Text = row.Cells["Occupation"].Value.ToString();
                textBoxNotes.Text = row.Cells["Notes"].Value.ToString();
            }
        }
    }
}
