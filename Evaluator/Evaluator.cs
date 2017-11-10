using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;



namespace Evaluator
{
    class Evaluator
    {
        // class variables
        private Thread Receiver;
        private TcpClient client;

        // main run thread
        public void Run()
        {
            // create log
            try
            {
                TextWriterTraceListener loglisting = new TextWriterTraceListener("EvaluatorLog" + DateTime.Now.ToFileTimeUtc().ToString() + ".log");
                Trace.Listeners.Add(loglisting);
                Trace.AutoFlush = true;
            }
            catch (Exception)   // for log errors
            {
                Console.WriteLine("Logger failed to start.");
                return;  // end
            }


            try
            {
                // connect on port 9999 on home ip
                int port = 9999;

                TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();
                Trace.WriteLine("Starting up...");


                while (true)
                {
                    // create a non blocking client
                    client = listener.AcceptTcpClient();

                    Trace.WriteLine("Connecting...");

                    // Create the receive thread 
                    //  this thread allows for each connection to have its own thread
                    Receiver = new Thread(new ThreadStart(ReceiveThread));
                    Receiver.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e.Message);
            }
        }


        // parce the string into an equation and then back

        string ParceNum(string txtstr)
        {
            try
            {
                int result;        
                int location = txtstr.IndexOf('+');
                // -1 is returned if the answer is not found
                if (location == -1)
                {
                    return null;

                }
                //  parce each number
                //  any extra characters will cause an error
                //    Trace.WriteLine("LD" + location);
                string first = txtstr.Substring(0, location);
                //     Trace.WriteLine("first" + first + " len:" + txtstr.Length + " loc:" + location);
                string sec = txtstr.Substring(location + 1, txtstr.Length - location - 1);
                result = int.Parse(first) + int.Parse(sec);
                  
               
                return (result.ToString());
            }
            catch (Exception)  // all errors return null
            {
                Trace.WriteLine("Parse Error");
                return null;
            }
        }



        // communication thread that handles all requests
        private void ReceiveThread()
        {
            String request;
            String response;
            int recievedcount;
            Byte[] bytes = new Byte[256];

            try {

               NetworkStream stream = client.GetStream();

                while ((recievedcount = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // get the request
                    request = System.Text.Encoding.ASCII.GetString(bytes, 0, recievedcount);

                    Trace.WriteLine("Recieved:" + request);

                    // parse the request

                    response = ParceNum(request);
                    if (response == null)
                    {
                        response = "Error";
                    }

                    // send the response.
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);
                    stream.Write(msg, 0, msg.Length);
                    Trace.WriteLine("Responded:" + response);

                }

            }catch (Exception e)
            {
                Trace.WriteLine("Error:" + e.Message);
            }
        }
    }
}
