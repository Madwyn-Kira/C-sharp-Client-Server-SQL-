using System;
using System.Configuration;                                        
using System.Data.Common;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Data;                                                // База данных локальная, для изменения пути переходим в app.Config и меняем путь до базы данных.


namespace Vitakor_Server
{
    class Server
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["VeterenarDB"].ConnectionString;
        const int Port = 80;
        static void Main(string[] args)
        {
            // Подключаемся к БД

            Console.WriteLine("Getting Connection ...");
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                Console.WriteLine("Openning Connection ...");
                conn.Open();
                Console.WriteLine("Connection successful!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            // Сервер

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);

            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listenSocket.Bind(ipPoint);

                listenSocket.Listen(10);

                Console.WriteLine("Server is started");

                while (true)
                {
                    Socket handler = listenSocket.Accept();

                    // Получаем

                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    byte[] data = new byte[256];

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    SqlCommand sqlCommand = new SqlCommand(builder.ToString(), conn);
                    SqlDataReader sqlDataReader = null;

                    switch (builder.ToString().Split(' ')[0].ToLower())
                    {
                        case "select":

                            sqlDataReader = sqlCommand.ExecuteReader();

                            while (sqlDataReader.Read())
                            {
                                Console.WriteLine($"{sqlDataReader["Id"]} {sqlDataReader["Doctor"]} {sqlDataReader["Owner"]} {sqlDataReader["Service"]} {sqlDataReader["Vaccination"]}");
                            }

                            if (sqlDataReader != null)
                                sqlDataReader.Close();

                            break;
                        case "insert":

                            Console.WriteLine($"Добавлено {sqlCommand.ExecuteNonQuery()} строк");      // Пример команды со стороны клиента:
                                                                                                       // Select * From 
                            break;
                        case "update":

                            Console.WriteLine($"Изменено {sqlCommand.ExecuteNonQuery()} строк");

                            break;
                        case "delete":

                            Console.WriteLine($"Удалено {sqlCommand.ExecuteNonQuery()} строк");

                            break;
                        default:

                            Console.WriteLine($"Команда {builder.ToString()} неправильна!");

                            break;
                    }

                    // Отправляем

                    string message = "Доставлено";
                    data = Encoding.Unicode.GetBytes(message);
                    handler.Send(data);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
