
/* ------------------------ ЧИТАТЬ ПОЛНОСТЬЮ ------------------------ 
ЧЕГО ИСПОЛЬЗУЕТСЯ?
    ТОЛЬКО РСА И ВНУТРЕННИЕ ФУНКЦИИ КЛАССА RSAParameters

ЧТО ДЕЛАЕТ КНОПОЧКА 1?
    1.КОГДА ВЫБИРАЕТ КНОПОЧКУ 1, ФОРМИРУЮТСЯ КЛЮЧИКИ
    2.САМ ВЫБИРАЕШЬ ПАРОЛЬ, НА ОСНОВЕ КОТОРОГО КЛЮЧИКИ ШИФРУЮТСЯ В АБРАКАДАБРУ
    3. АБРАКАДАБРА ЗАПИСЫВАЕТСЯ В ФАЙЛИК. ДАЖЕ ЕСЛИ У ПЛОХОГО МИСТЕРА БУДЕТ ЭТОТ ФАЙЛ, 
    ОН ВСЕ РАВНО НЕ ЗНАЕТ НИ ПАРОЛЯ, НИ КАКИМИ МЕТОДАМИ ХЕШИРОВАЛИСЬ КЛЮЧИ, А ТУТ АЕС НА ОСНОВЕ SHA256

ЧТО ДЕЛАЕТ КНОПОЧКА 2?
    КНОПОЧКА ДОСТАЁТ АБРАКАДАБРУ ИЗ ФАЙЛА
    ВВОДИШЬ ПАРОЛЬ,ЧТОБЫ КЛЮЧИКИ БЫЛИ ДОСТУПНЫ ДЛЯ ИСПОЛЬЗОВАНИЯ
    ОКЕЙ, В СИСТЕМА КЛЮЧИ РАСШИФРОВАЛИСЬ

ЧТО ДЕЛАЕТ КНОПОЧКА 3?
    ТУТ ВСЕ ПРОСТО - ПРОСТЕЙШИЙ ШИФРАТОР СТРОКИ
    ЗАПИСЫВАЕТ ЗАШИФРОВАННОЕ В ФАЙЛИК
    НЕ РАБОТАЕТ, ПОКА НЕ НАЖАТА КНОПОЧКА 2

ЧТО ДЕЛАЕТ КНОПОЧКА 4?
    ДЕШИФРАТОР ФАЙЛИКА
    НЕ РАБОТАЕТ, ПОКА НЕ НАЖАТА КНОПОЧКА 2
------------------------------------------------------------------------
*/
using System.Security.Cryptography;
using System.Text;

class Test
{
    // КЛАСС С ДАННЫМИ ОБО ВСЁМ, ЧТО ЕСТЬ В РСА
    static RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

    // ИНИЦИАЛИЗАЦИЯ ДВУХ КЛЮЧЕЙ
    static RSAParameters publicKey;
    static RSAParameters privateKey;

