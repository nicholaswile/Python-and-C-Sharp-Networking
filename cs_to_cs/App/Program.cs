// Created 2023-10-30 by NW

// Main C# program

namespace MyNetwork {
    class Program {
        async public static Task Main(string[] args) {
            if (args.Length > 0) {
                switch (args[0]){
                    case ("SERVER"): 
                        Console.WriteLine("Running Server");
                        await Server.Server_Main();
                        break;
                    case ("CLIENT"): 
                        Console.WriteLine("Running Client");
                        await Client.Client_Main();
                        break;
                    default: break;
                }
            }
            else {
                Console.WriteLine("Hello world!");
            }
        }
    }
        
}