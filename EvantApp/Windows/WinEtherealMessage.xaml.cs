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

namespace EvantApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для WinEtherealMessage.xaml
    /// </summary>
    public partial class WinEtherealMessage : Window
    {
        int filtPopularGroup;
        int filtMesCategies;
        int filtFederalDistrict;
        public WinEtherealMessage()
        {
            InitializeComponent();
            var listPopularGroup = Helper.GetContext().PopularGroup.Where(p=>p.PopularGroupEvent == Container.idEvent).ToList();
            listPopularGroup.Insert(0, new PopularGroup() { Content = "Все популярные группы" });
            CmbPopularGroup.ItemsSource = listPopularGroup;
            var listMessageCategories = Helper.GetContext().MessageCategories.ToList();
            listMessageCategories.Insert(0, new MessageCategories() { MessageCategorieName = "Все категории" });
            CmbMessCategories.ItemsSource = listMessageCategories;
            var listFederalDistrict = Helper.GetContext().FederalDistricts.ToList();
            listFederalDistrict.Insert(0, new FederalDistricts() { FederalDistrictName = "Все федераьные области" });
            CmbFederalDistrict.ItemsSource = listFederalDistrict;
            FillTable(filtPopularGroup, filtMesCategies, filtFederalDistrict);
        }

        private void FillTable(int filtPopularGroup, int filtMesCategies, int filtFederalDistrict)
        {
            var message = Helper.GetContext().Messages.Where(p=>p.MessegeEvent == Container.idEvent).ToList();
            var PopMessage = Helper.GetContext().PopularMessages.Where(p => p.PopularMessageMessageId == p.Messages.IdMessage).ToList();
            PopMessage = PopMessage.Where(p => p.PopularMessagePopularGroup == filtPopularGroup).ToList();
            if (CmbPopularGroup.SelectedIndex > 0)
            {
                try
                {
                    message = message.Where(p => p.IdMessage == PopMessage.First().PopularMessageMessageId).ToList();
                }
                catch
                {
                    message.Clear();
                }

            }
            if (CmbMessCategories.SelectedIndex > 0)
            {
                message = message.Where(p => p.MessageCategories == filtMesCategies).ToList();
            }
            if (CmbFederalDistrict.SelectedIndex > 0)
            {
                message = message.Where(p => p.User.FederalDistrictId == filtFederalDistrict).ToList();
            }
            DgMessage.ItemsSource = message.ToList();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            var staff = Helper.GetContext().Staff.Find(Container.idStaff);
            staff.Online = false;
            Helper.GetContext().SaveChanges();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void CmbPopularGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filtPopularGroup = int.Parse(CmbPopularGroup.SelectedValue.ToString());
            FillTable(filtPopularGroup, filtMesCategies, filtFederalDistrict);
        }

        private void CmbMessCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filtMesCategies = int.Parse(CmbMessCategories.SelectedValue.ToString());
            FillTable(filtPopularGroup, filtMesCategies, filtFederalDistrict);
        }

        private void CmbFederalDistrict_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filtFederalDistrict = int.Parse(CmbFederalDistrict.SelectedValue.ToString());
            FillTable(filtPopularGroup, filtMesCategies, filtFederalDistrict);
        }

        private void DgMessage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DgMessage.SelectedItem != null && DgMessage.SelectedItem is Messages messages)
            {
                TbMessageText.Text = messages.MessageText.ToString();
            }
        }
    }
}
