using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tenant_Management_System
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Window
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            //inputs
            //emailTbx - email input
            //passwordTbx - password input
            //confirmpasswordTbx - confirm password input
            //registererrorLbl - error message label


            //pass confirmed pass and emai cant be empty
            //pass must be 6 characters long
            //pass and confirm pass must match
            //email must be valid
        }

        private void loginLinkTxt_Click(object sender, RoutedEventArgs e)
        {
            //loginLinkTxt - login link
            //when clicked, it should open the login page

        }
    }
}
