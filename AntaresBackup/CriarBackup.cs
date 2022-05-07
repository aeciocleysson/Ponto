using MySql.Data.MySqlClient;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace AntaresBackup
{
    public class CriarBackup
    {
        public static void ExecutarBackup()
        {
            //string connectionString = "Host=192.168.1.200;Database=dbmega;Username=mega;Password=mega@3212";
            string connectionString = "Host=127.0.0.1;Database=dbmega;Username=mega;Password=mega@3212";
            //string connectionString = "Host=127.0.0.1;Database=dberpmega;Username=root;Password=3103";

            string local = $"C:\\Antares\\Backup\\Sql\\";
            string dia = DateTime.Now.Day.ToString();
            string mes = DateTime.Now.Month.ToString();
            string ano = DateTime.Now.Year.ToString();
            string hora = DateTime.Now.ToLongTimeString().Replace(":", "");

            try
            {
                StreamWriter vWriter = new StreamWriter(@"C:\\Antares\\Log\\log.txt", true);
                vWriter.WriteLine("Serviço rodando: " + DateTime.Now.ToString("dddd", new CultureInfo("pt-BR")) + " " + DateTime.Now);
                vWriter.Flush();

                var nomeDoArquivo = $"{ano}_{mes}_{dia}_{hora}";
                var destino = $"C:\\Antares\\Backup\\Zipado\\Antares Mega\\dumpMega_{nomeDoArquivo}.zip";
                //var destino = $"C:\\Antares\\Backup\\Zipado\\Antares Rb\\dumpRb_{nomeDoArquivo}.zip";

                var arquivo = local + "\\" + nomeDoArquivo + ".sql";
                MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();

                MySqlBackup mb = new MySqlBackup(cmd);

                cmd.Connection = conn;
                mb.ExportToFile(arquivo);
                conn.Close();

                vWriter.WriteLine("Backup criado: " + DateTime.Now);
                vWriter.WriteLine("---------------------------------------------------------");
                vWriter.Flush();
                vWriter.Close();

                ZipFile.CreateFromDirectory(local, destino);
                File.SetAttributes(arquivo, FileAttributes.Normal);
                File.Delete(arquivo);
            }
            catch (Exception ex)
            {
                StreamWriter vWriter = new StreamWriter(@"C:\\Antares\\Log\\logErro.txt", true);
                vWriter.WriteLine(ex);
                vWriter.WriteLine(DateTime.Now);
                vWriter.WriteLine("---------------------------------------------------------");
                vWriter.Flush();
                vWriter.Close();
            }
        }
    }
}