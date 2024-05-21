using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

public class Client
{
    // Адрес сервера
    private const string serverAddress = "192.168.250.87"; // Замени на IP адрес или доменное имя сервера

    // Порт для соединения с сервером
    private const int Port = 8086;

    // Метод для запуска клиента
    public void Start()
    {
        while (true)
        {
            Console.Write("Введите название файла: ");
            string fileName = Console.ReadLine();
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Имя файла не может быть пустым.");
                continue;
            }

            GetFileByName(fileName);
        }
    }

    // Метод для запроса файла у сервера
    public void GetFileByName(string fileName)
    {
        try
        {
            using (TcpClient client = new TcpClient(serverAddress, Port)) // Подключаемся к серверу
            using (NetworkStream stream = client.GetStream())
            {
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
                stream.Write(fileNameBytes, 0, fileNameBytes.Length); // Отправляем имя файла на сервер

                string filesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files");
                if (!Directory.Exists(filesDirectory))
                {
                    Directory.CreateDirectory(filesDirectory); // Создаем директорию, если она не существует
                }

                string filePath = Path.Combine(filesDirectory, fileName);
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0) // Читаем и записываем файл частями
                    {
                        fs.Write(buffer, 0, bytesRead);
                    }
                }

                Console.WriteLine($"Файл '{fileName}' успешно получен и сохранен по пути: {filePath}"); // Выводим сообщение с путем к файлу
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении файла: {ex.Message}");
        }
    }
}
