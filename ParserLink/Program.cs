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
            ParserAccounting(connection, listID, linkAccounting);



            // закрываем соединение с БД
            connection.Close();

            Console.WriteLine("КОНЕЦ");
            Console.ReadLine();
        }
        static private void ParserAccounting(MySqlConnection connection, List<int> listID, List<string> linkAccounting)// парсер бух учета
        {
            MySqlCommand command;
            MySqlDataReader reader;
            string sql;
            // подгружаем html страницу
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.GetEncoding("windows-1251");
            string accountingCompany;
            string[] codeAsset = { "1100", "1110", "1120", "1130", "1140", "1150", "1160", "1170", "1180", "1190", "1200", "1210", "1220", "1230", "1240", "1250", "1260", "1600" };
            string[] codePassive = { "1300", "1310", "1320", "1340", "1350", "1360", "1370", "1400", "1410", "1420", "1430", "1450", "1500", "1510", "1520", "1530", "1540", "1550", "1700" };
            string[] codeFinancialResults = { "2110", "2120", "2100", "2210", "2220", "2200", "2310", "2320", "2330", "2340", "2350", "2300", "2410", "2460", "2400", "2510", "2520", "2500", "2900", "2910" };
            int id = 0;
            for (int i = 0; i < listID.Count; i++)
            {
                try
                {
                    accountingCompany = wc.DownloadString(linkAccounting[i]);
                    // Console.WriteLine(accountingCompany);

                    string regYear = "\"date\": \"(.*?)\",";
                    MatchCollection mcYear = Regex.Matches(accountingCompany, regYear);
                    Console.WriteLine(mcYear.Count);
                    for (int j = 0; j < mcYear.Count; j++)
                    {
                        Console.WriteLine(mcYear[j].Groups[1].Value);
                    }
                    List<List<string>> asset = new List<List<string>>();
                    //актив
                    for (int n = 0; n < codeAsset.Length; n++)
                    {
                        string regAsset = "\"" + codeAsset[n] + "\":\\s(.*?),";
                        MatchCollection mcAsset = Regex.Matches(accountingCompany, regAsset);
                        Console.WriteLine(mcAsset.Count);
                        Console.WriteLine(codeAsset[n]);
                        Console.WriteLine("_______________________");
                        asset.Add(new List<string>());
                        for (int c = 0; c < mcYear.Count; c++)
                        {
                            if (mcAsset.Count == 0)
                            {
                                asset[n].Add("0");
                                Console.WriteLine(asset[n][c]);
                            }
                            else
                            {
                                switch (mcAsset[c].Groups[1].Value)
                                {
                                    case "0":
                                        asset[n].Add("0");
                                        break;
                                    case "null":
                                        asset[n].Add("0");
                                        break;
                                    default:
                                        string t = mcAsset[c].Groups[1].Value;
                                        t = t.Trim(new char[] { '"' });
                                        asset[n].Add(t);
                                        break;

                                }
                                Console.WriteLine(asset[n][c]);
                            }

                        }
                                           
                       
                        
                    }
                    Console.WriteLine("_________ПАССИВ______________");
                    List<List<string>> passive = new List<List<string>>();
                    //пассив
                    for (int n = 0; n < codePassive.Length; n++)
                    {
                        string regPassive = "\"" + codePassive[n] + "\":\\s(.*?),";
                        MatchCollection mcPassive = Regex.Matches(accountingCompany, regPassive);
                        Console.WriteLine(mcPassive.Count);
                        Console.WriteLine(codePassive[n]);
                        Console.WriteLine("_______________________");
                        passive.Add(new List<string>());
                        for (int c = 0; c < mcYear.Count; c++)
                        {
                            if (mcPassive.Count == 0)
                            {
                                passive[n].Add("0");
                                Console.WriteLine(passive[n][c]);
                            }
                            else
                            {
                                switch (mcPassive[c].Groups[1].Value)
                                {
                                    case "0":
                                        passive[n].Add("0");
                                        break;
                                    case "null":
                                        passive[n].Add("0");
                                        break;
                                    default:
                                        string t = mcPassive[c].Groups[1].Value;
                                        t = t.Trim(new char[] { '"' });
                                        passive[n].Add(t);
                                        break;

                                }
                                Console.WriteLine(passive[n][c]);
                            }



                        }
                    }
                    
                    //отчет о финансовых результатах
                    Console.WriteLine("_________ФИНАН РЕЗ______________");
                    List<List<string>> financialResults = new List<List<string>>();
                    for (int n = 0; n < codeFinancialResults.Length; n++)
                    {
                        string regFinancialResults = "\"" + codeFinancialResults[n] + "\":\\s(.*?)[,\\s]";
                        MatchCollection mcFinancialResults = Regex.Matches(accountingCompany, regFinancialResults);
                        Console.WriteLine(mcFinancialResults.Count);
                        Console.WriteLine(codeFinancialResults[n]);
                        Console.WriteLine("_______________________");                       
                        financialResults.Add(new List<string>());
                        for (int c = 0; c < mcYear.Count; c++)
                        {
                            if (mcFinancialResults.Count == 0)
                            {
                                financialResults[n].Add("0");
                                Console.WriteLine(financialResults[n][c]);
                            }
                            else
                            {
                              //  Console.WriteLine(mcFinancialResults[c].Groups[0].Value);
                                switch (mcFinancialResults[c].Groups[0].Value)
                                {
                                    case "0":
                                        financialResults[n].Add("0");
                                        break;
                                    case "null":
                                        financialResults[n].Add("0");
                                        break;
                                    default:
                                        string t = mcFinancialResults[c].Groups[1].Value;
                                        t = t.Trim(new char[] { '"' });
                                        financialResults[n].Add(t);
                                        break;

                                }
                                Console.WriteLine(financialResults[n][c]);
                            }

                        }
                    }
                    for (int c = 0; c < mcYear.Count; c++)
                    {
                        //внеоборотне активы
                        id += 1;
                        sql = "INSERT INTO `17ias-amisi_marekhinasa`.`non-current assets` (`ID`, `Intangible assets`, `Research and development results`, `Intangible search assets`, `Tangible search assets`, `Fixed assets`, `Profitable investments in tangible assets`, `Financial investments`, `Deferred tax assets`, `Other`, `Result`) VALUES ('" + id + "','" + asset[0][c] + "', '" + asset[1][c] + "', '" + asset[2][c] + "', '" + asset[3][c] + "',  '" + asset[4][c] + "', '" + asset[5][c] + "', '" + asset[6][c] + "', '" + asset[7][c] + "', '" + asset[8][c] + "', '" + asset[9][c] + "')";
                        command = new MySqlCommand(sql, connection);
                        // объект для чтения ответа сервера
                        reader = command.ExecuteReader();
                        //оборотные активы
                        sql = "INSERT INTO `17ias-amisi_marekhinasa`.`current assets` (`ID`, `Stocks`, `Value added tax on purchased valuables`, `Accounts receivable`, `Financial investments (excluding cash equivalents)`, `Cash and cash equivalents`, `Other`, `Result`) VALUES ('" + id + "','" + asset[10][c] + "', '" + asset[11][c] + "', '" + asset[12][c] + "', '" + asset[13][c] + "', '" + asset[14][c] + "', '" + asset[15][c] + "', '" + asset[16][c] + "')";
                        command = new MySqlCommand(sql, connection);
                        // объект для чтения ответа сервера
                        reader = command.ExecuteReader();
                        //капитал и резервы
                        sql = "INSERT INTO `17ias-amisi_marekhinasa`.`capital and reserves` (`ID`, `Authorized capital`, `Own shares`, `Revaluation of non-current assets`, `Additional capital (without revaluation)`, `Reserve capital`, `Retained earnings (uncovered loss)`, `Result`) VALUES ('" + id + "','" + passive[0][c] + "',  '" + passive[1][c] + "', '" + passive[2][c] + "', '" + passive[3][c] + "', '" + passive[4][c] + "', '" + passive[5][c] + "', '" + passive[6][c] + "')";
                        command = new MySqlCommand(sql, connection);
                        // объект для чтения ответа сервера
                        reader = command.ExecuteReader();
                        //долгосрочные обяз
                        sql = "INSERT INTO `17ias-amisi_marekhinasa`.`long-term liabilities` (`ID`, `Borrowed funds`, `Deferred tax liabilities`, `Estimated liabilities`, `Other`, `Result`) VALUES ('" + id + "','" + passive[7][c] + "', '" + passive[8][c] + "', '" + passive[9][c] + "', '" + passive[10][c] + "', '" + passive[11][c] + "')";
                        command = new MySqlCommand(sql, connection);
                        // объект для чтения ответа сервера
                        reader = command.ExecuteReader();
                        //краткосрочные обяз
                        sql = "INSERT INTO `17ias-amisi_marekhinasa`.`short-term liabilities` (`ID`, `Borrowed funds`, `Accounts payable`, `Deferred income`, `Estimated liabilities`, `Other`, `Result`) VALUES ('" + id + "','" + passive[12][c] + "', '" + passive[13][c] + "', '" + passive[14][c] + "', '" + passive[15][c] + "', '" + passive[16][c] + "', '" + passive[17][c] + "')";
                        command = new MySqlCommand(sql, connection);
                        // объект для чтения ответа сервера
                        reader = command.ExecuteReader();
                        reader.Close(); // закрываем reader
                    }

                   

                    System.Threading.Thread.Sleep(1000);


                        Console.WriteLine("ВРЕМЯ____________________________________________________");
                        //     System.Threading.Thread.Sleep(3000);
                    }
                catch (WebException e)
                {
                    Console.WriteLine(e);
                    System.Threading.Thread.Sleep(300000);
                    i--;
                }

            }
        }
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
