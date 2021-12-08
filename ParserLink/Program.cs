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

            // парсер для компании
            /*   sql = "SELECT * FROM region";
               command = new MySqlCommand(sql, connection);
               // объект для чтения ответа сервера
               reader = command.ExecuteReader();
               List<Region> listRegion = new List<Region>();
               while (reader.Read())
               {
                   listRegion.Add(new Region(int.Parse(reader[0].ToString()), reader[1].ToString(), reader[2].ToString()));
               }
               reader.Close(); // закрываем reader
               ParserCompany(connection, listRegion);*/

            // парсер доп. данных по комнапии
            /* List<int> listID = new List<int>();
             List<string> listLink = new List<string>();
             sql = "SELECT ID, Link FROM organization";
             command = new MySqlCommand(sql, connection);
             // объект для чтения ответа сервера
             reader = command.ExecuteReader();
             while (reader.Read())
             {
                 listID.Add(int.Parse(reader[0].ToString()));
                 listLink.Add(reader[1].ToString());
             }
             reader.Close(); // закрываем reader
             ParserADDInfoCompany(connection,listID, listLink);
             */
            // парсер бух учет
            List<int> listID = new List<int>();
            List<string> linkAccounting = new List<string>();
            sql = "SELECT ID, `Link accounting`  FROM organization";
            command = new MySqlCommand(sql, connection);
            // объект для чтения ответа сервера
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                listID.Add(int.Parse(reader[0].ToString()));
                linkAccounting.Add(reader[1].ToString());
            }
            reader.Close(); // закрываем reader
            ParserADDInfoCompany(connection, listID, linkAccounting);



            // закрываем соединение с БД
            connection.Close();

            Console.WriteLine("КОНЕЦ");
            Console.ReadLine();
        }
        /*      static private void ParserAccounting(MySqlConnection connection, List<int> listID, List<string> linkAccounting)// парсер бух учета
              {
                  MySqlCommand command;
                  MySqlDataReader reader;
                  string sql;
                  // подгружаем html страницу
                  Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                  WebClient wc = new WebClient();
                  wc.Encoding = Encoding.GetEncoding("windows-1251");
                  string accountingCompany;
                  for (int i = 0; i < listID.Count; i++)
                  {
                      try
                      {
                          accountingCompany = wc.DownloadString(LinkAccounting[i]);
                          Console.WriteLine(company);

                          string id = regions[i].Link.Remove(0, 45);
                          string regCount = "&gt;</a></li><li><a href=\"/sro/list.php\\?regions=" + id + "&PAGE_NUMBER=(.*?)\"\\s title";
                          MatchCollection mcCount = Regex.Matches(company, regCount);
                          //    Console.WriteLine(mcCount.Count);
                          int count;
                          if (mcCount.Count == 0) count = 1;
                          else count = int.Parse(mcCount[0].Groups[1].Value);
                          for (int n = 1; n <= count; n++)
                          {

                              company = wc.DownloadString(regions[i].Link + "&PAGE_NUMBER=" + n);
                              //    Console.WriteLine(company);
                              string reg1 = "<a class=\"post\" href=\"(.*?)\">(.*?)<span class='grey'>(.*?)</span>";
                              string reg2 = "ААС</a></td>\\s.?(.*?)</td>";
                              MatchCollection mc = Regex.Matches(company, reg1);
                              MatchCollection mc2 = Regex.Matches(company, reg2);//огрн орнз
                              for (int j = 0; j < mc.Count; j++)
                              {
                                  string title = mc[j].Groups[2].Value;
                                  int indexOfChar1 = title.IndexOf("&quot;");
                                  if (indexOfChar1 > -1) title = title.Replace("&quot;", "\"");


                                  title = title.Remove(title.Length - 2);
                                  string link = "https://www.audit-it.ru" + mc[j].Groups[1].Value;
                                  Console.WriteLine(link);//link
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

                                  if (gg == true)
                                  {
                                      //       sql = "UPDATE `17ias-amisi_marekhinasa`.`organization` SET `Title` = '" + title + "', `ОГРН` = '" + ss[0] + "', `ОРНЗ` = '" + ss[1] + "', `Link` = '" + link + "', `Type` = '" + type + "', `Region_ID` = '" + regions[i].ID + "' WHERE (`ID` = '2377')";
                                      sql = "INSERT INTO organization (`Title`, `ОГРН`, `ОРНЗ`, `Link`, `Type`, `Region_ID`) VALUES ('" + title + "', '" + ss[0] + "', '" + ss[1] + "', '" + link + "', '" + type + "', '" + regions[i].ID + "')";
                                      command = new MySqlCommand(sql, connection);
                                      // объект для чтения ответа сервера
                                      reader = command.ExecuteReader();
                                      reader.Close(); // закрываем reader
                                  }

                                  if (title == "АКФ \"МИАН\"")
                                  {
                                      gg = true;
                                  }

                              }
                              System.Threading.Thread.Sleep(10000);
                          }

                          Console.WriteLine("ВРЕМЯ____________________________________________________");
                          System.Threading.Thread.Sleep(50000);
                      }
                      catch (WebException e)
                      {
                          Console.WriteLine(e);
                          System.Threading.Thread.Sleep(300000);
                          i--;
                      }

                  }
              }*/
        static private void ParserADDInfoCompany(MySqlConnection connection, List<int> listID, List<string> listLink)
        {
            Console.WriteLine("                        ");
            Console.WriteLine("---------ParserADDInfoCompany-------");
            Console.WriteLine("                        ");
            MySqlCommand command;
            MySqlDataReader reader;
            string sql;
            // подгружаем html страницу
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.GetEncoding("windows-1251");
            string company;
            bool postal = false;
            for (int i = 1519; i < listLink.Count; i++)
            {
                try
                {
                    company = wc.DownloadString(listLink[i]);
                    //  Console.WriteLine(company);

                    //Console.WriteLine(company);
                    string reg = "<p>.*?\"bold\">(.*?)<.*?\\s.*?\\s.*?\\sИНН.*?\"bold\">(.*?)<";
                    MatchCollection mc1 = Regex.Matches(company, reg);
                    Console.WriteLine(listLink[i]);

                    reg = "<p>.*?\"bold\">(.*?)[,.]\\s(.*?)</.*?\\s.*?\"bold\">(.*?)</span>";
                    MatchCollection mc2 = Regex.Matches(company, reg);
                    reg = "<br>Сайт:.*?\">(.*?)<";
                    MatchCollection mc3 = Regex.Matches(company, reg);
                    Console.WriteLine(mc1.Count);
                    Console.WriteLine(mc1[0].Groups[1].Value);
                    Console.WriteLine(mc1[0].Groups[2].Value);

                    Console.WriteLine(mc2.Count);
                    string p, adr;
                    if (postal == true)
                    {
                        postal = false;

                        p = mc2[0].Groups[1].Value.Remove(6);
                        adr = "г. " + mc2[0].Groups[2].Value;
                    }
                    else
                    {
                        p = mc2[0].Groups[1].Value;
                        adr = mc2[0].Groups[2].Value;
                    }
                    Console.WriteLine(p);
                    Console.WriteLine(adr);
                    Console.WriteLine(mc2[0].Groups[3].Value);


                    string website = "";
                    if (mc3.Count > 0) website = mc3[0].Groups[1].Value;
                    Console.WriteLine(mc3.Count);
                    Console.WriteLine(website);
                    Console.WriteLine("__________________");

                    reg = "/buh_otchet/(.*?)\"\\sclass=\"asbtn";
                    MatchCollection mc4 = Regex.Matches(company, reg);

                    try
                    {
                        sql = "UPDATE organization SET `Year of registration` = '" + (mc1[0].Groups[1].Value) + "', `Legal address` = '" + adr + "', `Website` = '" + website + "', `ИНН` = '" + mc1[0].Groups[2].Value + "', `Postal code`='" + p + "',`Link accounting`='https://www.audit-it.ru/buh_otchet/" + mc4[0].Groups[1].Value + "'  WHERE (`ID` = '" + listID[i] + "')";
                        command = new MySqlCommand(sql, connection);
                        // объект для чтения ответа сервера
                        reader = command.ExecuteReader();
                        reader.Close();
                    }
                    catch (Exception e)
                    {
                        if (e.Message == "Data truncated for column 'Postal code' at row 1")
                        {
                            postal = true;
                            i--;
                            continue;
                        }
                        else if (e.Message == "Specified argument was out of the range of valid values. (Parameter 'i')")
                        {
                            sql = "UPDATE organization SET `Year of registration` = '" + (mc1[0].Groups[1].Value) + "', `Legal address` = '" + adr + "', `Website` = '" + website + "', `ИНН` = '" + mc1[0].Groups[2].Value + "', `Postal code`='" + p + "',`Link accounting`='Отсутствует'  WHERE (`ID` = '" + listID[i] + "')";
                            command = new MySqlCommand(sql, connection);
                            // объект для чтения ответа сервера
                            reader = command.ExecuteReader();
                            reader.Close();
                        }
                        else i--;
                    }


                    string[] telephs = mc2[0].Groups[3].Value.Split(',');

                    for (int j = 0; j < telephs.Length; j++)
                    {
                        if (telephs.Length > 1 && j < telephs.Length - 1)
                        {
                            telephs[j + 1] = telephs[j + 1].Trim(' ');
                        }
                        sql = "INSERT INTO telephone (`Telephone`, `Organization_ID`) VALUES ('" + telephs[j] + "', '" + listID[i] + "')";
                        command = new MySqlCommand(sql, connection);
                        // объект для чтения ответа сервера
                        reader = command.ExecuteReader();
                        reader.Close();
                    }

                    Console.WriteLine("ВРЕМЯ____________________________________________________");
                    System.Threading.Thread.Sleep(1000);

                }
                catch (WebException e)
                {
                    Console.WriteLine(e);
                    System.Threading.Thread.Sleep(300000);
                    i--;
                }
            }

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
            bool gg = false;
            for (int i = 29; i < regions.Count; i++)
            {
                try
                {
                    company = wc.DownloadString(regions[i].Link);
                    // Console.WriteLine(company);

                    string id = regions[i].Link.Remove(0, 45);
                    string regCount = "&gt;</a></li><li><a href=\"/sro/list.php\\?regions=" + id + "&PAGE_NUMBER=(.*?)\"\\s title";
                    MatchCollection mcCount = Regex.Matches(company, regCount);
                    //    Console.WriteLine(mcCount.Count);
                    int count;
                    if (mcCount.Count == 0) count = 1;
                    else count = int.Parse(mcCount[0].Groups[1].Value);
                    for (int n = 1; n <= count; n++)
                    {

                        company = wc.DownloadString(regions[i].Link + "&PAGE_NUMBER=" + n);
                        //    Console.WriteLine(company);
                        string reg1 = "<a class=\"post\" href=\"(.*?)\">(.*?)<span class='grey'>(.*?)</span>";
                        string reg2 = "ААС</a></td>\\s.?(.*?)</td>";
                        MatchCollection mc = Regex.Matches(company, reg1);
                        MatchCollection mc2 = Regex.Matches(company, reg2);//огрн орнз
                        for (int j = 0; j < mc.Count; j++)
                        {
                            string title = mc[j].Groups[2].Value;
                            int indexOfChar1 = title.IndexOf("&quot;");
                            if (indexOfChar1 > -1) title = title.Replace("&quot;", "\"");
                            title = title.Remove(title.Length - 2);
                            string link = "https://www.audit-it.ru" + mc[j].Groups[1].Value;
                            Console.WriteLine(link);//link
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

                            if (gg == true)
                            {
                                sql = "INSERT INTO organization (`Title`, `ОГРН`, `ОРНЗ`, `Link`, `Type`, `Region_ID`) VALUES ('" + title + "', '" + ss[0] + "', '" + ss[1] + "', '" + link + "', '" + type + "', '" + regions[i].ID + "')";
                                command = new MySqlCommand(sql, connection);
                                // объект для чтения ответа сервера
                                reader = command.ExecuteReader();
                                reader.Close(); // закрываем reader
                            }

                            if (title == "АКФ \"МИАН\"")
                            {
                                gg = true;
                            }

                        }
                        System.Threading.Thread.Sleep(2000);
                        //     Console.WriteLine(gg);
                    }

                    Console.WriteLine("ВРЕМЯ____________________________________________________");
                    System.Threading.Thread.Sleep(1000);
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
