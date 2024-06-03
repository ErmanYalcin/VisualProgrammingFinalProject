using System;
using System.Linq;
using System.Windows.Forms;
using System.Data.Entity;
using VisualProgrammingFinalProject.Model;

namespace VisualProgrammingFinalProject
{
    public partial class TeacherManagementForm : Form
    {
        public TeacherManagementForm()
        {
            InitializeComponent();
        }

        private void TeacherManagementForm_Load(object sender, EventArgs e)
        {
            LoadTeachers();
        }

        private void LoadTeachers()
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var teachers = context.Teachers
                    .Include(t => t.Classes)
                    .ToList() // Verileri önce çekiyoruz
                    .Select(t => new
                    {
                        t.TeacherID,
                        t.FirstName,
                        t.LastName,
                        t.Subject,
                        t.Email,
                        t.PhoneNumber,
                        Classes = string.Join(", ", t.Classes.Select(c => c.ClassName))
                    })
                    .ToList();

                dataGridViewTeachers.DataSource = teachers;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var teacher = new Teacher
                {
                    FirstName = textBoxFirstName.Text,
                    LastName = textBoxLastName.Text,
                    Subject = textBoxSubject.Text,
                    Email = textBoxEmail.Text,
                    PhoneNumber = textBoxPhoneNumber.Text
                };
                context.Teachers.Add(teacher);
                context.SaveChanges();
                LoadTeachers();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewTeachers.SelectedRows.Count > 0)
            {
                using (var context = new SchoolManagementSystemEntities1())
                {
                    var row = dataGridViewTeachers.SelectedRows[0];
                    int teacherId = (int)row.Cells["TeacherID"].Value;
                    var teacher = context.Teachers.Include(t => t.Classes).FirstOrDefault(t => t.TeacherID == teacherId);

                    if (teacher != null)
                    {
                        teacher.FirstName = textBoxFirstName.Text;
                        teacher.LastName = textBoxLastName.Text;
                        teacher.Subject = textBoxSubject.Text;
                        teacher.Email = textBoxEmail.Text;
                        teacher.PhoneNumber = textBoxPhoneNumber.Text;
                        context.SaveChanges();
                        LoadTeachers();
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewTeachers.SelectedRows.Count > 0)
            {
                using (var context = new SchoolManagementSystemEntities1())
                {
                    var row = dataGridViewTeachers.SelectedRows[0];
                    int teacherId = (int)row.Cells["TeacherID"].Value;
                    var teacher = context.Teachers.FirstOrDefault(t => t.TeacherID == teacherId);

                    if (teacher != null)
                    {
                        context.Teachers.Remove(teacher);
                        context.SaveChanges();
                        LoadTeachers();
                    }
                }
            }
        }

        private void dataGridViewTeachers_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewTeachers.SelectedRows.Count > 0)
            {
                var row = dataGridViewTeachers.SelectedRows[0];
                textBoxFirstName.Text = row.Cells["FirstName"].Value.ToString();
                textBoxLastName.Text = row.Cells["LastName"].Value.ToString();
                textBoxSubject.Text = row.Cells["Subject"].Value.ToString();
                textBoxEmail.Text = row.Cells["Email"].Value.ToString();
                textBoxPhoneNumber.Text = row.Cells["PhoneNumber"].Value.ToString();
            }
        }
    }
}
