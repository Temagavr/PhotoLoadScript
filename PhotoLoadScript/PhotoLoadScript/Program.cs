using System;
using System.IO;
using System.Text.Json;
using Npgsql;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

namespace PhotoLoadScript
{
  class Program
  {
    private const string _datePush = "01-01-2177";
    public static Settings ReadJson(ref string path)
    {
      using (StreamReader file = new StreamReader(path))
      {
        try
        {

          string json = file.ReadToEnd();

          return JsonSerializer.Deserialize<Settings>(json);

        }
        catch (Exception)
        {
          Console.WriteLine("Problem reading file");

          return null;
        }
      }
    }

    public static void InsertPhoto(ref NpgsqlConnection conn, ref Settings settings, ref string url, ref DateTime dateTaken)
    {
      try
      {
        using (var command = new NpgsqlCommand($"INSERT INTO {settings.tableName} (path, id_tag_list, date, date_graduate) VALUES (@path, @id_tag_list, @date, @date_graduate)", conn))
        {
          command.Parameters.AddWithValue("path", url);
          command.Parameters.AddWithValue("id_tag_list", Convert.ToInt32(settings.id_tag_list));
          command.Parameters.AddWithValue("date", dateTaken);
          command.Parameters.AddWithValue("date_graduate", Convert.ToInt32(settings.year));

          int nRows = command.ExecuteNonQuery();
          Console.Out.WriteLine(String.Format("Number of rows inserted={0}", nRows));
        }
      }
      catch (Exception e)
      {
        Console.WriteLine("Error: " + e);
        Console.WriteLine(e.StackTrace);
      }
    }

    public static DateTime GetDateTaken(ref string photoUrl)
    {
      try
      {
        Image myImg = Image.FromFile(photoUrl);
        PropertyItem propItem = myImg.GetPropertyItem(36867);
        DateTime dtaken;
        //Convert date taken metadata to a DateTime object

        string sdate = Encoding.UTF8.GetString(propItem.Value).Trim();
        string secondhalf = sdate.Substring(sdate.IndexOf(" "), (sdate.Length - sdate.IndexOf(" ")));
        string firsthalf = sdate.Substring(0, 10);
        firsthalf = firsthalf.Replace(":", "-");
        sdate = firsthalf + secondhalf;
        dtaken = DateTime.Parse(sdate);

        Console.WriteLine("Date taken: " + dtaken);
        return dtaken;
      }
      catch (Exception)
      {
        //Console.WriteLine("Error: " + e);
        return Convert.ToDateTime(_datePush);
      }
    }

    static void Main(string[] args)
    {
      Settings photosSettings = ReadJson(ref args[0]);

      if (photosSettings != null)
      {
        Console.WriteLine(photosSettings.tableName + "  -  " + photosSettings.year + "  -  " + photosSettings.id_tag_list);

        Console.WriteLine("Params readed success!");

        try
        {
          var conn = DBUtils.GetDBConnection();
          conn.Open();

          Console.WriteLine("Success connection to db!");

          string[] filePhotos = Directory.GetFiles(args[1]);
          foreach (string photo in filePhotos)
          {
            string photoUrl = photo;

            DateTime dateTaken = GetDateTaken(ref photoUrl);
            if (dateTaken == Convert.ToDateTime(_datePush))
              dateTaken = Convert.ToDateTime("01-01-" + photosSettings.year);

            //InsertPhoto(ref conn, ref photosSettings, ref photoUrl, ref dateTaken);
          }

          conn.Close();
          conn.Dispose();
          conn = null;
        }
        catch (Exception e)
        {
          Console.WriteLine("Problem with connection!");
          Console.WriteLine("Error: " + e);
        }
      }
    }
  }
}
