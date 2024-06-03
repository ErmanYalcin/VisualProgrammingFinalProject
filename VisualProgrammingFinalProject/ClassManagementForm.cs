using System;
using System.Linq;
using System.Windows.Forms;
using System.Data.Entity;
using VisualProgrammingFinalProject.Model;

namespace VisualProgrammingFinalProject
{
    public partial class ClassManagementForm : Form
    {
        public ClassManagementForm()
        {
            InitializeComponent();
        }

        private void ClassManagementForm_Load(object sender, EventArgs e)
        {
            LoadClasses();
            LoadTeachers();
        }

        private void LoadClasses()
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var classList = context.Classes
                    .Include(c => c.Teacher)
                    .Include(c => c.Students)
                    .ToList() // Verileri önce çekiyoruz
                    .Select(c => new
                    {
                        c.ClassID,
                        c.ClassName,
                        c.TeacherID,
                        TeacherName = c.Teacher.FirstName + " " + c.Teacher.LastName,
                        Students = string.Join(", ", c.Students.Select(s => s.FirstName + " " + s.LastName))
                    })
                    .ToList();

                dataGridViewClasses.DataSource = classList;
            }
        }

        private void LoadTeachers()
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var teachers = context.Teachers
                    .Select(t => new
                    {
                        t.TeacherID,
                        FullName = t.FirstName + " " + t.LastName
                    })
                    .ToList();

                comboBoxTeacher.DataSource = teachers;
                comboBoxTeacher.DisplayMember = "FullName";
                comboBoxTeacher.ValueMember = "TeacherID";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var newClass = new Class
                {
                    ClassName = textBoxClassName.Text,
                    TeacherID = (int)comboBoxTeacher.SelectedValue
                };
                context.Classes.Add(newClass);
                context.SaveChanges();
                LoadClasses();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewClasses.SelectedRows.Count > 0)
            {
                using (var context = new SchoolManagementSystemEntities1())
                {
                    var row = dataGridViewClasses.SelectedRows[0];
                    int classId = (int)row.Cells["ClassID"].Value;
                    var classToUpdate = context.Classes.FirstOrDefault(c => c.ClassID == classId);

                    if (classToUpdate != null)
                    {
                        classToUpdate.ClassName = textBoxClassName.Text;

                        if (comboBoxTeacher.SelectedValue != null)
                        {
                            classToUpdate.TeacherID = (int)comboBoxTeacher.SelectedValue;
                        }
                        else
                        {
                            MessageBox.Show("Please select a teacher.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        context.SaveChanges();
                        LoadClasses();
                    }
                }
            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewClasses.SelectedRows.Count > 0)
            {
                using (var context = new SchoolManagementSystemEntities1())
                {
                    var row = dataGridViewClasses.SelectedRows[0];
                    int classId = (int)row.Cells["ClassID"].Value;
                    var classToDelete = context.Classes.FirstOrDefault(c => c.ClassID == classId);

                    if (classToDelete != null)
                    {
                        context.Classes.Remove(classToDelete);
                        context.SaveChanges();
                        LoadClasses();
                    }
                }
            }
        }

        private void dataGridViewClasses_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewClasses.SelectedRows.Count > 0)
            {
                var row = dataGridViewClasses.SelectedRows[0];
                textBoxClassName.Text = row.Cells["ClassName"].Value.ToString();
                comboBoxTeacher.SelectedValue = row.Cells["TeacherID"].Value;
            }
        }
    }
}
