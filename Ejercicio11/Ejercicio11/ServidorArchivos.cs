using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ejercicio11
{
    class ServidorArchivos
    {
        bool funcionando = true;
        public string leeArchivo(string nombre, int nLineas)
        {
            string resultado = "", linea = "";
            int cont = 0;
            try
            {
                using (StreamReader sr = new StreamReader(Environment.GetEnvironmentVariable("EXAMEN") + "\\" + nombre))
                {
                    while ((linea = sr.ReadLine()) != null && cont < nLineas)
                    {
                        resultado += linea;
                        cont++;
                    }
                }
                return resultado;
            }
            catch (IOException)
            {
                return "<ERROR_IO>";
            }
        }

        public int leePuerto()
        {
            if (int.TryParse(leeArchivo("puerto.txt", 1), out int puerto))
            {
                if (puerto < IPEndPoint.MaxPort && puerto > IPEndPoint.MinPort)
                {
                    return puerto;
                }
                else
                {
                    return 31416;
                }
            }
            else
            {
                return 31416;
            }
        }

        public void guardaPuerto(int numero)
        {
            using (StreamWriter sw = new StreamWriter(Environment.GetEnvironmentVariable("EXAMEN") + "\\puerto.txt"))
            {
                sw.WriteLine(numero);
            }
        }

        public string listaArchivos()
        {
            string files = "";
            DirectoryInfo di = new DirectoryInfo(Environment.GetEnvironmentVariable("EXAMEN"));
            foreach (FileInfo file in di.GetFiles())
            {
                if (file.Extension == ".txt")
                {
                    files += file.Name + "\n";
                }
            }
            return files;
        }

        public void iniciaServidorArchivos()
        {
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                int puerto = leePuerto();
                IPEndPoint ie = new IPEndPoint(IPAddress.Any, puerto);

                try
                {
                    s.Bind(ie);
                }
                catch (Exception)
                {
                    Console.WriteLine("Puerto Ocupado");
                    funcionando = false;
                }
                s.Listen(10);
                Console.WriteLine("Puerto: {0}", ie.Port);

                while (funcionando)
                {
                    Socket sCliente = s.Accept();
                    Thread hiloCliente = new Thread(HiloCliente);
                    hiloCliente.Start(sCliente);
                }
            }
        }

        public void HiloCliente(object socket)
        {
            string msg = "";
            string archivo = "";
            int n = 0;
            Socket sCLiente = (Socket)socket;
            using (sCLiente)
            {
                IPEndPoint ieCliente = (IPEndPoint)sCLiente.RemoteEndPoint;
                Console.WriteLine("Ip: "+ieCliente.Address+" Puerto: "+ieCliente.Port);
                using (NetworkStream ns = new NetworkStream(sCLiente))
                using (StreamWriter sw = new StreamWriter(ns))
                using (StreamReader sr = new StreamReader(ns))
                {
                    sw.WriteLine("CONEXION ESTABLECIDA");
                    sw.Flush();
                    while (msg != null && msg != "CLOSE" && msg != "HALT" && funcionando)
                    {
                        try
                        {
                            msg = sr.ReadLine();
                            string[] palabras = msg.Split(' ', ',');
                            switch (palabras[0])
                            {
                                case "GET":
                                    archivo = palabras[1];
                                    Int32.TryParse(palabras[2], out n);
                                    Console.WriteLine(archivo+","+n);
                                    sw.WriteLine(leeArchivo(archivo, n));
                                    sw.Flush();
                                    break;
                                case "PORT":
                                    Int32.TryParse(palabras[1], out n);
                                    guardaPuerto(n);
                                    break;
                                case "LIST":
                                    sw.WriteLine(listaArchivos());
                                    sw.Flush();
                                    break;
                                case "HALT":
                                    funcionando = false;
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (IOException)
                        {

                        }
                    }
                }
            }
            Console.WriteLine("Se ha perdido la conexion");
        }
    }
}
