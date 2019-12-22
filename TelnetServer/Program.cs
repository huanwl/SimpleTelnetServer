using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SynchronousSocketListener
{
    public static void Main(String[] args)
    {
        string data = "";
        byte[] bytes = new Byte[1];

        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 23);

        Socket listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        listener.Bind(localEndPoint);
        listener.Listen(10);

        // Start listening for connections.  
        while (true)
        {
            Console.WriteLine("Waiting for a connection...");
            Socket handler = listener.Accept();
            bool isExit = false;

            while (true)
            {
                handler.Send(Encoding.ASCII.GetBytes("Send: "));
                while (true)
                {
                    int bytesRec = handler.Receive(bytes, 1, SocketFlags.None);
                    Console.WriteLine(bytes[0]);

                    if (bytes[0] == 13) // enter
                    {
                        break;
                    }
                    else if (bytes[0] == 3) // ctrl + c
                    {
                        isExit = true;
                        break;
                    }
                    else if (bytes[0] == 8) // backspace
                    {
                        data = data.Substring(0, data.Length - 1);
                        handler.Send(new byte[] { 32, 8 });
                    }
                    else
                    {
                        data += Encoding.ASCII.GetString(bytes);
                    }
                }

                if (isExit)
                {
                    break;
                }

                var result = CelsiusToFahrenheit(data);
                handler.Send(Encoding.ASCII.GetBytes(string.Format("Result: {0:0.00}\r\n", result)));
                //handler.Send(new byte[] { 67, 82 });
                data = "";
            }

            handler.Send(Encoding.ASCII.GetBytes("Exit"));
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }

    public static double CelsiusToFahrenheit(string temperatureCelsius)
    {
        double celsius = System.Double.Parse(temperatureCelsius);
        return (celsius * 9 / 5) + 32;
    }
}