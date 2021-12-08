using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ParserLink
{
    class Region
    {
        MySqlConnection Connection { get; set; }
        MySqlCommand command;
        MySqlDataReader reader;

       public int ID;
        public string Title;
        public string Link;
       public Region(int id, string title, string link)
        {
            ID = id;
            Title = title;
            Link = link;
        }
      /*  private void ParserRedion()
        {
            // подгружаем html страницу
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.GetEncoding("windows-1251");
            string region = wc.DownloadString("https://www.audit-it.ru/sro/regions.php");

            //парсем ссылки регионов
            string reg = "<option value=\"(.*?)\" >(.*?)</option>";
            MatchCollection mc = Regex.Matches(region, reg);
            string sql;
            for (int j = 0; j < mc.Count; j++)
            {
                sql = " INSERT INTO region( Title, Link) VALUES('" + mc[j].Groups[2].Value + "', 'https://www.audit-it.ru/sro/list.php?regions=" + mc[j].Groups[1].Value + "')";
                command = new MySqlCommand(sql, Connection);
                // объект для чтения ответа сервера
                reader = command.ExecuteReader();
                reader.Close(); // закрываем reader
            }
            Connection.Close();
        }*/

    }
}
