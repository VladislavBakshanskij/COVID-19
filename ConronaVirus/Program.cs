using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoronaVirus {
    public class Program {
        private static readonly string Path = Environment.CurrentDirectory + @"/result.json";
        
        private static async Task<string> LoadSourceHtml(string url = "https://www.worldometers.info/coronavirus/") {
            string source = string.Empty;
            
            using (HttpClient httpClient = new HttpClient()) 
                using (HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url)) 
                    if (httpResponseMessage != null && httpResponseMessage.StatusCode == HttpStatusCode.OK) 
                        source = await httpResponseMessage.Content.ReadAsStringAsync();
            return source;
        }

        private static Coronavirus GetCoronaState(string source) {
            HtmlParser htmlParser = new HtmlParser();
            IHtmlDocument htmlDocument = htmlParser.ParseDocument(source);

            IEnumerable<IElement> elements = htmlDocument.QuerySelectorAll("div");
            elements = elements.Where(tag => tag.ClassName != null && tag.ClassName.Contains("maincounter"));

            List<string> result = new List<string>();
            foreach (IElement element in elements)
                result.Add(element.TextContent.Replace("\n", string.Empty));

            return new Coronavirus(int.Parse(result[0].Replace(",", "")), int.Parse(result[1].Replace(",", "")), int.Parse(result[2].Replace(",", "")));
        }

        private static void Save(Coronavirus coronavirus) {
            string json = string.Empty;
            using (FileStream fs = new FileStream(Path, FileMode.OpenOrCreate))
                json = JsonSerializer.Serialize(coronavirus, typeof(Coronavirus));
            File.WriteAllText(Path, json);
        }

        private static Coronavirus Read() {
            if (!File.Exists(Path))
                return null;
            string text = File.ReadAllText(Path);
            Coronavirus coronavirus = JsonSerializer.Deserialize(text, typeof(Coronavirus)) as Coronavirus;
            return coronavirus;
        }

        public static void Main(string[] args) {
            string html = LoadSourceHtml().Result;
            Coronavirus coronavirus = GetCoronaState(html);
            Coronavirus oldCoronavirusState = Read();

            if (oldCoronavirusState == null) {
                Console.WriteLine($"Дата: {coronavirus.Date}");
                Console.WriteLine($"--Число зараженных: {coronavirus.Cases}");
                Console.WriteLine($"--Число умерших: {coronavirus.Death}");
                Console.WriteLine($"--Число выживших: {coronavirus.Recovered}");
            } else {
                Console.WriteLine($"Дата: {coronavirus.Date}");
                Console.WriteLine($"--Число зараженных: {coronavirus.Cases}");
                Console.WriteLine($"--Число умерших: {coronavirus.Death}");
                Console.WriteLine($"--Число выживших: {coronavirus.Recovered}");

                Console.WriteLine("\n----------------------------------------------------------\n");
                
                Console.WriteLine($"Дата: {oldCoronavirusState.Date}");
                Console.WriteLine($"--Число зараженных: {oldCoronavirusState.Cases}");
                Console.WriteLine($"--Число умерших: {oldCoronavirusState.Death}");
                Console.WriteLine($"--Число выживших: {oldCoronavirusState.Recovered}");

                Console.WriteLine("\n----------------------------------------------------------\n");

                Console.WriteLine($"За период {oldCoronavirusState.Date} - {coronavirus.Date}");
                Console.WriteLine($"--Число зараженных изменилось на: {coronavirus.Cases - oldCoronavirusState.Cases}");
                Console.WriteLine($"--Число умерших изменилось на: {coronavirus.Death - oldCoronavirusState.Death}");
                Console.WriteLine($"--Число выживших изменилось на: {coronavirus.Recovered - oldCoronavirusState.Recovered}");
            }
            Save(coronavirus);

            Console.Read();
        }
    }
}
