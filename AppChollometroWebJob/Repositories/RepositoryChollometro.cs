using AppChollometroWebJob.Data;
using AppChollometroWebJob.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppChollometroWebJob.Repositories
{
    public class RepositoryChollometro
    {
        private ChollometroContext context;

        public RepositoryChollometro(ChollometroContext context)
        {
            this.context = context;
        }

        public async Task<int> GetMaxCholloIdAsync()
        {
            if(this.context.Chollos.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await this.context.Chollos.MaxAsync(x => x.IdChollo) + 1;
            }
        }

        public async Task<List<Chollo>> GetChollosWebAsync()
        {
            string url = "https://www.chollometro.com/rss";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = @"text/html application/xhtml+xml, *.*";
            request.Referer = "https://www.chollometro.com/";
            request.Headers.Add("Accept-language", "es-ES");
            request.Host = "www.chollometro.com";
            request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
            HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync();
            string xmlData = "";
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                xmlData = await reader.ReadToEndAsync();
            }

            XDocument document = XDocument.Parse(xmlData);
            var consulta = from datos in document.Descendants("item")
                           select datos;
            List<Chollo> chollosList = new List<Chollo>();
            int idchollo = await this.GetMaxCholloIdAsync();
            foreach(var tag in consulta)
            {
                Chollo chollo = new Chollo();
                chollo.IdChollo = idchollo;
                chollo.Titulo = tag.Element("title").Value;
                chollo.Link = tag.Element("link").Value;
                chollo.Descripcion = tag.Element("description").Value;
                chollo.Fecha = DateTime.Now;
                idchollo += 1;
                chollosList.Add(chollo);
            }
            return chollosList;
        }

        public async Task PopulateChollosAzureAsync()
        {
            List<Chollo> chollos = await this.GetChollosWebAsync();
            foreach(Chollo c in chollos)
            {
                this.context.Chollos.Add(c);
            }
            await this.context.SaveChangesAsync();
        }
    }
}
