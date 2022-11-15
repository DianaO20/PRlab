using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace HttpClientApp
{
    class HttpClientApp
    {
        private static Uri uri = new Uri("http://localhost:8000/");
        static HttpClient client = new HttpClient() { BaseAddress = uri };
        static string fileName = @"C:\Users\Diana\Documents\output.txt";
        static async Task Main(string[] args)
        {
            //Here we check if an output file exists on that location, and if yes, we delete it, because we will create a new one
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            HttpClientApp program = new HttpClientApp();
            //Send request to the server
            await program.SendRequest();
            //Our work is done, send request to the server to shutdown
            await client.PostAsync(String.Concat(uri.ToString(), "shutdown"), null);
        }

        private async Task SendRequest()
        {
            string response = await client.GetStringAsync(uri);
            //We call this method to store the data that we recieved in the file.
            ProcessRequest(response);
            //Here we send a response with the number of tasks we have in the file to the server
            await SendResponse();
        }

        private void ProcessRequest(string response)
        {
            Console.WriteLine(response);
            WriteToFile(response);
        }

        private void WriteToFile(string text)
        {
            using (FileStream fs = File.Create(fileName))
            {
                // Add some text to file    
                byte[] input = new UTF8Encoding(true).GetBytes(text);
                fs.Write(input, 0, input.Length);
            }
        }

        private async Task SendResponse()
        {
            string result = "";
            using (StreamReader sr = File.OpenText(fileName))    
        {    
            string s = "";
            while ((s=sr.ReadLine()) != null)    
            {
                    result = String.Concat(result, s);    
            }    
        } 
            await client.PostAsync(String.Concat(uri.ToString(), "extract"), 
                new StringContent(String.Concat("Number of tasks currently in file: ", ToNumberOfTasks(result).ToString())));
        }

        private int ToNumberOfTasks(string output)
        {
            String[] tasks = output.Split(" ", 10);
            return tasks.Count();
        }

    }
}