/*
ЯЗЫК ПРОГРАММИРОВАНИЯ C#, КОНСТРУКТОР ГРАФИЧЕСКОГО ИНТЕРФЕЙСА WPF, .NET 6.0 Framework
ОПИСАНИЕ:
    Программа с самописным алгоритмом шифрования и дешифрования. 
    Имеет функции выбора файла любого формата, 
    функцию выбора применяемого метода(шифрование и дешифрование), 
    функции шифрования  и дешифрования базируются на введенном пароле
    примитивный понятный интерфейс.

ВЫБОР ФАЙЛА:
    Используется встроенная библиотека Microsoft.Win32, 
    используемые классы: OpenFileDialog, File 
    используемые методы: ShowDialog, ReadAllBytes
    Позволяет выбрать файл из доступной для пользователя директории

ВЫВОД ФАЙЛА:
    Используется встроенная библиотека IO, 
    классы: SaveFileDialog, File
    методы: ShowFialog, WriteAllBytes

ШИФРОВАНИЕ: 
    Представляет из себя компиляцию с заменой и смешением компонентов файла 
    Логика обработки файла нарушается, что ведет к невозможности чтения файла

*/
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

namespace ILyaContainer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private byte[] abc; //  Формирует результат при шифровании

        private byte[,] table; // Формирует результат при дешифровании

        private void button_browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); // Открывает указанный пользователем файл

            openFileDialog.Multiselect = false; // Запрет на выбор нескольких файлов

            if (openFileDialog.ShowDialog() == true)
            {
                textbox_path.Text = openFileDialog.FileName; // Графический вывод выбранной директории в поле textbox_path
            }
        }

        private void OutputFile(byte[] result)
        {
            string extention = System.IO.Path.GetExtension(textbox_path.Text); // Сохранение расширения исходного файла
            File.WriteAllBytes(textbox_path.Text, result);

            //SaveFileDialog saveFileDialog = new SaveFileDialog(); 

            //saveFileDialog.Filter = "Files (*" + extention + ")|*" + extention; // Указатель на расширение

            //if (saveFileDialog.ShowDialog() == true)
            //{
            //    File.WriteAllBytes(saveFileDialog.FileName, result); // Запись результата в файл с указанием имени (расширение добавляется автоматически)
            //}
        }

        private void Encrypt(byte[] ContentFile, byte[] keys)
        {
            textbox_password.Clear();
            byte[] result = new byte[ContentFile.Length];

            for (int i = 0; i < ContentFile.Length; i++)
            {
                byte value = ContentFile[i];

                byte key = keys[i];

                int valueIndex = -1, keyIndex = -1;

                for (int j = 0; j < 256; j++)
                {
                    if (abc[j] == value)
                    {
                        valueIndex = j;
                        break;
                    }
                }
                for (int j = 0; j < 256; j++)
                {
                    if (abc[j] == key)
                    {
                        keyIndex = j;
                        break;
                    }
                }
                result[i] = table[keyIndex, valueIndex];
            }
            // OutputFile(result);
            File.WriteAllBytes(textbox_path.Text, result);
        }
        private string Decrypt(byte[] ContentFile, byte[] keys)
        {
            //textbox_password.Clear();
            byte[] result = new byte[ContentFile.Length];

            for (int i = 0; i < ContentFile.Length; i++)
            {
                byte value = ContentFile[i];

                byte key = keys[i];

                int valueIndex = -1, keyIndex = -1;

                for (int j = 0; j < 256; j++)
                {
                    if (abc[j] == key)
                    {
                        keyIndex = j;
                        break;
                    }
                }
                for (int j = 0; j < 256; j++)
                {
                    if (table[keyIndex, j] == value)
                    {
                        valueIndex = j;
                        break;
                    }
                }
                result[i] = abc[valueIndex];
            }
            return System.Text.Encoding.UTF8.GetString(result);
            
        }
        private string SHA_Enc(string inwordstr)
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
        private void button_start_Click(object sender, RoutedEventArgs e)
        {
            // ИСКЛЮЧЕНИЯ: Заполнение поля "директория", заполнение поля "пароль", указание метода 
            if (!String.IsNullOrEmpty(textbox_path.Text) &&
                !String.IsNullOrEmpty(textbox_password.Text) &&
                (ratio_button_Encrypt.IsChecked == false ^ ratio_button_decrypt.IsChecked == false))
            {
                // Проверка файла на существование 
                if (!File.Exists(textbox_path.Text))
                {
                    MessageBox.Show("File doesn't exist", "Error");
                    return;

                }

                abc = new byte[256];

                for (int i = 0; i < 256; i++) 
                    abc[i] = Convert.ToByte(i);

                
                table = new byte[256, 256];

                for (int i = 0; i < 256; i++)
                {
                    for (int j = 0; j < 256; j++)
                    {
                        table[i, j] = abc[(i + j) % 256];
                    }
                }
                // Блок try-catch для избежания ошибок памяти с графическим выводом информации пользователю
                try
                {
                    // Читение всех байт исходного файла, запись в массив байт
                    byte[] ContentFile = File.ReadAllBytes(textbox_path.Text);

                    // Конвертация пароля
                    byte[] passwordTmp = Encoding.ASCII.GetBytes(SHA_Enc(textbox_password.Text)); // Формирует ключи

                    byte[] keys = new byte[ContentFile.Length];

                    // Заполнение массива ключей
                    for (int i = 0; i < ContentFile.Length; i++)
                        keys[i] = passwordTmp[i % passwordTmp.Length];

                    // Обработка выбора клиента: шифрование/дешифрование
                    if (ratio_button_Encrypt.IsChecked == true)
                    {
                        Encrypt(ContentFile, keys);
                    }
                    else if (ratio_button_decrypt.IsChecked == true)
                    {
                        string str = Decrypt(ContentFile, keys);
                        Frame.Content = new Actions(str);
                        
                    }
                    else
                        MessageBox.Show("Please, select a function", "Error");
                }
                catch (Exception)
                {

                    MessageBox.Show("File is somewhere opened", "Error");
                }
            }
            else
                MessageBox.Show("Please, enter the password, choose the function and file", "Error");
        }
    }
}
