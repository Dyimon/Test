using Microsoft.VisualBasic;
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
using System.Windows.Threading;

namespace EvantApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для WinModerator.xaml
    /// </summary>
    public partial class WinModerator : Window
    {
        int fillDistrict;
        int fillStatus;
        int fillCategory;
        int fillProcessed;
        int status;
        public WinModerator()
        {
            InitializeComponent();
            // Заполнение выпадающего списка
            var categories = Helper.GetContext().MessageCategories.ToList();
            categories.Insert(0, new MessageCategories() { MessageCategorieName="Все категории" }); // Добавение нового элемента в начало списка переменной 
            CmbMesCategory.ItemsSource = categories;
            var districts = Helper.GetContext().FederalDistricts.ToList();
            districts.Insert(0, new FederalDistricts() { FederalDistrictName = "Все оласти" });
            CmbFedDistrict.ItemsSource = districts;
            var status = Helper.GetContext().StatusMessage.ToList();
            status.Insert(0, new StatusMessage() { StatusMessage1 = "Все статусы" });
            CmbLastMesStatus.ItemsSource = status;
            
            FillTable(fillDistrict, fillStatus, fillCategory, fillProcessed);
            CmbProcessed.SelectedIndex = 0;
            var events = Helper.GetContext().Events.Find(Container.idEvent);
            if(events.DateTimeBegin<DateTime.Now && events.DateTimeEnd > DateTime.Now)
            {
                BtnInEther.IsEnabled = true;
                BtnInGroupPopular.IsEnabled = true;
            }
            else
            {
                BtnInEther.IsEnabled = false;
                BtnInGroupPopular.IsEnabled = false;
            }
            
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += TimeUpdate;
            timer.Start();
        }
        DispatcherTimer timer = new DispatcherTimer();
        private void TimeUpdate(object sender, EventArgs e)
        {
            FillTable(fillDistrict, fillStatus, fillCategory, fillProcessed);
        }

        // Обновление данных таблицы
        private void FillTable(int fillDistrict, int fillStatus,int fillCategory, int fillProcessed)
        {
            CmbPopularGroup.ItemsSource = Helper.GetContext().PopularGroup.Where(p => p.PopularGroupEvent == Container.idEvent).ToList();
            // Создаем переменную которая хранит данные таблицы
            var message = Helper.GetContext().MessageProcessing.Where(p => p.MessageProcessingStaff == Container.idStaff && p.Messages.MessegeEvent == Container.idEvent).ToList();
            // Начало фильтрации данных
            if (CmbMesCategory.SelectedIndex > 0)
            {
                message = message.Where(p => p.Messages.MessageCategories == fillCategory).ToList();
            }
            if (CmbFedDistrict.SelectedIndex > 0)
            {
                message = message.Where(p => p.Messages.User.FederalDistrictId == fillDistrict).ToList();
            }
            if (CmbLastMesStatus.SelectedIndex > 0)
            {
                message = message.Where(p => p.MessageProcessingStatusMessage == fillStatus).ToList();
            }
            if(CmbProcessed.SelectedIndex > 0)
            {
                switch (fillProcessed)
                {
                    case 1:
                        message = message.Where(p => p.Messages.Processed == true).ToList();
                        break;
                    case 2:
                        message = message.Where(p => p.Messages.Processed == false).ToList();
                        break;
                }
            }
            
            // Конец фильтрации данных
            DgMessage.ItemsSource = message.ToList();
        }
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            var user = Helper.GetContext().Staff.Find(Container.idStaff);
            user.Online = false;
            Helper.GetContext().SaveChanges();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Visibility = Visibility.Visible;
            Close();
        }

        private void DgMessage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TbError.Text = "";
            if (DgMessage.SelectedItem != null && DgMessage.SelectedItem is MessageProcessing procesMessege)
            {
                TbTextMessage.Text = procesMessege.Messages.MessageText;
                var userMes = procesMessege.Messages.User;
                TbUserInfo.Text = userMes.SecondName + " " + userMes.FirstName + " " + userMes.Patronymic + " Возраст: " + userMes.Age + " Номер телефона: " + userMes.Phone + " Почта: " + userMes.Email;
                if (procesMessege.Messages.MediaContent != null)
                {
                    string link = @"Resources/" + procesMessege.Messages.MediaContent;
                    Uri uri = new Uri(link, UriKind.Relative);
                    MeVideo.Source = uri;
                }
                else
                {
                    MeVideo.Source = null;
                } 
            }
        }

        private void CmbMesCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TbError.Text = "";
            fillCategory = int.Parse(CmbMesCategory.SelectedValue.ToString());
            FillTable(fillDistrict, fillStatus, fillCategory, fillProcessed);
        }

        private void CmbFedDistrict_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TbError.Text = "";
            fillDistrict = int.Parse(CmbFedDistrict.SelectedValue.ToString());
            FillTable(fillDistrict, fillStatus, fillCategory, fillProcessed);
        }

        private void CmbLastMesStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TbError.Text = "";
            fillStatus = int.Parse(CmbLastMesStatus.SelectedValue.ToString());
            FillTable(fillDistrict, fillStatus, fillCategory, fillProcessed);
        }

        private void CmbProcessed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TbError.Text = "";
            fillProcessed = CmbProcessed.SelectedIndex;
            FillTable(fillDistrict, fillStatus, fillCategory, fillProcessed);
        }

        //Метод изменения статуса
        private void EditStatus()
        {
            if (DgMessage.SelectedItem != null && DgMessage.SelectedItem is MessageProcessing message)
            {
                message.MessageProcessingStatusMessage = status;
                message.Messages.Processed = true;
                Helper.GetContext().SaveChanges();
                FillTable(fillDistrict, fillStatus, fillCategory, fillProcessed);
            }
            else
            {
                TbError.Text = "Выберите обращение";
            }
        }
        private void BtnRejected_Click(object sender, RoutedEventArgs e)
        {
            status = 1;
            TbError.Text = "Обращение отклонено";
            EditStatus(); //Вызываем метод изменения статуса обращения
        }

        private void BtnProcessing_Click(object sender, RoutedEventArgs e)
        {
            status = 2;
            TbError.Text = "Обращение отправленно на дополнительное расмотрение";
            EditStatus(); //Вызываем метод изменения статуса обращения
        }

        private void BtnCompleted_Click(object sender, RoutedEventArgs e)
        {
            status=3;
            TbError.Text = "Обращение завершено";
            EditStatus(); //Вызываем метод изменения статуса обращения
        }

        private void BtnInEther_Click(object sender, RoutedEventArgs e)
        {
            status = 4;
            TbError.Text = "Обращение назначено в эфир";
            EditStatus(); //Вызываем метод изменения статуса обращения
        }

        private void BtnInGroupPopular_Click(object sender, RoutedEventArgs e)
        {
            TbError.Text = "";
            if (DgMessage.SelectedItem != null && DgMessage.SelectedItem is MessageProcessing message && CmbPopularGroup.SelectedItem != null)
            {
                DataContext = new PopularMessages();
                var newPopular = DataContext as PopularMessages;
                newPopular.PopularMessageMessageId = message.MessageProcessingMessage;
                newPopular.PopularMessagePopularGroup = int.Parse(CmbPopularGroup.SelectedValue.ToString());
                newPopular.vote = 0;
                message.Messages.Processed = true;
                Helper.GetContext().PopularMessages.Add(newPopular);
                Helper.GetContext().SaveChanges();
                TbError.Text = "Обращение добавлено в популярную группу";
                
            }
            else
            {
                TbError.Text = "Не выбрано обращение или популярная группа";
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            TbError.Text = "";
            string popularGroup = Interaction.InputBox("Введите название популярной группы! Добавить?", "Добавление новой популярной группы");
            if(popularGroup.Length >0)
            {
                DataContext = new PopularGroup();
                var newPopularGroup = DataContext as PopularGroup;
                newPopularGroup.Content = popularGroup;
                newPopularGroup.PopularGroupEvent = int.Parse(Container.idEvent.ToString());
                Helper.GetContext().PopularGroup.Add(newPopularGroup);
                Helper.GetContext().SaveChanges();
                TbError.Text = "Популярная группа добавлена";
                FillTable(fillDistrict, fillStatus, fillCategory, fillProcessed);
            }
            
        }
    }
}
