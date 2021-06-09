using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ejercicio10
{
    class Sala
    {
        public static List<Socket> sClientes = new List<Socket>();
        static int port;
        static bool puertoOcupado = true;
        private static readonly object l = new object();

        static int leePuerto()
        {
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                using (StreamReader sr = new StreamReader(@"C:\temp\puerto.txt"))
                {
                    int.TryParse(sr.ReadLine(), out port);
                    if (port > 0 && port < 65535)
                    {
                        return port;
                    }
                    else
                    {
                        Console.WriteLine("Puerto no válido");
                        return 10000;
                    }
                }
            }
            
        }

        static void envioMensaje(string m, IPEndPoint ie)
        {
            lock (l)
            {
                foreach (Socket socket in sClientes)
                {

                        IPEndPoint siP = (IPEndPoint)socket.RemoteEndPoint;
                        using (NetworkStream ns = new NetworkStream(socket))
                        using (StreamWriter sw = new StreamWriter(ns))
                        {
                            if (siP.Port != ie.Port)
                            {
                                sw.WriteLine("IP: {0} {1}", ie.Address, m);
                                sw.Flush();
                            }
                        }
                }
            }
        }

        public void iniciaServicioChat()
        {
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                port = leePuerto();
                IPEndPoint ie = new IPEndPoint(IPAddress.Any, port);
                do
                {
                    try
                    {
                        s.Bind(ie);
                        puertoOcupado = false;
                    }
                    catch (Exception)
                    {
                        port++;
                        if (port > 65535)
                        {
                            port = 10000;
                        }
                    }
                } while (puertoOcupado);
                Console.WriteLine(port);
                s.Listen(10);


                while (true)
                {
                    Socket sCliente = s.Accept();
                    Thread hCliente = new Thread(hiloCliente);
                    hCliente.Start(sCliente);

                    lock (this)
                    {
                        sClientes.Add(sCliente);
                    }
                }
            }
        }

        static void hiloCliente(object socket)
        {
            Socket cliente = (Socket)socket;
            using (cliente)
            {
                IPEndPoint iP = (IPEndPoint)cliente.RemoteEndPoint;
                string mesage = "";
                using (NetworkStream ns = new NetworkStream(cliente))
                using (StreamWriter sw = new StreamWriter(ns))
                using (StreamReader sr = new StreamReader(ns))
                {
                    
                    Console.WriteLine("IP: {0} Puerto: {1}", iP.Address, iP.Port);
                    sw.WriteLine("Bienvenido terricola");
                    sw.Flush();


                    while (mesage != "MELARGO" && mesage != null)
                    {
                        try
                        {
                            mesage = sr.ReadLine();

                            if (mesage != null)
                            {
                                envioMensaje(mesage, iP);
                                Console.WriteLine(mesage);
                            }
                        }
                        catch (Exception)
                        {

                            
                        }
                    }
                }
            }
            lock (l)
            {
                sClientes.Remove(cliente);
            }
            Console.WriteLine("Se cerro conexión");
        }
    }
}
