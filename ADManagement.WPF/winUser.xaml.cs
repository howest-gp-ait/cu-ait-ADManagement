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
using ADManagement.LIB.Services;
using ADManagement.LIB.Entities;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Runtime.Remoting.Lifetime;
using System.Diagnostics;

namespace ADManagement.WPF
{
    /// <summary>
    /// Interaction logic for winUser.xaml
    /// </summary>
    public partial class winUser : Window
    {
        public winUser()
        {
            InitializeComponent();
        }

        public bool isNew;
        public User user;
        public string ReferenceOU;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(isNew)
                user = new User();
            List<DirectoryEntry> allOUs = Stats.GetAllOUs();
            cmbOUs.Items.Clear();
            foreach (DirectoryEntry de in allOUs)
            {
                string waarde = de.Path.Replace(ActiveDirectory.LDAPLong , "");
                waarde = waarde.Replace(ActiveDirectory.LDAPShort, "");
                cmbOUs.Items.Add(waarde);
            }


            if (isNew)
            {
                txtFirstName.Text = "";
                txtLastname.Text = "";
                txtPassword.Text = "";
                txtUserName.Text = "";
                rdbExpiresNever.IsChecked = true;
                rdbExpiresAt.IsChecked = false;
                dtpExpirationDate.Visibility = Visibility.Hidden;
                chkActive.IsChecked = true;
                lstWel.Items.Clear();
            }
            else
            {
                cmbOUs.SelectedItem = user.OU;
                txtFirstName.Text = user.FirstName;
                txtLastname.Text = user.LastName;
                txtPassword.Text = "";
                txtUserName.Text = user.SamAccountName;
                chkActive.IsChecked = !user.IsAccountLockedOut;
                if (user.AccountExpirationDate == null)
                {
                    rdbExpiresNever.IsChecked = true;
                    rdbExpiresAt.IsChecked = false;
                    dtpExpirationDate.Visibility = Visibility.Hidden;
                }
                else
                {
                    rdbExpiresNever.IsChecked = false;
                    rdbExpiresAt.IsChecked = true;
                    dtpExpirationDate.Visibility = Visibility.Visible;
                    dtpExpirationDate.SelectedDate = (DateTime)user.AccountExpirationDate;
                }

            }
            vulListWel();
            vulListNiet();
        }
        private void vulListWel()
        {
            lstWel.Items.Clear();
            if (isNew)
            {
                // nieuwe gebruiker : deze list blijft leeg

            }
            else
            {
                // bestaande gebruikers : vul list met de groepen waar gebruiker lid van is
                if (user == null) return;
                foreach (Group g in user.MemberOff)
                    lstWel.Items.Add(g.Name);
            }

        }
        private void vulListNiet()
        {
            lstNiet.Items.Clear();
            if (isNew)
            {
                // nieuwe gebruiker dus alle groepen toevoegen aan deze listbox
                List<DirectoryEntry> allGroups = Stats.GetAllGroups();
                foreach(DirectoryEntry de in allGroups)
                {
                    lstNiet.Items.Add(de.Name.Replace("CN=", ""));
                }

            }
            else
            {
                // bestaande gebruiker : enkel groepen afbeelden waartoe deze gebruiker niet behoort
                if (user == null) return;
                List<DirectoryEntry> allGroups = Stats.GetAllGroups();
                lstNiet.Items.Clear();
                foreach (DirectoryEntry de in allGroups)
                {
                    bool gevonden = false;
                    string waarde = de.Name.Replace("CN=", "");
                    foreach (Group g in user.MemberOff)
                    {
                        if (g.Name == waarde)
                        {
                            gevonden = true;
                            break;
                        }
                    }
                    if (!gevonden)
                        lstNiet.Items.Add(waarde);
                }
            }
        }
        private void btnAddToGroup_Click(object sender, RoutedEventArgs e)
        {
            if (lstNiet.SelectedIndex == -1) return;
            lstWel.Items.Add(lstNiet.SelectedItem);
            lstNiet.Items.Remove(lstNiet.SelectedItem);
        }
        private void btnRemoveFromGroup_Click(object sender, RoutedEventArgs e)
        {
            if (lstWel.SelectedIndex == -1) return;
            lstNiet.Items.Add(lstWel.SelectedItem);
            lstWel.Items.Remove(lstWel.SelectedItem);
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ReferenceOU = "";
            this.Close();

        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(cmbOUs.SelectedIndex == -1)
            {
                cmbOUs.Focus();
                return;
            }
            string ou = cmbOUs.SelectedItem.ToString();

            string firstname = txtFirstName.Text.Trim();
            string lastname = txtLastname.Text.Trim();
            string samAccountName = txtUserName.Text.Trim();
            string paswoord = txtPassword.Text.Trim();
            if (isNew && paswoord.Length == 0)
            {
                txtPassword.Focus();
                return;
            }
            bool isAccountLockedOut = true;
            if (chkActive.IsChecked == true)
                isAccountLockedOut = false;
            DateTime? accountExpirationDate = null;
            if (rdbExpiresAt.IsChecked == true)
            {
                accountExpirationDate = dtpExpirationDate.SelectedDate;
            }
            List<string> groepnamen = new List<string>();
            //elke gebruiker MOET tot de groep "Domain Users" behoren, dus eventueel nog toevoegen aan de list

            bool isDomainUser = false;
            foreach (string groep in lstWel.Items)
            {
                groepnamen.Add(groep);
                if (groep == "Domain Users")
                    isDomainUser = true;
            }
            if (!isDomainUser)
                groepnamen.Add("Domain Users");

            user.DisplayName = firstname + " " + lastname;
            user.LoginName = samAccountName + ActiveDirectory.ADDomainEmail;
            user.FirstName = firstname;
            user.LastName = lastname;
            user.Email = samAccountName + ActiveDirectory.ADDomainEmail;
            user.SamAccountName = samAccountName;
            user.IsAccountLockedOut = isAccountLockedOut;
            user.AccountExpirationDate = accountExpirationDate;
            if (isNew)
            {
                string bericht = user.CreateThisUser(cmbOUs.SelectedItem.ToString(), paswoord);
                if (bericht != "")
                {
                    MessageBox.Show(bericht);
                    return;
                }
                UserGroupMembership.AddNewUserToGroups(samAccountName, groepnamen);
            }
            else
            {
                string bericht = user.UpdateThisUser(cmbOUs.SelectedItem.ToString(), paswoord);
                if (bericht != "")
                {
                    MessageBox.Show(bericht);
                    return;
                }
                UserGroupMembership.UpdateExistingUserGroupMembership(samAccountName, groepnamen);
            }
            ReferenceOU = ou;
            this.Close();

        }
    }
}
