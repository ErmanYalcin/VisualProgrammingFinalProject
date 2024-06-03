using System;
using System.Linq;
using System.Windows.Forms;
using VisualProgrammingFinalProject.Model; // Add the namespace for your Entity Framework context

namespace VisualProgrammingFinalProject
{
    public partial class loginform : Form
    {
        public loginform()
        {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text;
            string password = textBoxPassword.Text;

            // Veritabanı doğrulaması
            using (var context = new SchoolManagementSystemEntities1())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);
                if (user != null)
                {
                    MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide(); // loginform'u gizle
                    Form1 mainForm = new Form1(user.Username); // Ana formu açarken kullanıcı adını geç
                    mainForm.ShowDialog();
                    this.Close(); // Uygulamayı kapat
                }
                else
                {
                    labelErrorMessage.Text = "Invalid username or password";
                }
            }
        }

        private void loginform_Load(object sender, EventArgs e)
        {

        }
    }
}
