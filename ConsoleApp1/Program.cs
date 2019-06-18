using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;
using System.IO;


namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://habr.com/ru/post/424873/");

                var responce = client.GetAsync("https://habr.com/ru/post/424873/").Result;
                
                
                var content = responce.Content.ReadAsStringAsync().Result;
                               
                var htmlDoc = new HtmlDocument();

                htmlDoc.LoadHtml(content);

                var nodes = htmlDoc.DocumentNode.Descendants().SelectMany(n => n.Attributes.Where(a =>  a.Name == "src"));


                foreach (var item in nodes)
                {                    
                    var head = client.GetAsync(new Uri(client.BaseAddress, item.Value)).Result;

                    if (head.Content.Headers.ContentType.MediaType != "text/html")
                    {
                        var s = head.Content.Headers.ContentType.MediaType.Split('/').Last();


                        if (s == "javascript")
                        {
                            s = "js";
                        }


                        var file = File.Create($"{Guid.NewGuid().ToString()}.{s}");
                        using (Stream stream = head.Content.ReadAsStreamAsync().Result)
                        {
                            stream.CopyTo(file);
                        }
                                      

                    }

                    Console.WriteLine(head.Content.Headers.ContentType.ToString());

                }

                htmlDoc.Save("html.html",Encoding.UTF8);
                //foreach (var item in nodes)
                //{
                //    Console.WriteLine(item.Value);
                //}


            }
        }
    }
}
