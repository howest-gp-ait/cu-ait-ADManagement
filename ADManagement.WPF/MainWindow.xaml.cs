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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ADManagement.LIB.Services;
using ADManagement.LIB.Entities;
using ADManagement.LIB.Helpers;

namespace ADManagement.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BouwTV();
            grpUser.Visibility = Visibility.Hidden;
            grpGroup.Visibility = Visibility.Hidden;
            grpGroup.Margin = grpUser.Margin;
        }
        private void BouwTV()
        {
            OrgUnits ous = new OrgUnits();
            TVOU.Items.Clear();
            foreach (OrgUnit ou in ous.OUs)
            {
                TreeViewItem itm = new TreeViewItem();
                itm.Tag = ou.Tag;
                itm.Header = ou.Name;
                BouwTVVerder(itm, ou);
                TVOU.Items.Add(itm);
            }
        }
        private void BouwTVVerder(TreeViewItem parent_tv_itm, OrgUnit parent_ou)
        {
            foreach (OrgUnit ou in parent_ou.Childeren)
            {
                TreeViewItem itm = new TreeViewItem();
                itm.Tag = ou.Tag;
                itm.Header = ou.Name;
                BouwTVVerder(itm, ou);
                parent_tv_itm.Items.Add(itm);
            }

        }

        private void TVOU_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            grpUser.Visibility = Visibility.Hidden;
            grpGroup.Visibility = Visibility.Hidden;

            TreeViewItem itm = (TreeViewItem) TVOU.SelectedItem;

            UsersInOU users = new UsersInOU(itm.Tag.ToString());
            lstUsers.ItemsSource = users.Users;

            GroupsInOU groups = new GroupsInOU(itm.Tag.ToString());
            lstGroups.ItemsSource = groups.Groups;
        }

        private void lstUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            grpUser.Visibility = Visibility.Hidden;
            if (lstUsers.SelectedIndex == -1) return;

            grpUser.Visibility = Visibility.Visible;
            User user = (User)lstUsers.SelectedItem;
            lblUserDisplayName.Content = user.DisplayName;
            lblUserFirstName.Content = user.FirstName;
            lblUserLastName.Content = user.LastName;
            lblUserUserName.Content = user.LoginName;
            lstMemberOff.ItemsSource = user.MemberOff;

        }

        private void lstGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            grpGroup.Visibility = Visibility.Hidden;
            if (lstGroups.SelectedIndex == -1) return;

            grpGroup.Visibility = Visibility.Visible;
            Group group = (Group)lstGroups.SelectedItem;
            lblGroupName.Content = group.Name;
            lstUsersInGroup.ItemsSource = group.UserMembers;
            lstGroupsInGroup.ItemsSource = group.GroupMembers;
        }

        private void btnNewUser_Click(object sender, RoutedEventArgs e)
        {
            winUser win = new winUser();
            win.isNew = true;
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ShowDialog();
            if (Helper.HandleNull(win.ReferenceOU) != "")
                SelectOU(win.ReferenceOU);

        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            winUser win = new winUser();
            win.isNew = false;
            win.user = (User)lstUsers.SelectedItem;
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ShowDialog();
            if (Helper.HandleNull(win.ReferenceOU) != "")
                SelectOU(win.ReferenceOU);
        }
        private void SelectOU(string zoekOU)
        {
            if (zoekOU.Substring(0, 4) != "LDAP")
                zoekOU = ActiveDirectory.LDAPShort + zoekOU;
            foreach (TreeViewItem itm in TVOU.Items)
            {
                if(itm.Tag.ToString() == zoekOU)
                {
                    itm.IsSelected = true;
                    TVOU_SelectedItemChanged(null, null);
                    return;
                }
                if (SelectOUVerder(zoekOU, itm))
                    return;
            }
        }
        private bool SelectOUVerder(string zoekOU, TreeViewItem itm)
        {
            foreach (TreeViewItem subitem in itm.Items)
            {
                if (subitem.Tag.ToString() == zoekOU)
                {
                    subitem.IsSelected = true;
                    TVOU_SelectedItemChanged(null, null);
                    return true;
                }
            }
            return false;
        }
        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Zeker?","Gebruiker wissen",MessageBoxButton.YesNo, MessageBoxImage.Question,MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                User user = (User)lstUsers.SelectedItem;
                user.DeleteThisUser();
                TVOU_SelectedItemChanged(null, null);
            }

        }

        private void btnNewGroup_Click(object sender, RoutedEventArgs e)
        {
            winGroup win = new winGroup();
            win.isNew = true;
            win.ShowDialog();
            if (Helper.HandleNull(win.ReferenceOU) != "")
                SelectOU(win.ReferenceOU);
        }

        private void btnEditGroup_Click(object sender, RoutedEventArgs e)
        {
            winGroup win = new winGroup();
            win.isNew = false;
            win.group = (Group)lstGroups.SelectedItem;
            win.ShowDialog();
            if (Helper.HandleNull(win.ReferenceOU) != "")
                SelectOU(win.ReferenceOU);
        }

        private void btnDeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Zeker?", "Groep wissen", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                Group groep = (Group)lstGroups.SelectedItem;
                groep.DeleteThisGroup();
                TVOU_SelectedItemChanged(null, null);
            }
        }
    }
}
