using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using VisualProgrammingFinalProject.Model;

namespace VisualProgrammingFinalProject
{
    public partial class Form1 : Form
    {
        private string loggedInUser;
        private string userRole;

        public Form1(string username)
        {
            InitializeComponent();
            loggedInUser = username;
            this.Text = $"Welcome, {loggedInUser}";

            SetUserRole();
            SetupButtonsBasedOnRole();

            var cities = GetCities();
        }

        private void SetUserRole()
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == loggedInUser);
                if (user != null)
                {
                    userRole = user.Role;
                }
            }
        }

        private void SetupButtonsBasedOnRole()
        {
            if (userRole == "management")
            {
                // Okul Yönetimi Butonları
                btnStudentManagement.Visible = true;
                btnTeacherManagement.Visible = true;
                btnParentManagement.Visible = true;
                btnClassManagement.Visible = true;

                // Kantin Butonları
                btnCafeteriaManagement.Visible = false;
            }
            else if (userRole == "canteen")
            {
                // Kantin Butonları
                btnCafeteriaManagement.Visible = true;

                // Okul Yönetimi Butonları
                btnStudentManagement.Visible = false;
                btnTeacherManagement.Visible = false;
                btnParentManagement.Visible = false;
                btnClassManagement.Visible = false;
            }
        }

        public void loadform(object Form)
        {
            if (this.mainpanel.Controls.Count > 0)
                this.mainpanel.Controls.RemoveAt(0);
            Form f = Form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            this.mainpanel.Controls.Add(f);
            this.mainpanel.Tag = f;
            f.Show();
        }

        private void btnStudentManagement_Click(object sender, EventArgs e)
        {
            loadform(new StudentManagementForm());
        }

        private void btnTeacherManagement_Click(object sender, EventArgs e)
        {
            loadform(new TeacherManagementForm());
        }

        private void btnParentManagement_Click(object sender, EventArgs e)
        {
            loadform(new ParentManagementForm());
        }

        private void btnClassManagement_Click(object sender, EventArgs e)
        {
            loadform(new ClassManagementForm());
        }

        private void btnCafeteriaManagement_Click(object sender, EventArgs e)
        {
            loadform(new CafeteriaManagementForm());
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #region Database Functions

        #region Login Functions
        public bool Login(string username, string password)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                return context.Users.Any(user => user.Username == username && user.PasswordHash == password);
            }
        }

        #endregion

        #region Student Functions
        public void AddStudent(Student student)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                context.Students.Add(student);
                context.SaveChanges();
            }
        }
        public void UpdateStudent(Student student)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                context.Entry(student).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public void DeleteStudent(int studentId)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var student = context.Students.Find(studentId);
                if (student != null)
                {
                    context.Students.Remove(student);
                    context.SaveChanges();
                }
            }
        }
        public Student GetStudent(int studentId)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                return context.Students.Find(studentId);
            }
        }

        public List<Student> GetAllStudents()
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                return context.Students.ToList();
            }
        }
        #endregion

        #region Parent Functions
        public void AddParent(Parent parent)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                context.Parents.Add(parent);
                context.SaveChanges();
            }
        }
        public void UpdateParent(Parent parent)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                context.Entry(parent).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public void DeleteParent(int parentId)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var parent = context.Parents.Find(parentId);
                if (parent != null)
                {
                    context.Parents.Remove(parent);
                    context.SaveChanges();
                }
            }
        }
        public Parent GetParent(int parentId)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                return context.Parents.Find(parentId);
            }
        }

        public List<Parent> GetAllParents()
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                return context.Parents.ToList();
            }
        }
        #endregion

        #region Cafeteria Functions
        public List<CafeteriaPurchas> GetCafeteriaPurchases(int studentId)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                return context.CafeteriaPurchases.Where(p => p.StudentID == studentId).ToList();
            }
        }
        public bool MakePurchase(int studentId, int cafeteriaItemId, int quantity)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var student = context.Students.Find(studentId);
                var cafeteriaItem = context.CafeteriaItems.Find(cafeteriaItemId);

                if (student != null && cafeteriaItem != null && cafeteriaItem.Stock >= quantity)
                {
                    decimal totalCost = cafeteriaItem.Price * quantity;
                    if (student.Balance >= totalCost)
                    {
                        var purchase = new CafeteriaPurchas
                        {
                            StudentID = studentId,
                            ItemID = cafeteriaItemId,
                            Quantity = quantity,
                            PurchaseDate = DateTime.Now
                        };

                        student.Balance -= totalCost;
                        cafeteriaItem.Stock -= quantity;
                        context.CafeteriaPurchases.Add(purchase);
                        context.SaveChanges();
                        return true;
                    }
                }
                return false;
            }
        }
        public bool CanPurchase(int studentId, int cafeteriaItemId, int quantity)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                var student = context.Students.Find(studentId);
                var cafeteriaItem = context.CafeteriaItems.Find(cafeteriaItemId);

                if (student != null && cafeteriaItem != null)
                {
                    // Alışveriş kısıtlamalarını kontrol et
                    var restrictions = context.ParentRestrictions
                        .Where(r => r.StudentID == studentId && r.RestrictedItemID == cafeteriaItemId)
                        .FirstOrDefault();

                    if (restrictions != null)
                    {
                        return false;
                    }

                    // Öğrencinin bakiyesinin yeterli olup olmadığını kontrol et
                    decimal totalCost = cafeteriaItem.Price * quantity;
                    if (student.Balance >= totalCost)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        #endregion

        #region City and County Functions
        public List<city> GetCities()
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                return context.cities.ToList();
            }
        }

        public List<county> GetCountiesByCity(int cityId)
        {
            using (var context = new SchoolManagementSystemEntities1())
            {
                return context.counties.Where(c => c.city_id == cityId).ToList();
            }
        }

        #endregion
        #endregion

        #region Logout Function
        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Hide(); // Form1'i gizle
            loginform loginForm = new loginform();
            loginForm.ShowDialog(); // Login formunu göster
            this.Close(); // Form1'i kapat
        }
        #endregion
    }
}