    public static string Encrypt(string plainText)
    {
        // ПЕРЕПРИСВОЕНИЕ КЛАССА ДЛЯ КОРРЕКТНОСТИ РАБОТЫ
        rsa = new RSACryptoServiceProvider();

        // УСТАНОВКА ОТКРЫТОГО КЛЮЧА НОВОМУ КЛАССА
        rsa.ImportParameters(publicKey);

        // ЭНКОД ТЕКСТА ДЛЯ КОДИРОВАНИЕ В BYTE
        var data = Encoding.Unicode.GetBytes(plainText);

        // ШИФРОВАНИЕ ТЕКСТА
        var cypher = rsa.Encrypt(data, false);

        // ВОЗВРАЩЕНИЕ ЗАШИФРОВАННОГО ТЕКСТА ТИПА ДАННЫЙ STRING
        return Convert.ToBase64String(cypher);
    }
    public static string Decrypt(string cypherText)
    {
        // КОНВЕРТАЦИЯ STRING В BYTE
        var dataBytes = Convert.FromBase64String(cypherText);

        // ПРИСВОЕНИЕ КЛАССУ РСА ЗАКРЫТОГО КЛЮЧА
        rsa.ImportParameters(privateKey);

        // ПОБИТОВОЕ ДЕШИФРОВАНИЕ ТЕКСТА (БИТОВ)
        var plainText = rsa.Decrypt(dataBytes, false);

        // ВОЗВРАЩЕНИЕ ДЕКОДИРОВАННОГО ТЕКСТА ТИПА ДАННЫЙ STRING
        return Encoding.Unicode.GetString(plainText);
    }
    static void ExportT(byte[] pass)
    {
        // ФОРМИРУЕМ ОТКРЫТЫЙ КЛЮЧ
        publicKey = rsa.ExportParameters(false);

        // ФОРМИРУЕМ ЗАКРЫТЫЙ КЛЮЧ
        privateKey = rsa.ExportParameters(true);

        // ПЕРЕПРИСВОЕНИЕ
        byte[] password = pass; 

        // ПАРАМЕТР, ОТНОСИТЕЛЬНО КОТОРОГО БУДУТ ШИФРОВАТЬСЯ КЛЮЧИ (здесь AES256 + SHA256)
        // ПАРОЛЬ ХЕШИРУЕТСЯ И ШИФРУЕТСЯ АЕСОМ
        PbeParameters k = new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, 1);

        // ЭКСПОРТИРУЮ КЛЮЧИ
        var prkeys = rsa.ExportEncryptedPkcs8PrivateKey(password, k);

