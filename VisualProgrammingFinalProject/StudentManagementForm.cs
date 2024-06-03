using System;
using System.Linq;
using System.Windows.Forms;
using System.Data.Entity;
using VisualProgrammingFinalProject.Model;

namespace VisualProgrammingFinalProject
{
    public partial class StudentManagementForm : Form
    {
        public StudentManagementForm()
        {
            InitializeComponent();
        }

        private void StudentManagementForm_Load(object sender, EventArgs e)
        {
            LoadStudents();
            LoadParents();
            LoadClasses();
        }

        private void LoadStudents()
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var students = context.Students
                    .Include(s => s.Parent)
                    .Include(s => s.Class)
                    .ToList()
                    .Select(s => new StudentViewModel
                    {
                        StudentID = s.StudentID,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        DateOfBirth = s.DateOfBirth,
                        Address = s.Address,
                        ParentID = s.ParentID,
                        ParentName = s.Parent != null ? s.Parent.FirstName + " " + s.Parent.LastName : "N/A",
                        ClassID = s.ClassID,
                        ClassName = s.Class != null ? s.Class.ClassName : "N/A"
                    })
                    .ToList();
                dataGridViewStudents.DataSource = students;
            }
        }

        private void LoadParents()
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var parents = context.Parents.ToList();
                comboBoxParent.DataSource = parents;
                comboBoxParent.DisplayMember = "FirstName"; // Adjust as needed
                comboBoxParent.ValueMember = "ParentID";
            }
        }

        private void LoadClasses()
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var classes = context.Classes.ToList();
                comboBoxClass.DataSource = classes;
                comboBoxClass.DisplayMember = "ClassName"; // Adjust as needed
                comboBoxClass.ValueMember = "ClassID";
            }
        }

        private void dataGridViewStudents_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewStudents.SelectedRows.Count > 0)
            {
                var row = dataGridViewStudents.SelectedRows[0];
                textBoxFirstName.Text = row.Cells["FirstName"].Value.ToString();
                textBoxLastName.Text = row.Cells["LastName"].Value.ToString();
                dateTimePickerDOB.Value = (DateTime)row.Cells["DateOfBirth"].Value;
                textBoxAddress.Text = row.Cells["Address"].Value.ToString();
                comboBoxParent.SelectedValue = row.Cells["ParentID"].Value;
                comboBoxClass.SelectedValue = row.Cells["ClassID"].Value;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var student = new Student
                {
                    FirstName = textBoxFirstName.Text,
                    LastName = textBoxLastName.Text,
                    DateOfBirth = dateTimePickerDOB.Value,
                    Address = textBoxAddress.Text,
                    ParentID = (int?)comboBoxParent.SelectedValue,
                    ClassID = (int?)comboBoxClass.SelectedValue,
                };

                context.Students.Add(student);
                context.SaveChanges();
                LoadStudents();
            }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewStudents.SelectedRows.Count > 0)
            {
                using (var context = new SchoolManagementSystemEntities1())
                {
                    var row = dataGridViewStudents.SelectedRows[0];
                    int studentId = (int)row.Cells["StudentID"].Value;
                    var student = context.Students.FirstOrDefault(s => s.StudentID == studentId);

                    if (student != null)
                    {
                        student.FirstName = textBoxFirstName.Text;
                        student.LastName = textBoxLastName.Text;
                        student.DateOfBirth = dateTimePickerDOB.Value;
                        student.Address = textBoxAddress.Text;
                        student.ParentID = (int?)comboBoxParent.SelectedValue;
                        student.ClassID = (int?)comboBoxClass.SelectedValue;
                        context.SaveChanges();
                        LoadStudents();
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewStudents.SelectedRows.Count > 0)
            {
                using (var context = new SchoolManagementSystemEntities1())
                {
                    var row = dataGridViewStudents.SelectedRows[0];
                    int studentId = (int)row.Cells["StudentID"].Value;
                    var student = context.Students.FirstOrDefault(s => s.StudentID == studentId);

                    if (student != null)
                    {
                        context.Students.Remove(student);
                        context.SaveChanges();
                        LoadStudents();
                    }
                }
            }
        }
    }
}
