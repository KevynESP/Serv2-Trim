using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServidorCliente
{
    class Servidor
    {
        static bool puertoOcupado = true;
        static int puerto = 17;
        static string msg = "";

        static void Main(string[] args)
        {
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                do
                {
                    IPEndPoint ip = new IPEndPoint(IPAddress.Any, puerto);
                    try
                    {
                        s.Bind(ip);
                        puertoOcupado = false;
                    }
                    catch (Exception)
                    {
                        puerto++;
                    }
                } while (puertoOcupado);
                Console.WriteLine(puerto);
                s.Listen(10);

                while (msg != "Apagar")
                {
                    Socket sCliente = s.Accept();
                    Thread hilo = new Thread(hiloCLiente);
                    hilo.Start(sCliente);
                }

                Console.WriteLine("Se ha perdido la conexión");
                Console.ReadLine();
            }
        }
        static void hiloCLiente(object socket)
        {
            using (Socket sCliente = (Socket)socket)
            {
                IPEndPoint ipCliente = (IPEndPoint)sCliente.RemoteEndPoint;

                Console.WriteLine("Conectado cliente {0}", ipCliente.Address);

                using (NetworkStream ns = new NetworkStream(sCliente))
                using (StreamReader sr = new StreamReader(ns))
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    sw.WriteLine("te has conectado");
                    sw.Flush();
                    msg = sr.ReadLine();

                        try
                        {
                            switch (msg)
                            {
                                case "HORA":
                                    Console.WriteLine(msg);
                                    sw.WriteLine("Hora: {0}", DateTime.Now.ToString("T"));
                                    sw.Flush();
                                    break;
                                case "FECHA":
                                    Console.WriteLine(msg);
                                    sw.WriteLine("Date: {0}", DateTime.Now.ToString("d"));
                                    sw.Flush();
                                    break;
                                case "TODO":
                                    Console.WriteLine(msg);
                                    sw.WriteLine("Todo: {0}", DateTime.Now);
                                    sw.Flush();
                                    break;
                            }
                        }
                        catch (IOException e)
                        {
                            
                        }
                    Console.WriteLine("Se desconecto");
                }
            }  
        }
    }
}
