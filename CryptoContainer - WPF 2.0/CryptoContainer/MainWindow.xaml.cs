using System;
using System.IO;
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
using System.Security.Cryptography;

namespace CryptoContainer
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



        // ЗАПИСЬ В ФАЙЛ ХЕША, КОТОРЫЙ ПОТРЕБУЕТСЯ ДЛЯ АУТЕНТИФИКАЦИИ
        private async void FileInput( string txt,string str)
        {
            await File.AppendAllTextAsync(txt, str + "\n");
        }



        // ХЕШИРОВАНИЕ ПРИНИМАЕМЫХ ДАННЫХ
        private string SHA256_encoder(string inwordstr)
        {
            var msgBytes = Encoding.ASCII.GetBytes(inwordstr);
            var sha = new HMACSHA256(msgBytes);
            var hash = sha.ComputeHash(msgBytes);
            string str = null;
            foreach (byte b in hash)
            {
                str += b.ToString("x2");
            }
            return str;
        }



        // КЛИК КНОПКИ "СОЗДАТЬ" - ОБРАБОТКА ИСКЛЮЧЕНИЯ ПУСТОЙ СТРОКИ И ОБРАЩЕНИЕ К ХЕШ-ФУНКЦИИ
        private void button_create_Click(object sender, RoutedEventArgs e)
        {
            if (text_box_login.Text == "" || text_box_pass.Text == "")
            {
                MessageBox.Show("Input login and password", "Error");
            }
            else
            {
                string str = SHA256_encoder(text_box_login.Text); // - ХЕШИРУЕМ ЛОГИН
                FileInput("hashLoginAndPass.txt", str);
                str = SHA256_encoder(text_box_pass.Text); //         - ХЕШИРУЕМ ПАРОЛЬ
                FileInput("hashLoginAndPass.txt", str);


                // УСТАНОВКА PUBLIC И PRIVATE КЛЮЧЕЙ ДЛЯ ПОЛЬЗОВАТЕЛЯ
                UserData.RSAEncryption();
                

                // ОТКРЫТЫЙ КЛЮЧ БЕЗ ИНФОРМАЦИИ ОБ РСА
                str = UserData.GetPublicKey();
                FileInput("publicKey.txt",str);


                // ЗАКРЫТЫЙ КЛЮЧ БЕЗ ИНФОРМАЦИИ ОБ РСА
                str = UserData.GetPrivateKey();              
                FileInput("privateKey.txt",str);
                
            }
        }



        // ПРОВЕРКА НА СУЩЕСТВОВАНИЕ ЛОГИНА И ПАРОЛЯ
        private bool ChekingExistHash()
        {
            bool exist = false;
            try
            {
                // ЧТЕНИЕ ФАЙЛА С ЛОГИНАМИ И ПАРОЛЯМИ
                StreamReader sr = new StreamReader("hashLoginAndPass.txt");
                string line = sr.ReadLine();

                // УКАЗЫВАЕМ ЛОГИН, ПО ЛОГИКЕ ПРОГРАММЫ ЗА НИМ СРАЗУ ИДЕТ ПАРОЛЬ
                // ДА, КОСТЫЛЬ, НО ВРЕМЕНИ НА НАПИСАНИЕ БЫЛО МАЛО
                string loginpass = SHA256_encoder(text_box_login.Text);
                while (line != null)
                {
                    if (loginpass == line || loginpass + "\n" == line)
                    {
                        loginpass = SHA256_encoder(text_box_pass.Text);
                        line = sr.ReadLine();
                    }
                    // ЕСЛИ ЛОГИН СОВПАЛ, А СЛЕДУЮЩАЯ СТРОКА - ПАРОЛЬ, НЕ СОВПАЛ, ТО ИДЕМ ДАЛЬШЕ
                    // МОЖЕТ БЫТЬ ТАКАЯ СИТУАЦИЯ, ЧТО В СИСТЕМЕ НЕСКОЛЬКО ОДИНАКОВЫХ ЛОГИНОВ
                    if (loginpass == line || loginpass + "\n" == line)
                    {
                        exist = true;
                    }
                    line = sr.ReadLine();
                }
                sr.Close();
                Console.ReadLine();
            }
            catch (Exception) { }
            if (exist)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        // КЛИК КНОПКИ "ПРОДОЛЖИТЬ", Т.Е. АВТОРИЗАЦИЯ - ПРОВЕРКА НА ПУСТУЮ СТРОКУ
        private int mistakes_counter = 0; // ПРОСТЕЙШАЯ ЗАЩИТА ОТ БРУТФОРСА
        private void button_continue_Click(object sender, RoutedEventArgs e)
        {
            if (text_box_login.Text == "" || text_box_pass.Text == "") MessageBox.Show("Input login and password", "Error");
            else
            {
                if (File.Exists("hashLoginAndPass.txt"))
                {
                    if (ChekingExistHash())
                    {
                        MessageBox.Show("GREAT!", "Message");
                        // УСТАНОВКА СТАТИЧЕСКИХ ПОЛЕЙ ЛОГИНА, ПАРОЛЯ И ИХ ХЕШИРОВАННЫХ ВАРИАНТОВ
                        UserData.DataSetter(
                            text_box_login.Text,
                            text_box_pass.Text,
                            SHA256_encoder(text_box_login.Text),
                            SHA256_encoder(text_box_pass.Text));

                        UserData.SetUserKeys();
                        // ЗАПУСК ВТОРОГО ФРЕЙМА ДЛЯ ОТОБРАЖЕНИЯ ФУНКЦИОНАЛЬНОГО ОКНА
                        Frame.Content = new Actions();
                    }
                    else
                    {
                        mistakes_counter++;
                        MessageBox.Show("Something goes wrong", "Message");
                        if (mistakes_counter == 5) Close();
                    }
                }
                else MessageBox.Show("Create a profile at first", "Message");
            }
        }
    }
}
