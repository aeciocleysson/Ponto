using MySql.Data.MySqlClient;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace AntaresBackup
{
    public class CriarBackup
    {
        public void ExecutarBackup()
        {
            string connectionString = "Host=localhost;Database=DbErpMega;Username=root;Password=3103";

            string local = $"C:\\Antares\\Backup\\Sql\\";
            string destino = $"C:\\Antares\\Backup\\Zipado\\backup_{DateTime.Now.ToLongTimeString().Replace(":", "")}.zip";
            string dia = DateTime.Now.Day.ToString();
            string mes = DateTime.Now.Month.ToString();
            string ano = DateTime.Now.Year.ToString();
            string hora = DateTime.Now.ToLongTimeString().Replace(":", "");

            try
            {
                StreamWriter vWriter = new StreamWriter(@"C:\\Antares\\Log\\log.txt", true);
                vWriter.WriteLine("Serviço rodando: " + DateTime.Now.ToString("dddd", new CultureInfo("pt-BR")) + " " + DateTime.Now);
                vWriter.Flush();

                var nomeDoArquivo = $"{ano}-{mes}-{dia}_{hora}";

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