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
    class Program
    {
        static void Main(string[] args)
        {
            // устанавливаем соединение с БД
            string host = "s2.kts.tu-bryansk.ru";
            int port = 3306;
            string database = "17IAS-AMISI_MarekhinaSA";
            string username = "17IAS-AMISI.MarekhinaSA";
            string password = "!cbK>ngTmiYxS&xH";
            MySqlConnection connection = new MySqlConnection("Server=" + host + ";Database=" + database + ";port=" + port + ";User Id=" + username + ";password=" + password);
            connection.Open();
            MySqlCommand command;
            MySqlDataReader reader;
            string sql;
            //  ParserRedion(connection);

            /*    // подгружаем html страницу
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.GetEncoding("windows-1251");
                string region = wc.DownloadString("https://www.audit-it.ru/sro/regions.php");

                /*  //парсем ссылки регионов
                   string reg = "<option value=\"(.*?)\" >(.*?)</option>";
                   MatchCollection mc = Regex.Matches(region, reg);
                   for (int j = 0; j < mc.Count; j++)
                   {
                       sql = " INSERT INTO region( Title, Link) VALUES('" + mc[j].Groups[2].Value + "', 'https://www.audit-it.ru/sro/list.php?regions=" + mc[j].Groups[1].Value + "')";
                       command = new MySqlCommand(sql, connection);
                       // объект для чтения ответа сервера
                       reader = command.ExecuteReader();
                       reader.Close(); // закрываем reader
                   }*/

            sql = "SELECT * FROM region";
            command = new MySqlCommand(sql, connection);
            // объект для чтения ответа сервера
            reader = command.ExecuteReader();
            List<Region> listRegion = new List<Region>();
            while (reader.Read())
            {
                listRegion.Add(new Region(int.Parse(reader[0].ToString()), reader[1].ToString(), reader[2].ToString()));
            }
            reader.Close(); // закрываем reader
            ParserCompany(connection, listRegion);
            // закрываем соединение с БД
            connection.Close();

            Console.WriteLine("КОНЕЦ");
            Console.ReadLine();
        }
        static private void ParserCompany(MySqlConnection connection, List<Region> regions)
        {
            MySqlCommand command;
            MySqlDataReader reader;
            string sql;
            // подгружаем html страницу
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.GetEncoding("windows-1251");
            string company;

            for (int i = 0; i < regions.Count; i++)
            {
                try
                {
                    company = wc.DownloadString(regions[i].Link);
                    // Console.WriteLine(company);

                    string id= regions[i].Link.Remove(0,45);
                    string regCount = "&gt;</a></li><li><a href=\"/sro/list.php\\?regions=" +id + "&PAGE_NUMBER=(.*?)\"\\s title";
                    MatchCollection mcCount = Regex.Matches(company, regCount);
                    Console.WriteLine(mcCount.Count);
                    int count = int.Parse(mcCount[0].Groups[1].Value);
                    for (int j = 1; j <= count; j++)
                    {

                        company = wc.DownloadString(regions[i].Link+ "&PAGE_NUMBER=" + i);
                    //    Console.WriteLine(company);
                        string reg1 = "<a class=\"post\" href=\"(.*?)\">(.*?)<span class='grey'>(.*?)</span>";
                        string reg2 = "ААС</a></td>\\s.?(.*?)</td>";//<td>(.*?)";///s((.*?)) ";
                        MatchCollection mc = Regex.Matches(company, reg1);
                        MatchCollection mc2 = Regex.Matches(company, reg2);//огрн орнз
                        for (int n = 0; n < mc.Count; n++)
                        {
                            string title = mc[j].Groups[2].Value;
                            int indexOfChar1 = title.IndexOf("АФ &quot;");
                            int indexOfChar2 = title.IndexOf("Аудиторская фирма &quot;");
                            int indexOfChar3 = title.IndexOf("Фирма &quot;");
                            if (indexOfChar1 > -1)
                            {
                                title = title.Remove(0, 9);
                                title = title.Remove(title.Length - 8);
                            }
                            else if (indexOfChar2 > -1)
                            {
                                title = title.Remove(0, 24);
                                title = title.Remove(title.Length - 8);
                            }
                            else if (indexOfChar3 > -1)
                            {
                                title = title.Remove(0, 13);
                                title = title.Remove(title.Length - 8);
                            }
                            else title = title.Remove(title.Length - 2);
                            Console.WriteLine(mc[j].Groups[1].Value);//link
                            Console.WriteLine(title);//mc[j].Groups[2].Value; наименование организации
                            string type = mc[j].Groups[3].Value;
                            type = type.Trim(new char[] { ' ' });
                            Console.WriteLine(type);//вид

                            string s = mc2[j].Groups[1].Value;
                            s = s.Remove(0, 6);
                            string[] ss = s.Split(' ');
                            Console.WriteLine(ss[0]);//огрн
                            ss[1] = ss[1].Remove(0, 1);
                            ss[1] = ss[1].Remove(ss[1].Length - 1);
                            Console.WriteLine(ss[1]);//орнз

                            /*   sql = "INSERT INTO organization (`Title`, `ОГРН`, `ОРНЗ`, `Link`, `Type`, `Region_ID`) VALUES ('fff', 'fs', '5455', '4654', '545', '456', '4', '90')";
                              command = new MySqlCommand(sql, connection);
                               // объект для чтения ответа сервера
                               reader = command.ExecuteReader();
                               reader.Close(); // закрываем reader*/
                        }
                    }
                        
                    Console.WriteLine("ВРЕМЯ");
                    System.Threading.Thread.Sleep(30000);
                }
                catch (WebException e)
                {
                    Console.WriteLine(e);
                    System.Threading.Thread.Sleep(300000);
                    i--;
                }

            }
        }
        static private void ParserRedion(MySqlConnection connection)
        {
            MySqlCommand command;
            MySqlDataReader reader;
            string sql;
            // подгружаем html страницу
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.GetEncoding("windows-1251");
            string region = wc.DownloadString("https://www.audit-it.ru/sro/regions.php");

            //парсем ссылки регионов
            string reg = "<option value=\"(.*?)\" >(.*?)</option>";
            MatchCollection mc = Regex.Matches(region, reg);
            for (int j = 0; j < mc.Count; j++)
            {
                sql = " INSERT INTO region( Title, Link) VALUES('" + mc[j].Groups[2].Value + "', 'https://www.audit-it.ru/sro/list.php?regions=" + mc[j].Groups[1].Value + "')";
                command = new MySqlCommand(sql, connection);
                // объект для чтения ответа сервера
                reader = command.ExecuteReader();
                reader.Close(); // закрываем reader
            }
        }
    }
}
