using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;

namespace CryptoContainer
{
    /// <summary>
    /// Interaction logic for Actions.xaml
    /// </summary>
    public partial class Actions : Page
    {
        public Actions()
        {
            InitializeComponent();
        }
        private void button_choose_file_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); // ------------- ЧТЕНИЕ ФАЙЛА

            openFileDialog.Filter = "Textfiles|*.txt"; // ------------------------ ФИЛЬТР ФОРМАТА ФАЙЛА

            openFileDialog.Multiselect = false; // ------------------------------- ЗАПРЕТ НА ВЫБОР НЕСКОЛЬКИХ ФАЙЛОВ

            if (openFileDialog.ShowDialog() == true)            
                textbox_opened_file.Text = openFileDialog.FileName; // ----------- УСТАНОВКА В ПОЛЕ ТЕКСТБОКЛА НАЗВАНИЯ ФАЙЛА
            
        }
        private string cyph = string.Empty; // ----------------------------------- DATA VARIABLE 
        private void button_encrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (textbox_opened_file.Text != "")
                {
                    StreamReader sr = new StreamReader(textbox_opened_file.Text); 

                    cyph = sr.ReadToEnd(); // ----------------------------------- ЧИТАЕМ ВСЕ СОДЕРЖИМОЕ

                    cyph = UserData.Encrypt(cyph); // --------------------------- ОТПРАВЛЯЕМ ДАННЫЕ ИЗ ФАЙЛА НА ШИФРОВАНИЕ

                    string fileExt = System.IO.Path.GetExtension(textbox_opened_file.Text);

                    SaveFileDialog saveFileDialog = new SaveFileDialog();

                    saveFileDialog.Filter = "Files (*" + fileExt + ") |*" + fileExt;

                    if (saveFileDialog.ShowDialog() == true) // ------------------ ЗАПИСЫВАЕМ ЗАКРИПТОВАННОЕ В НОВЫЙ ФАЙЛ
                        File.WriteAllBytes(saveFileDialog.FileName, Encoding.UTF8.GetBytes(cyph));
                    
                }
            }
            catch (Exception)
            {
                MessageBox.Show("File is in use or too big.", "Error");
            }
        }

        private void button_decrypt_Click_1(object sender, RoutedEventArgs e)
        {
            // ЕСИЛ ВЫБРАН ЧУЖОЙ КЛЮЧ, ТО МЫ НЕ МОЖЕМ РАСШИФРОВЫВАТЬ
            if (KeyOwnerBool)
                MessageBox.Show("Use your public key", "ERROR!");
            else
            {
                if (textbox_opened_file.Text != "")
                {
                    StreamReader sr = new StreamReader(textbox_opened_file.Text);
                    cyph = sr.ReadToEnd();
                    cyph = UserData.Decrypt(cyph); // ---------------------------- РАСШИФРОВЫВАЕМ

                    string fileExt = System.IO.Path.GetExtension(textbox_opened_file.Text);
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Files (*" + fileExt + ") |*" + fileExt;

                    if (saveFileDialog.ShowDialog() == true)
                        File.WriteAllBytes(saveFileDialog.FileName, System.Text.Encoding.ASCII.GetBytes(cyph));

                }
            }
        }
        // КЛЮЧИ: ВОЗВРАТ К СВОЕМУ И ИЗМЕНЕНИЕ НА ЧУЖОЙ
        private bool KeyOwnerBool = false;
        private void button_Another_Public_Key_Click(object sender, RoutedEventArgs e)
        {
            KeyOwnerBool = true;
            UserData.SetPublicKey();
        }
        private void button_return_your_key_Click(object sender, RoutedEventArgs e)
        {
            UserData.SetPublicKey();
            KeyOwnerBool = false;
            MessageBox.Show("Your public key is returned", "Message");
        }
    }
}

