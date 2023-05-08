using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace CryptoContainer
{
    // ЗДЕСЬ ВСЁ СТАТИК, ТО ЕСТЬ ИЗ ЛЮБОЙ ЧАСТИ ПРОГРАММЫ МОЖНО ОБРАТИТЬСЯ К ЭТИМ ДАННЫМ
    // ПОСКОЛЬКУ ПРИЛОЖЕНИЕ НА C#, ТО ДОПОЛНИТЕЛЬНОЙ ЗАЩИТЫ НЕ ТРЕБУЕТСЯ
    // ИЗ-ЗА ЧРЕЗМЕРНОЙ СЛОЖНОСТИ ДЕОБФУСКАЦИИ С АССЕМБЛЕРА
    internal class UserData
    {
        // ДАЛЕЕ НЕ ПРИГОДЯТСЯ, НО ОЧЕНЬ ХОТЕЛОСЬ ИНТЕГРИРОВАТЬ ЭТИ ДАННЫЕ С КЛЮЧАМИ
        // ЛИШНИЙ ФУНКЦИОНАЛ РЕШИЛА НЕ УДАЛЯТЬ. ВДРУГ БУДУ ДОВОДИТЬ ДО УМА ПРОЕКТ
        private static string login = "";
        public static string pass = "";
        public static string login_hash = "";
        public static string pass_hash = "";
        public static void DataSetter(string l, string p, string hl, string hp)
        {
            login = l;
            pass = p;
            login_hash = hl;
            pass_hash = hp;
        }
        public static string DataGetter()
        {
            return login + pass + login_hash + pass_hash;
        }

        // БИБЛИОТЕКF CRYPTOGRAPHY ДЛЯ ИСПОЛЬЗОВАНИЯ РСА КЛЮЧЕЙ
        // Создает ключ размером 2048 бита. Это значит, что ним можно зашифровать максимум 2048 бит
        // (на самом деле еще меньше, потому что используется криптографическая схема PKCS1, а не прямое шифрование).
        // Программа тестовая, учебная, на этапе выбора криптографической схемы этот нюанс не учитывался.

        private static RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);
        private static RSAParameters _privateKey;
        private static RSAParameters _publicKey;
        public static void RSAEncryption()
        {
            _privateKey = csp.ExportParameters(true);
            _publicKey = csp.ExportParameters(false);

        }
        // ПРЕДВАРИТЕЛЬНАЯ УСТАНОВКА КЛЮЧЕЙ ( МОЖНО БЫЛО СДЕЛАТЬ SAFE FILE DIALOG, ЗНАЮ)
        public static void SetUserKeys()
        {
            StreamReader srv = new StreamReader("publicKey.txt"); // ---ШАБЛОН: ЕСЛИ БЫ КЛЮЧИ БЫЛИ В БД ПРИВЯЗАНЫ К ЮЗЕРУ

            string line = srv.ReadLine();

            var sr = new StringReader(line); // ------------------------КЛЮЧИ ЗАПИСАНЫ В ЛИНИЮ БЕЗ ТЕРМИНАЛЬНОГО НУЛЯ И \n

            var xs = new XmlSerializer(typeof(RSAParameters)); // ------ТЕХНИЧЕКИ - КОНВЕРТАЦИЯ КЛЮЧЕЙ В RSAParameters

            _publicKey = (RSAParameters)xs.Deserialize(sr);



            srv = new StreamReader("privateKey.txt");

            line = srv.ReadLine();

            sr = new StringReader(line);

            xs = new XmlSerializer(typeof(RSAParameters));

            _privateKey = (RSAParameters)xs.Deserialize(sr);

            sr.Close();

        }
        // ФУНКЦИОНАЛ КНОПОКИ SET ANOTHER PUBLIC KEY И RETURN TO OWN KEY
        // ВЫНЕСЕН СЮДА ДЛЯ УПРОЩЕНИЯ ОБРАЩЕНИЯ К ПРИВАТНЫМ ПЕРЕМЕННЫМ
        public static void SetPublicKey()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); // -- ВЫБОР ФАЙЛА

            openFileDialog.Filter = "Textfiles|*.txt"; // ------------- ФИЛЬТР: ТОЛЬКО ТХТ-ФАЙЛЫ

            openFileDialog.Multiselect = false; // -------------------- ОТКЛЮЧЕНИЕ ВОЗМОЖНОСТИ СЕЛЕКТА НЕСКОЛЬКИХ ФАЙЛОВ

            if (openFileDialog.ShowDialog() == true)
            {
                StreamReader sr = new StreamReader(openFileDialog.FileName);

                string line = sr.ReadToEnd();

                var srv = new StringReader(line);

                var xs = new XmlSerializer(typeof(RSAParameters));

                _publicKey = (RSAParameters)xs.Deserialize(srv);
            }
        }

        // ОСНОВА КРИПТОГРАФИИИ - ПОЛУЧЕНИЕ КЛЮЧЕЙ, ЭНКРИПТОР, ДЕКРИПТОР

        // МЕТОДЫ КЛАССА ДЛЯ ОТЛАДКИ, НА ЭТАПЕ ПОДГОТОВКИ К ВЫПУСКУ В ПРОДАКШЕН УДАЛЯЮТСЯ
        public static string GetPublicKey()
        {
            var sw = new StringWriter();

            var xs = new XmlSerializer(typeof(RSAParameters));

            xs.Serialize(sw, _publicKey);

            return sw.ToString();
        }
        public static string GetPrivateKey()
        {
            var sw = new StringWriter();

            var xs = new XmlSerializer(typeof(RSAParameters));

            xs.Serialize(sw, _privateKey);

            return sw.ToString();
        }
        // ПОСТРОКОВОЕ ОПИСАНИЕ:
        // ИНИЦИАЛИЗАЦИЯ НОВОГО КЛАССА, УСТАНОВКА ПАРАМЕТРОВ, ЭНКОДИНГ В byte[] СТРОКИ, ШИФРОВАНИЕ, ВОЗВРАТ РЕЗУЛЬТАТА
        public static string Encrypt(string plainText)
        {
            csp = new RSACryptoServiceProvider();

            csp.ImportParameters(_publicKey);

            var data = Encoding.Unicode.GetBytes(plainText);

            var cypher = csp.Encrypt(data, false);

            return Convert.ToBase64String(cypher);
        }
        // ПОСТРОКОВОЕ ОПИСАНИЕ: 
        // ЭНКОДИНГ В byte[] СТРОКИ, УСТАНОВКА ПАРАМЕТРОВ, ДЕШИФРОВАНИЕ, ВОЗВРАТ РЕЗУЛЬТАТА
        public static string Decrypt(string cypherText)
        {
            var dataBytes = Convert.FromBase64String(cypherText);

            csp.ImportParameters(_privateKey);

            var plainText = csp.Decrypt(dataBytes, false);

            return Encoding.Unicode.GetString(plainText);
        }
    }
}
