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
using ADManagement.LIB.Helpers;

namespace ADManagement.WPF
{
    /// <summary>
    /// Interaction logic for winGroup.xaml
    /// </summary>
    public partial class winGroup : Window
    {
        public winGroup()
        {
            InitializeComponent();
        }
        public bool isNew;
        public Group group;
        public string ReferenceOU;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (isNew)
                group = new Group();
            List<DirectoryEntry> allOUs = Stats.GetAllOUs();
            cmbOUs.Items.Clear();
            foreach (DirectoryEntry de in allOUs)
            {
                string waarde = de.Path.Replace(ActiveDirectory.LDAPLong, "");
                waarde = waarde.Replace(ActiveDirectory.LDAPShort, "");
                cmbOUs.Items.Add(waarde);
            }
            if(isNew)
            {
                txtGroupName.Text = "";
                lstUsersWel.Items.Clear();
                lstGroupsWel.Items.Clear();
                vulLstUsersNiet();
                vulLstGroupsNiet();
            }
            else
            {
                txtGroupName.Text = group.Name;
                cmbOUs.SelectedItem = group.OU;

                vulLstUsersWel();
                vulLstUsersNiet();
                vulLstGroupsWel();
                vulLstGroupsNiet();
            }
        }
        private void vulLstUsersWel()
        {
            List<string> users = GroupMemberShip.MemberUsers(group.Name);
            foreach (string user in users)
            {
                lstUsersWel.Items.Add(user);
            }
        }
        private void vulLstUsersNiet()
        {
            lstUsersNiet.Items.Clear();
            List<DirectoryEntry> usersAll = Stats.GetAllUsers();
            List<string> usersMembers = GroupMemberShip.MemberUsers(group.Name);

            foreach (DirectoryEntry de in usersAll)
            {
                string waarde = Helper.HandleNull(de.Properties["SAMAccountName"][0]);
                if (waarde != "")
                {
                    bool gevonden = false;
                    foreach(string user in usersMembers)
                    {
                        if(user == waarde)
                        {
                            gevonden = true;
                            break;
                        }
                    }
                    if(!gevonden)
                        lstUsersNiet.Items.Add(waarde);
                }
            }
        }
        private void vulLstGroupsWel()
        {
            List<string> groups = GroupMemberShip.MemberGroups(group.Name);
            foreach (string group in groups)
            {
                lstGroupsWel.Items.Add(group);
            }
        }
        private void vulLstGroupsNiet()
        {
            lstGroupsNiet.Items.Clear();
            List<DirectoryEntry> groupsAll = Stats.GetAllGroups();
            List<string> groupsMember = GroupMemberShip.MemberGroups(group.Name);

            foreach (DirectoryEntry de in groupsAll)
            {
                string waarde = Helper.HandleNull(de.Properties["cn"][0]);
                if (waarde != "")
                {
                    bool gevonden = false;
                    foreach (string groep in groupsMember)
                    {
                        if (groep == waarde)
                        {
                            gevonden = true;
                            break;
                        }
                    }
                    if (!gevonden && waarde != group.Name)
                        lstGroupsNiet.Items.Add(waarde);
                }

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ReferenceOU = "";
            this.Close();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbOUs.SelectedIndex == -1)
            {
                cmbOUs.Focus();
                return;
            }
            string ou = cmbOUs.SelectedItem.ToString();
            string name = txtGroupName.Text.Trim();
            if(name.Length == 0)
            {
                txtGroupName.Focus();
                return;
            }
            List<string> usersInGroup = new List<string>();
            List<string> groupsInGroup = new List<string>();
            foreach (string gebruiker in lstUsersWel.Items)
            {
                usersInGroup.Add(gebruiker);
            }
            foreach (string groep in lstGroupsWel.Items)
            {
                groupsInGroup.Add(groep);
            }

            if (isNew)
            {
                group.Name = name;
                string bericht = group.CreateThisGroup(cmbOUs.SelectedItem.ToString());
                if (bericht != "")
                {
                    MessageBox.Show(bericht);
                    return;
                }
            }
            else
            {
                string bericht = group.UpdateThisGroup(cmbOUs.SelectedItem.ToString(),name);
                if (bericht != "")
                {
                    MessageBox.Show(bericht);
                    return;
                }
            }
            GroupMemberShip.AddUsersToThisGroup(group.Name, usersInGroup);
            GroupMemberShip.AddGroupsToThisGroup(group.Name, groupsInGroup);
            ReferenceOU = ou;
            this.Close();

        }


        private void btnAddUserToGroup_Click(object sender, RoutedEventArgs e)
        {
            if (lstUsersNiet.SelectedIndex == -1) return;
            lstUsersWel.Items.Add(lstUsersNiet.SelectedItem);
            lstUsersNiet.Items.Remove(lstUsersNiet.SelectedItem);

        }

        private void btnRemoveUserFromGroup_Click(object sender, RoutedEventArgs e)
        {
            if (lstUsersWel.SelectedIndex == -1) return;
            lstUsersNiet.Items.Add(lstUsersWel.SelectedItem);
            lstUsersWel.Items.Remove(lstUsersWel.SelectedItem);

        }

        private void btnAddGroupToGroup_Click(object sender, RoutedEventArgs e)
        {
            if (lstGroupsNiet.SelectedIndex == -1) return;
            lstGroupsWel.Items.Add(lstGroupsNiet.SelectedItem);
            lstGroupsNiet.Items.Remove(lstGroupsNiet.SelectedItem);

        }

        private void btnRemoveGroupFromGroup_Click(object sender, RoutedEventArgs e)
        {
            if (lstGroupsWel.SelectedIndex == -1) return;
            lstGroupsNiet.Items.Add(lstGroupsWel.SelectedItem);
            lstGroupsWel.Items.Remove(lstGroupsWel.SelectedItem);


        }
    }
}
