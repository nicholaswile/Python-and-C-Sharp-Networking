// Created 2023-11-27 by NW 

// Main C# program [now with file transfer, only works with running Python server]

namespace MyNetwork {
    class Program {
        async public static Task Main(string[] args) {
            if (args.Length > 0) {
                switch (args[0]){
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