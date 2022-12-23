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
    /// Логика взаимодействия для WinPopularMessage.xaml
    /// </summary>
    public partial class WinPopularMessage : Window
    {
        int filt;
        int mes=0;
        public WinPopularMessage()
        {
            InitializeComponent();
            var group = Helper.GetContext().PopularGroup.ToList();
            group.Insert(0, new PopularGroup() { Content = "Все" });
            CmbFilt.ItemsSource = group;
            FillTable(filt);
        }
        private void FillTable(int filt)
        {
            var popularMessage = Helper.GetContext().PopularMessages.ToList();
            if (filt > 0)
            {
                popularMessage = popularMessage.Where(p => p.IdPopularMessage == filt).ToList();
            }
            popularMessage = popularMessage.Where(p => p.IdPopularMessage != mes).ToList();
            DgMessage.ItemsSource = popularMessage;
        }

        private void CmbFilt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filt = CmbFilt.SelectedIndex;
            FillTable(filt);
        }

        private void BtnVoice_Click(object sender, RoutedEventArgs e)
        {
            if(DgMessage.SelectedItem != null && DgMessage.SelectedItem is PopularMessages popularMessage)
            {
                popularMessage.vote += 1;
                Helper.GetContext().SaveChanges();
                TbError.Text = "Вы проголосовали";
                mes = popularMessage.IdPopularMessage;
                FillTable(filt);
                BtnVoice.IsEnabled = false;  
            }
            else
            {
                TbError.Text = "Выберите обращение";
            }
            

        }

        private void DgMessage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgMessage.SelectedItem != null && DgMessage.SelectedItem is PopularMessages popularMessage)
            {
                if(popularMessage.Messages.MediaContent != null)
                {

                }
                else
                {
                    MeVideo.Source = null;
                }
                var events = popularMessage.Messages.Events;
                if (events.DateTimeBegin <= DateTime.Now && events.DateTimeEnd >= DateTime.Now)
                {

                    BtnVoice.IsEnabled = true;
                    TbError.Text = "";
                }
                else
                {
                    BtnVoice.IsEnabled = false;
                    TbError.Text = "Голосовать можно только за обращения текущего мероприятия!";
                }
            }
        }
    }
}
