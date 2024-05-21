using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Server
{
    // Порт для прослушивания соединений
    private const int Port = 8086;

    // Путь к директории с файлами
    private const string filesPath = @"C:\Users\Лучший\Documents";

    // Метод для запуска сервера
    public void Start()
    {
        // Создание TcpListener для прослушивания входящих соединений
        TcpListener listener = new TcpListener(IPAddress.Any, Port);
        listener.Start();
        Console.WriteLine("Сервер запущен и ожидает подключений...");

        // Цикл для ожидания новых клиентов
        while (true)
        {
            try
            {
                // Прием нового клиента
                TcpClient client = listener.AcceptTcpClient();

                // Создание нового потока для обработки клиента
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    // Метод для обработки клиента
    private void HandleClient(TcpClient client)
    {
        try
        {
            using (NetworkStream stream = client.GetStream())
            {
                // Чтение имени файла от клиента
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string fileName = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // Определение пути к файлу
                string filePath = Path.Combine(filesPath, fileName);
                if (File.Exists(filePath))
                {
                    // Отправка файла частями
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] fileBuffer = new byte[1024];
                        int bytesSent;
                        while ((bytesSent = fs.Read(fileBuffer, 0, fileBuffer.Length)) > 0)
                        {
                            stream.Write(fileBuffer, 0, bytesSent);
                        }
                    }
                }
                else
                {
                    // Отправка сообщения, если файл не найден
                    byte[] notFoundMessage = Encoding.UTF8.GetBytes("File not found");
                    stream.Write(notFoundMessage, 0, notFoundMessage.Length);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке клиента: {ex.Message}");
        }
        finally
        {
            // Закрытие соединения с клиентом
            client.Close();
        }
    }
}
