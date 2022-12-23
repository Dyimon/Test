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

namespace EvantApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CmbEvent.ItemsSource = Helper.GetContext().Events.ToList();
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            if(CmbEvent.SelectedItem == null)
            {
                TbErrorLog.Text = "Выберите мероприятие!";
            }
            if(TbLogin.Text.Length >0 && TbPassword.Password.Length > 0)
            {
                var user = Helper.GetContext().Staff.FirstOrDefault(p => p.Login == TbLogin.Text && p.Password == TbPassword.Password);
                if(user != null)
                {
                    var stafEvent = Helper.GetContext().StaffEvent.Where(p => p.EventId == CmbEvent.SelectedIndex + 1 && p.StaffId == user.IdStaff).ToList();
                    if (stafEvent.Count == 0)
                    {
                        if (user.TypeStaff == 1)
                        {
                            if (TbEventStatus.Text == "Текущее" || TbEventStatus.Text == "Предстоящее")
                            {
                                MessageBox.Show("Вы вошли как администратор");
                                TbErrorLog.Text = "";
                            }
                            else
                            {
                                TbErrorLog.Text = "Выберите текущее или предстоящее мероприятие!";
                            }
                        }
                        else
                        {
                            TbErrorLog.Text = "Вас не выбрали на мероприятие!";
                        }
                    }
                    else
                    {
                        switch (user.TypeStaff)
                        {
                            case 2:
                                if (TbEventStatus.Text == "Текущее")
                                {
                                    Container.idStaff = user.IdStaff;
                                    user.Online = true;
                                    Helper.GetContext().SaveChanges();
                                    Windows.WinEtherealMessage winEtherealMessage = new Windows.WinEtherealMessage();
                                    winEtherealMessage.TbEventTitle.Text = TbEventName.Text;
                                    winEtherealMessage.TbNameStaff.Text = user.SecondName + " " + user.FirstName + " " + user.Patronumic;
                                    winEtherealMessage.TbTypeStaff.Text = user.TypeStaff1.TypeStaff1;
                                    winEtherealMessage.Show();
                                    this.Hide();
                                    TbErrorLog.Text = "";
                                }
                                else
                                {
                                    TbErrorLog.Text = "Выберите текущее мероприятие";
                                }
                                break;
                            case 3:
                                if (TbEventStatus.Text == "Текущее" || TbEventStatus.Text == "Прошедшее")
                                {
                                    
                                    Container.idStaff = user.IdStaff;
                                    user.Online = true;
                                    Helper.GetContext().SaveChanges();
                                    Windows.WinModerator winModerator = new Windows.WinModerator();
                                    winModerator.TbEventTitle.Text = TbEventName.Text;
                                    winModerator.TbNameStaff.Text = user.SecondName + " " + user.FirstName + " " + user.Patronumic;
                                    winModerator.TbTypeStaff.Text = user.TypeStaff1.TypeStaff1;
                                    winModerator.Show();
                                    this.Hide();
                                    TbErrorLog.Text = "";
                                }
                                else
                                {
                                    TbErrorLog.Text = "Выберите текущее или прошедшее мероприятие!";
                                }
                                break;
                            case 4:
                                if (TbEventStatus.Text == "Текущее")
                                {
                                    MessageBox.Show("Вы вошли как оператор");
                                    TbErrorLog.Text = "";
                                }
                                else
                                {
                                    TbErrorLog.Text = "Выберите текущее мероприятие!";
                                }
                                break;
                        }    
                    }
                }
                else
                {
                    TbErrorLog.Text = "Не верный логин или пароль!";
                }
            }
            else
            {
                TbErrorLog.Text = "Не все поля заполнены!";
            }
        }

        private void CmbEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string status = "";
            var events = Helper.GetContext().Events.First(p => p.IdEvent == CmbEvent.SelectedIndex + 1);
            TbEventName.Text = events.Name.ToString();
            Container.idEvent =events.IdEvent;
            TbDescription.Text = events.Description.ToString();
            
            if(events.DateTimeBegin <= DateTime.Now && events.DateTimeEnd >= DateTime.Now)
            {
                status = "Текущее";
            }
            if(events.DateTimeBegin < DateTime.Now && events.DateTimeEnd < DateTime.Now)
            {
                status = "Прошедшее";
            }
            if(events.DateTimeBegin > DateTime.Now && events.DateTimeEnd > DateTime.Now)
            {
                status = "Предстоящее";
            }
            TbEventStatus.Text = status;
        }

        private void BtnNewComment_Click(object sender, RoutedEventArgs e)
        {
            if(TbEventStatus.Text == "Текущее")
            {
                Windows.WinSupplyMessage winSupplyMessage = new Windows.WinSupplyMessage();
                winSupplyMessage.TbEventTitle.Text = TbEventName.Text;
                winSupplyMessage.Show();
                Hide();

            }
            else
            {
                TbErrorLog.Text = "Выберите текущее мероприятие";
                
            }
        }

        private void BtnPopularComment_Click(object sender, RoutedEventArgs e)
        {
            if (TbEventStatus.Text == "Текущее" || TbEventStatus.Text == "Прошедшее")
            {
                Windows.WinPopularMessage winPopularMessage = new Windows.WinPopularMessage();
                winPopularMessage.TbEventTitle.Text = TbEventName.Text;
                winPopularMessage.Show();
                this.Hide();
            }
            else
            {
                TbErrorLog.Text = "Выберите текущее или прошедшее мероприятие!";
                
            }
        }
    }
}
