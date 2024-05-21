using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Server
{
    private const int Port = 8086;
    private const string filesPath = @"C:\Users\Лучший\Documents";

    public void Start()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, Port);
        listener.Start();
        Console.WriteLine("Сервер запущен и ожидает подключений...");

        while (true)
        {
            try
            {
                TcpClient client = listener.AcceptTcpClient();
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    private void HandleClient(TcpClient client)
    {
        try
        {
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string fileName = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                string filePath = Path.Combine(filesPath, fileName);
                if (File.Exists(filePath))
                {
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
            client.Close();
        }
    }
}
