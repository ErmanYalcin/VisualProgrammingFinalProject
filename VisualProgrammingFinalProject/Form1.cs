using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisualProgrammingFinalProject.Model;

namespace VisualProgrammingFinalProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var cities = GetCities();
            cmbCities.DataSource = cities;
            cmbCities.DisplayMember = "title";
            cmbCities.ValueMember = "id"; // Burada id doğru
            cmbCities.SelectedIndex = -1; // İlk başta bir seçim olmasın
        }

        private void cmbCities_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCities.SelectedValue != null && cmbCities.SelectedValue is int)
            {
                int cityId = (int)cmbCities.SelectedValue; // SelectedIndex yerine SelectedValue kullanarak id'yi alın
                var counties = GetCountiesByCity(cityId);
                cmbCounties.DataSource = counties;
                cmbCounties.DisplayMember = "title";
                cmbCounties.ValueMember = "id"; // Burada id doğru
            }
        }


        #region Database Functions

        #region Login Functions
        public bool Login(string username, string password)
        {
            using (var context = new SchoolManagementSystemEntities())
            {
                // Assuming PasswordHash is already a hashed value
                return context.Users.Any(user => user.Username == username && user.PasswordHash == password);
            }
        }

        #endregion

        #region Student Functions
        public void AddStudent(Student student)
        {
            using (var context = new SchoolManagementSystemEntities())
            {
                context.Students.Add(student);
                context.SaveChanges();
            }
        }
        public void UpdateStudent(Student student)
        {
            using (var context = new SchoolManagementSystemEntities())
            {
                context.Entry(student).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public void DeleteStudent(int studentId)
        {
            using (var context = new SchoolManagementSystemEntities())
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
            using (var context = new SchoolManagementSystemEntities())
            {
                return context.Students.Find(studentId);
            }
        }

        public List<Student> GetAllStudents()
        {
            using (var context = new SchoolManagementSystemEntities())
            {
                return context.Students.ToList();
            }
        }
        #endregion

        #region Parent Functions
        public void AddParent(Parent parent)
        {
            using (var context = new SchoolManagementSystemEntities())
            {
                context.Parents.Add(parent);
                context.SaveChanges();
            }
        }
        public void UpdateParent(Parent parent)
        {
            using (var context = new SchoolManagementSystemEntities())
            {
                context.Entry(parent).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public void DeleteParent(int parentId)
        {
            using (var context = new SchoolManagementSystemEntities())
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
            using (var context = new SchoolManagementSystemEntities())
            {
                return context.Parents.Find(parentId);
            }
        }

        public List<Parent> GetAllParents()
        {
            using (var context = new SchoolManagementSystemEntities())
            {
                return context.Parents.ToList();
            }
        }
        #endregion

        #region Cafeteria Functions
        public List<CafeteriaPurchas> GetCafeteriaPurchases(int studentId)
        {
            using (var context = new SchoolManagementSystemEntities())
            {
                return context.CafeteriaPurchases.Where(p => p.StudentID == studentId).ToList();
            }
        }
        public bool MakePurchase(int studentId, int cafeteriaItemId, int quantity)
        {
            using (var context = new SchoolManagementSystemEntities())
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
            using (var context = new SchoolManagementSystemEntities())
            {
                var student = context.Students.Find(studentId);
                var cafeteriaItem = context.CafeteriaItems.Find(cafeteriaItemId);

                if (student != null && cafeteriaItem != null)
                {
                    // Alışveriş kısıtlamalarını kontrol et
                    var restrictions = context.PurchaseRestrictions
                        .Where(r => r.StudentID == studentId && r.CafeteriaItemID == cafeteriaItemId)
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
        public List<City> GetCities()
        {
            using (var context = new SchoolManagementSystemEntities())
            {
                return context.Cities.ToList();
            }
        }

        public List<County> GetCountiesByCity(int cityId)
        {
            using (var context = new SchoolManagementSystemEntities())
            {
                return context.Counties.Where(c => c.city_id == cityId).ToList();
            }
        }

        #endregion
        #endregion
    }
}