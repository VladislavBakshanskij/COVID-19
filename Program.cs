﻿using AngleSharp.Dom;
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

namespace Covid_19 {
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

            return new Coronavirus(){
                Cases = int.Parse(result[0].Replace(",", "")), 
                Death = int.Parse(result[1].Replace(",", "")), 
                Recovered = int.Parse(result[2].Replace(",", "")),
                Date = DateTime.Now
            };
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
                Console.WriteLine(coronavirus.ToString());
            } else {
                Console.WriteLine(coronavirus.ToString());

                Console.WriteLine("\n--------------------------------------------------------------------------------------------------------------------------------\n");

                Console.WriteLine(oldCoronavirusState.ToString());

                Console.WriteLine("\n--------------------------------------------------------------------------------------------------------------------------------\n");

                Console.WriteLine($"За период {oldCoronavirusState.Date} - {coronavirus.Date} прошло {coronavirus.Date - oldCoronavirusState.Date}");
                Console.WriteLine($"--Число зараженных изменилось на: {coronavirus.Cases - oldCoronavirusState.Cases}");
                Console.WriteLine($"--Число умерших изменилось на: {coronavirus.Death - oldCoronavirusState.Death}");
                Console.WriteLine($"--Число выживших изменилось на: {coronavirus.Recovered - oldCoronavirusState.Recovered}");
            }

            Save(coronavirus);
            Console.Read();
        }
    }
}
