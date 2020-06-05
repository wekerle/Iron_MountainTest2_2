using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;

namespace Test2_2
{
    class Program
    {
        static void Main(string[] args)
        {
            string zipPath = ConfigurationManager.AppSettings["ZipPath"];
            string extractPath = ConfigurationManager.AppSettings["ExtractPath"];
            Directory.Delete(extractPath, true);

            ZipFile.ExtractToDirectory(zipPath, extractPath);

            List<MetaInfo> metaInfos = GetMetaInfos(extractPath);
            AddMetaInfosToDatabase(metaInfos);
        }

        private static List<MetaInfo> GetMetaInfos(string extractPath) {
            string line;
            int counter = 0;
            List<MetaInfo> metainfos = new List<MetaInfo>();

            StreamReader file = new StreamReader($@"{extractPath}\info.meta");
            string metaInfoDelimiter = ConfigurationManager.AppSettings["MetaInfoDelimiter"];
            while ((line = file.ReadLine()) != null)
            {
                if (counter >= 1)
                {
                    var lineSplit = line.Split(metaInfoDelimiter.ToCharArray()[0]);
                    metainfos.Add(new MetaInfo
                    {
                        Id = Int32.Parse(lineSplit[0]),
                        CreationDate = DateTime.Parse(lineSplit[1]),
                        ImageRoute = lineSplit[2],
                        ImageExists = File.Exists($@"{extractPath}\{lineSplit[2]}")
                    });
                }
                counter++;
            }

            file.Close();

            return metainfos;
        }

        private static void AddMetaInfosToDatabase(List<MetaInfo> metaInfos) {
            using (var conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                DataTable dt = new DataTable();
 
                dt.Columns.Add(new DataColumn("Id", typeof(int)));
                dt.Columns.Add(new DataColumn("CreationDate", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("ImageRoute", typeof(string)));
                dt.Columns.Add(new DataColumn("ImageExists", typeof(int)));

                foreach (var item in metaInfos) {
                    dt.Rows.Add(item.Id, item.CreationDate, item.ImageRoute, item.ImageExists);
                }

                SqlCommand sqlcom = new SqlCommand("spInsertMetainfo", conn);
                sqlcom.CommandType = CommandType.StoredProcedure;
                sqlcom.Parameters.AddWithValue("@tableMetaInfos", dt);
                conn.Open();
                sqlcom.ExecuteNonQuery();
            }
        }
    }
}
