using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

public class Client
{
    private const string serverAddress = "127.0.0.1"; // Заменить на IP адрес или доменное имя сервера
    private const int Port = 8086;

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

    public void GetFileByName(string fileName)
    {
        try
        {
            using (TcpClient client = new TcpClient(serverAddress, Port))
            using (NetworkStream stream = client.GetStream())
            {
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
                stream.Write(fileNameBytes, 0, fileNameBytes.Length);

                string filesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files");
                if (!Directory.Exists(filesDirectory))
                {
                    Directory.CreateDirectory(filesDirectory);
                }

                string filePath = Path.Combine(filesDirectory, fileName);
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, bytesRead);
                    }
                }
            }

            Console.WriteLine($"Файл '{fileName}' успешно получен и сохранен.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении файла: {ex.Message}");
        }
    }
}
