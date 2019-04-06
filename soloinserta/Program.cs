using System;
using System.Configuration;
using System.Text;
using System.IO;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string ctl, connStr = "";
            string serv, port, usua, cont, data = "";         // local
            string rser, rusu, rcon, rdat = "";               // remoto
            // ********************************************** VARIABLES ***************************
            string dondeva = "2";       // "1"=local, "2"=remoto
            string sNombre = "e:/Solorsoft_DNI_RUC/padron_reducido_ruc_24012019.txt";
            string tabla = "sunat";
            // ************************************************************************************
            if (dondeva == "1")
            {
                // conexión local
                serv = ConfigurationManager.AppSettings["serv"].ToString();
                port = ConfigurationManager.AppSettings["port"].ToString();
                usua = ConfigurationManager.AppSettings["user"].ToString();
                cont = ConfigurationManager.AppSettings["pass"].ToString();
                data = ConfigurationManager.AppSettings["data"].ToString();
                ctl = ConfigurationManager.AppSettings["ConnectionLifeTime"].ToString();
                connStr = "server=" + serv + ";port=" + port + ";uid=" + usua + ";pwd=" + cont + ";database=" + data +
                    ";ConnectionLifeTime=" + ctl + ";";
            }else
            {
                // remoto
                rser = ConfigurationManager.AppSettings["srvclt"].ToString();
                rusu = ConfigurationManager.AppSettings["usrclt"].ToString();
                rcon = ConfigurationManager.AppSettings["pswclt"].ToString();
                rdat = ConfigurationManager.AppSettings["datclt"].ToString();
                port = ConfigurationManager.AppSettings["port"].ToString();
                ctl = ConfigurationManager.AppSettings["ConnectionLifeTime"].ToString();
                connStr = "server=" + rser + ";port=" + port + ";uid=" + rusu + ";pwd=" + rcon + ";database=" + rdat +
                    ";ConnectionLifeTime=" + ctl + ";";
            }
            int cta = 0;
            int num = 0;
            int iniciof = 0;  // 7529999
            string[] strArray;
            StringBuilder sCommand = new StringBuilder("INSERT INTO " + tabla + " (ruc,rsocial,estado,domicilio,ubigeo,tipovia,nombrevia,zona,tipozona,numero,interior,lote,ndpto,manzana,kilomet,marca,dia) VALUES ");
            using (MySqlConnection mConnection = new MySqlConnection(connStr))
            {
                mConnection.Open();
                List<string> Rows = new List<string>();
                foreach (string line in File.ReadLines(sNombre, Encoding.GetEncoding("iso-8859-1")))
                {
                    strArray = line.Split('|');
                    // 0 RUC|
                    // 1 NOMBRE O RAZ�N SOCIAL|
                    // 2 ESTADO DEL CONTRIBUYENTE|      cambia
                    // 3 CONDICI�N DE DOMICILIO|        cambia
                    // 4 UBIGEO|                        cambia
                    // 5 TIPO DE V�A|
                    // 6 NOMBRE DE V�A|
                    // 7 C�DIGO DE ZONA|
                    // 8 TIPO DE ZONA|
                    // 9 N�MERO|
                    // 10 INTERIOR|
                    // 11 LOTE|
                    // 12 DEPARTAMENTO|
                    // 13 MANZANA|
                    // 14 KIL�METRO|
                    num = num + 1;
                    if(num > iniciof)
                    {
	                    cta = cta + 1;
                        Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}')",
                        MySqlHelper.EscapeString(strArray[0]), MySqlHelper.EscapeString(strArray[1]),
                        MySqlHelper.EscapeString(strArray[2]), MySqlHelper.EscapeString(strArray[3]),
                        MySqlHelper.EscapeString(strArray[4]), MySqlHelper.EscapeString(strArray[5]),
                        MySqlHelper.EscapeString(strArray[6]), MySqlHelper.EscapeString(strArray[7]),
                        MySqlHelper.EscapeString(strArray[8]), MySqlHelper.EscapeString(strArray[9]),
                        MySqlHelper.EscapeString(strArray[10]), MySqlHelper.EscapeString(strArray[11]),
                        MySqlHelper.EscapeString(strArray[12]), MySqlHelper.EscapeString(strArray[13]),
                        MySqlHelper.EscapeString(strArray[14]), MySqlHelper.EscapeString(""),
                        MySqlHelper.EscapeString(DateTime.Now.ToString())));
                        if (cta == 20000)
                        {
                            sCommand.Append(string.Join(",", Rows));
                            sCommand.Append(";");
                            using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), mConnection))
                            {
				                myCmd.CommandTimeout = 300;
                                myCmd.CommandType = CommandType.Text;
                                myCmd.ExecuteNonQuery();
                            }
                            cta = 0;
                            sCommand.Clear();
                            sCommand = new StringBuilder("INSERT INTO " + tabla + " (ruc,rsocial,estado,domicilio,ubigeo,tipovia,nombrevia,zona,tipozona,numero,interior,lote,ndpto,manzana,kilomet,marca,dia) VALUES ");
                            Rows.Clear();
                        }
                    }
                }
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");
                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), mConnection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }
        }
    }
}
