using Microsoft.Win32;
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
    /// Логика взаимодействия для WinSupplyMessage.xaml
    /// </summary>
    public partial class WinSupplyMessage : Window
    {
        public WinSupplyMessage()
        {
            InitializeComponent();
            CmbFederalDistrict.ItemsSource = Helper.GetContext().FederalDistricts.ToList();
            CmbCategoryMessage.ItemsSource = Helper.GetContext().MessageCategories.ToList();
        }
        string videoName;
        public void BtnEnterVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Все медиа фыйлы|*.wav;*.aac;*.wma;*.wmv;*.avi;*.mpg;*.mpeg;*.m1v;*.mp2;*.mp3;*.mpa;*.mpe;*.m3u;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.cda;*.aif;*.aifc;*.aiff;*.mid;*.midi;*.rmi;*.mkv;*.WAV;*.AAC;*.WMA;*.WMV;*.AVI;*.MPG;*.MPEG;*.M1V;*.MP2;*.MP3;*.MPA;*.MPE;*.M3U;*.MP4;*.MOV;*.3G2;*.3GP2;*.3GP;*.3GPP;*.M4A;*.CDA;*.AIF;*.AIFC;*.AIFF;*.MID;*.MIDI;*.RMI;*.MKV|Все файлы (*.*)|*.*";
            openFileDialog.Filter = "Видео-файл (*.Mpg,*.Mov,*.Wmv,*.Avi)|*.mpg;*.mov;*.wmv;*.avi|Видео файл в формате (*.mp4)|*.mp4|Файл (*.Rm)|*.rm|Все файлы (*.*)|*.*";// +
            if (openFileDialog.ShowDialog() == true)
            {
                
                System.Uri uri = new System.Uri(openFileDialog.FileName);
                MeVideo.Source = uri;
                videoName = openFileDialog.SafeFileName;
            }
        }
        
        
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (TbAge.Text.Length>0 && CmbCategoryMessage.Text.Length>0 && CmbFederalDistrict.Text.Length>0 && (TbMessage.Text.Length>10 || MeVideo.HasVideo == true))
            {
                int type = 1;
                if(MeVideo.HasVideo == true)
                {
                    type = 2;
                }
                DataContext = new User();
                var newUser = DataContext as User;
                newUser.FirstName = TbFirstName.Text;
                newUser.SecondName = TbSecondName.Text;
                newUser.Patronymic = TbPatronumic.Text;
                newUser.Phone = TbTelephon.Text;
                newUser.Email = TbEmail.Text;
                newUser.Age = int.Parse(TbAge.Text);
                newUser.FederalDistrictId = int.Parse(CmbFederalDistrict.SelectedValue.ToString());
                Helper.GetContext().User.Add(newUser);
                Helper.GetContext().SaveChanges();
                DataContext = new Messages();
                var newMessages = DataContext as Messages;
                newMessages.DateTime = DateTime.Now;
                newMessages.MessageUser = newUser.IdUser;
                newMessages.TypeMessage = type;
                newMessages.MessageText = TbMessage.Text;
                newMessages.MessegeEvent = Helper.GetContext().Events.First(p => p.Name == TbEventTitle.Text).IdEvent;
                newMessages.MediaContent = videoName;
                newMessages.Processed = false;
                newMessages.MessageCategories = int.Parse(CmbCategoryMessage.SelectedValue.ToString());
                Helper.GetContext().Messages.Add(newMessages);
                Helper.GetContext().SaveChanges();
                BlockBlocked();
                TbError.Text = "Обращение подано на обработку!";
            }
            else
            {
                TbError.Text = "Не все поля заполнены!";
            }
        }

        private void BlockBlocked()
        {
            TbFirstName.IsEnabled = false;
            TbSecondName.IsEnabled = false;
            TbPatronumic.IsEnabled = false;
            TbTelephon.IsEnabled = false;
            TbEmail.IsEnabled = false;
            TbAge.IsEnabled = false;
            TbMessage.IsEnabled = false;
            CmbFederalDistrict.IsEnabled = false;
            CmbCategoryMessage.IsEnabled = false;
            BtnEnterVideo.IsEnabled = false;
            BtnDelVideo.IsEnabled = false;
            BtnOk.IsEnabled = false;
        }
        private void selectedModer()
        {
            var moder = Helper.GetContext().Staff.Where(p => p.TypeStaff1.TypeStaff1 == "Модератор").ToList();
            foreach (var onlyModer in  moder)
            {
               
                
            }
        }
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void TbAge_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Char.IsDigit(e.Text, 0) && TbAge.Text.Length<2)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void TbAge_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }

        }

        private void BtnDelVideo_Click(object sender, RoutedEventArgs e)
        {
            MeVideo.Source = null;
        }
    }
}
