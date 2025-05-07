using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BorisProxy;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

class HttpsTcpServer
{

    
    static void Main()
    {
        List<Process> RunningProcesses = new List<Process>();
        ConfigRoot cfg = JsonConvert.DeserializeObject<ConfigRoot>(File.ReadAllText("config.json"));

        Console.WriteLine("Loading Sinks");

        Dictionary<string, TcpSink> sinks = new Dictionary<string, TcpSink>();

        foreach(dynamic sink in cfg.run.sinks)
        {
            Console.WriteLine("\t"+sink.name);
            TcpSink psink = null;

            if(sink.type == "HTTP")
            {
                psink = new TcpClientSink((string)sink.addr,(short)sink.port);
            }else if(sink.type == "STATIC")
            {
                psink = new SimpleStaticSink((string)sink.path); 
            }

            if(psink == null)throw new Exception("Unknown sink");
            sinks.Add((string)sink.name,psink);
        }

        Console.WriteLine("Loading Routers");

        Dictionary<string, TcpSink> routers = new Dictionary<string, TcpSink>();

        foreach(dynamic router in cfg.run.routers)
        {
            TcpSink rsink = null;
            if(router.type == "HDRSEL")
            {
                rsink = new HttpHeaderDecisionSink((string)router.key);
            }
            if(rsink == null)throw new Exception("Unknown Router");
            routers.Add((string)router.name,rsink);
        }

        Console.WriteLine("Processing router references");

        foreach(dynamic router in cfg.run.routers)
        {
            if(router.type == "HDRSEL")
            {
                Dictionary<string,TcpSink> dta = new Dictionary<string, TcpSink>();
                TcpSink selsink = null;
                foreach(dynamic choice in router.choices)
                {   
                    if(sinks.ContainsKey((string)choice.value))
                    {
                        selsink = sinks[(string)choice.value];
                    }
                    else if(routers.ContainsKey((string)choice.value))
                    {
                        selsink = routers[(string)choice.value];
                    }else{
                        throw new Exception("Router/Sink not found");
                    }
                    dta.Add((string)choice.key,selsink);
                }

                if(sinks.ContainsKey((string)router.def))
                {
                    selsink = sinks[(string)router.def];
                }
                else if(routers.ContainsKey((string)router.def))
                {
                    selsink = routers[(string)router.def];
                }else{
                    throw new Exception("Router/Sink not found");
                }

                ((HttpHeaderDecisionSink)routers[(string)router.name]).ProcessReferences(dta,selsink);
            }
            else throw new Exception("Unknown Router");
        }

        Console.WriteLine("Finalsing References");

        foreach(KeyValuePair<string,TcpSink> router in routers)
        {
            if(router.Value is HttpHeaderDecisionSink)((HttpHeaderDecisionSink)router.Value).FinalizeReferences();
        }

        Console.WriteLine("Loadink Sources");

        Dictionary<string,TcpSource> sources = new Dictionary<string, TcpSource>();

        foreach(dynamic source in cfg.run.sources)
        {
            Console.WriteLine((string)source.name);

            TcpSource src = null;

            if(source.type == "HTTPS")
            {
                src = new SslListener(IPAddress.Parse((string)source.addr),(short)source.port,(string)source.cert_path,(string)source.cert_pass);
            }else if(source.type == "HTTP")
            {
                src = new BORISHttpListener(IPAddress.Parse((string)source.addr),(short)source.port);
            }

            if(src == null)throw new Exception("Unknown Source");
            src.AssignState((string)source.name);
            sources.Add((string)source.name,src);

            TcpSink selsink = null;
            if(sinks.ContainsKey((string)source.conn))
            {
                selsink = sinks[(string)source.conn];
            }
            else if(routers.ContainsKey((string)source.conn))
            {
                selsink = routers[(string)source.conn];
            }else{
                throw new Exception("Router/Sink not found");
            }

            src.SetProcessingFunction(selsink.CreatePfun());
        }

        Console.WriteLine("Starting up sources");

        foreach(KeyValuePair<string, TcpSource> srcs in sources)
        {
            srcs.Value.Start();
        }

        foreach(dynamic exec in cfg.exec)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = (string)exec.file;
            psi.Arguments = (string)exec.args;
            Process proc = Process.Start(psi);
            RunningProcesses.Add(proc);
        }

        while(true)
        {
            Console.Clear();

            Console.WriteLine(DateTime.Now.ToLongTimeString());
            Console.WriteLine();

            foreach(ServerStateDisplay.SourceState state in ServerStateDisplay.states)
            {
                Console.WriteLine($"{state.name} U: {state.upload} D: {state.download}");
            }

            Console.WriteLine($"{RunningProcesses.ToArray().Count((r)=>!r.HasExited)} Processes running");

            Thread.Sleep(200);
            if(Console.KeyAvailable)
            {
                string cmd = Console.ReadLine();
                if(cmd.ToLower() == "quit")
                {
                    Console.WriteLine("Quitting");
                    foreach(Process proc in RunningProcesses)
                    {
                        if(!proc.HasExited)proc.Kill();
                    }
                    Environment.Exit(0);
                }
            }
        }
    }
}
