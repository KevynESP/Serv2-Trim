﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio10
{
    class Program
    {
        static void Main(string[] args)
        {
            Sala sala = new Sala();
            sala.iniciaServicioChat();

 
            Console.ReadKey();
        }
    }
}