        // ЗАПИСЫВАЕМ ЗАШИФРОВАННЫЕ КЛЮЧИ В ФАЙЛ
        File.WriteAllBytes("test.txt", prkeys);

    }
    static void ImportT(byte[] password)
    {
        // УСТАНАВЛИВАЕМ ПУТЬ К ФАЙЛУ
        byte[] sourse = File.ReadAllBytes("test.txt");

        // ДЛИНА ПАРОЛЯ (ОНА ОГРАНИЧЕНА ФУНКЦИЕЙ IMPORTA)
        Int32 s = password.Length; 

        // ПРИСВОЕНИЕ ПРОГРАММНОМУ КЛАССУ РСА ПАРАМЕТРЫ ИЗ ФАЙЛА.
        // ЕСЛИ ПАРОЛЬ НЕ СОВПАДАЕТ - ОШИБКА
        rsa.ImportEncryptedPkcs8PrivateKey(password, sourse, out s);

        // ПРИСВОЕНИЕ ОТКРЫТОГО КЛЮЧА
        publicKey = rsa.ExportParameters(false);
        
        // ПРИСВОЕНИЕ ЗАКРЫТОГО КЛЮЧА
        privateKey = rsa.ExportParameters(true);
        //

    }
    // ################################################################# НАЧАЛО ТУТ #################################################################
    static void Main(string[] args)
    {
        Console.WriteLine("Press 1, 2, 3 or 4\n1. Export\n2. Import\n3. Encode\n4. Decode"); // ------ ПЕРЕЧИСЛЕНИЕ ФУНКЦИЙ

    Here: // -------------------------------------------------------------------------- СЮДА НАС ОТПРАВЛЯЕТ ПРОГА ДЛЯ ПОВТОРНОГО ВЫБОРА ФУНКЦИЙ

        int c = Convert.ToInt32(Console.ReadLine()); // ------------------------------- ЧИТАЕМ НАЖАТЫЙ СИМВОЛ И КОНВЕРТИРУЕМ ЕГО В INT

        switch (c) // ----------------------------------------------------------------- СМОТРИМ НА ЦИФРУ, ПРОГРАММА ВЫБИРАЕТ, КАКАЯ НАЖАТА
        {
            // ЕСЛИ НАЖАТА 1
            case 1:
                
                Console.WriteLine("Choose password:");
                
                string passExp = Console.ReadLine(); // ------------------------------- ЧТЕНИЕ ПАРОЛЯ (НА ОСНОВЕ ЕГО ШИФРУЮТСЯ КЛЮЧИ)
                
                ExportT(Encoding.ASCII.GetBytes(passExp)); // ------------------------- ОБРАЩЕНИЕ К ФУНКЦИИ: ИНИЦИАЛИЗАЦИЯ КЛЮЧЕЙ
                                                           //                           И ИХ ЗАПИСЬ В ФАЙЛ В ЗАШИФРОВАННОМ ВИДЕ
                
                break;
            // ЕСЛИ НАЖАТА 2 - НЕ РАБОТАЕТ, ЕСЛИ ПРЕДВАРИТЕЛЬНО НЕ БЫЛА НАЖАТА 1
            case 2:

                Console.WriteLine("Write password:");

                string passImp = Console.ReadLine(); // ------------------------------- СНОВА ЧИТАЕМ ПАРОЛЬ

                ImportT(Encoding.ASCII.GetBytes(passImp)); // ------------------------- УСТАНАВЛИВАЕМ КЛЮЧИ В ПРОГРАММЕ, ЕСЛИ ПАРОЛЬ НЕ ВЕРНЫЙ, ТО ВЫЛЕТИТ ОШИБКА

                break;
            // ЕСЛИ НАЖАТА 3 - НЕ РАБОТАТЕ, ЕСЛИ НЕ БЫЛИ НАЖАТА 2, А 2 НЕ РАБОТАЕТ, ЕСЛИ НИ РАЗУ НЕ БЫЛА НАЖАТА 1
            case 3:

                Console.WriteLine("Data to encrypt:");

                string encStr = Console.ReadLine(); // -------------------------------- ТУТ МЫ БУДЕМ ШИФРОВАТЬ НЕБОЛЬШУЮ СТРОКУ, ПО СУТИ ЭТО НУЖНО ТОЛЬКО ДЛЯ ПРОВЕРКИ,
                                                    //                                  РАБОТАЕТ ЛИ ДЕШИФРОВАНИЕ КЛЮЧЕЙ

                File.WriteAllBytes("Enc.txt", Encoding.ASCII.GetBytes(Encrypt(encStr))); //  ЗАПИСЬ ЗАШИФРОВАННЫХ ДАННЫХ В ФАЙЛ

                //Console.WriteLine((Encrypt(encStr))); // ---------------------------- ПО ЖЕЛАНИЮ: ВЫВОДИТ ЗАШИФРОВАНЫЕ ДАННЫЕ
                break;
            // ЕСЛИ НАЖАТА 4 - НЕ РАБОТАЕТ, ЕСЛИ НЕ БЫЛО НАЖАТО НИ 1, НИ 2. ЕСЛИ НЕ БЫЛА НИ РАЗУ НАЖАТА 3, ВЫДАЕТ ОШИБКУ, ПОСКОЛЬКУ ЗАШИФРОВАННЫХ ДАННЫХ НЕ БЫЛО
            case 4:

                byte[] Enc = File.ReadAllBytes("Enc.txt"); // ------------------------- ЧИТАЕМ ФАЙЛ С ЗАШИФРОВАННЫМИ ДАННЫМИ

                Console.WriteLine(Decrypt(Encoding.ASCII.GetString(Enc))); // --------- ВЫВОД РАСШИФРОВАННЫХ ДАННЫЫХ
                
                break;
            // ЕСЛИ БЫЛО НАЖАТО ЧТО УГОДНО, КРОМЕ 1,2,3 и 4
            default:
                Console.WriteLine("Error");
                break;
        }
        goto Here; // ----------------------------------------------------------------- ОТПРАВЛЯЕТ ДЛЯ ПОВТОРНОГО ВЫБОРА ФУНКЦИЙ
    }
}