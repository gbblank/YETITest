using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace Generator
{
    class Generator
    {


        public void Run()
        {

           
            int port = 9999;
            int SLEEPTIME = 2000;
            int MAXINT = 9999;
            Byte[] bytes = new Byte[256];
            String responce;
            int recievedcount;
            string equation;


             // create log
            try
            {
                TextWriterTraceListener loglisting = new TextWriterTraceListener("GeneratorLog" + DateTime.Now.ToFileTimeUtc().ToString() + ".log");
                Trace.Listeners.Add(loglisting);
                Trace.AutoFlush = true;
                      
            }
            catch (Exception )   // for log errors
            {
                Console.WriteLine("Logger failed to start.");
                return;  // end
            }
        
            
            try
            {
                // connection info
                IPAddress address = IPAddress.Parse("127.0.0.1");
                TcpClient client = new TcpClient();
                client.Connect(address, port);
                NetworkStream stream = client.GetStream();

                Trace.WriteLine("Connecting...");


                // main request loop 
                while (true)
                {
                 
                    equation = "";
                    Random rnd = new Random();
                    int first = rnd.Next(0, MAXINT);   // range of random numbers 
                    int second = rnd.Next(0, MAXINT);
              
                    // generate + function
                    equation = first.ToString() + "+" + second.ToString();

                    Trace.WriteLine("Sending:" + equation);

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(equation);
                    stream.Write(msg, 0, msg.Length);
                    
                
                    recievedcount = stream.Read(bytes, 0, bytes.Length);
                    
                    // get the request
                    responce = System.Text.Encoding.ASCII.GetString(bytes, 0, recievedcount);

                    Trace.WriteLine("Recieved:" + responce);
                    Trace.WriteLine("Equation:"  + equation  + "=" +responce);
                    // parse the request


                    Thread.Sleep(SLEEPTIME);
                }
            }

            catch (Exception e )
            {
                Console.WriteLine(e.Message);
               
            }



        }
    }
}
