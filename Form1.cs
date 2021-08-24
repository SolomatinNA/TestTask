using System;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Globalization;

namespace WindowsFormsApplication1
{
    struct Weath
    {
        public double temp_min;
        public double temp_max;
        public int pressure;
        public string data;
        public int time;
    };

    struct DailyWeath
    {
        public double temp_night;
        public double temp_morning;
        public int max_pressure;
        public string data;
        public double TempDiff;
    };

    public partial class TestTask3a : Form
    {
        public TestTask3a()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            WebRequest request = WebRequest.Create("https://api.openweathermap.org/data/2.5/forecast?q=Pechora&APPID=2f2952cab73b7bae9464246406488174&units=metric");
            WebResponse response = await request.GetResponseAsync();

            string answer;
            using (Stream s = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    answer = await reader.ReadToEndAsync();
                }
            }

            response.Close();

            Weath[] MyWeath;
            MyWeath = new Weath[40];

            int[] index;
            index = new int[4];
            for (int i = 0; i < 4; i++) { index[i] = -1; }

            for (int i = 0; i < 40; i++)
            {
                index[0] = answer.IndexOf("temp_min", index[0] + 1);
                MyWeath[i].temp_min = Convert.ToDouble(answer.Substring(index[0] + 10, answer.IndexOf(",", index[0] + 1) - index[0] - 10), CultureInfo.GetCultureInfo("en-US"));

                index[1] = answer.IndexOf("temp_max", index[1] + 1);
                MyWeath[i].temp_max = Convert.ToDouble(answer.Substring(index[1] + 10, answer.IndexOf(",", index[1] + 1) - index[1] - 10), CultureInfo.GetCultureInfo("en-US"));

                index[2] = answer.IndexOf("pressure", index[2] + 1);
                MyWeath[i].pressure = int.Parse(answer.Substring(index[2] + 10, 4));

                index[3] = answer.IndexOf("dt_txt", index[3] + 1);
                MyWeath[i].data = answer.Substring(index[3] + 9, 10);
                MyWeath[i].time = int.Parse(answer.Substring(index[3] + 20, 2));
            }

            DailyWeath[] DailyWeath;
            DailyWeath = new DailyWeath[4];

            int j = 0;
            while (true)
            {
                if (MyWeath[j].time > 0) j++;
                else break;
            }

            for (int i = 0; i < 4; i++)
            {
                DailyWeath[i].temp_night = Math.Round((MyWeath[j].temp_max + MyWeath[j + 1].temp_max) / 2, 2);
                DailyWeath[i].temp_morning = Math.Round((MyWeath[j + 2].temp_max + MyWeath[j + 3].temp_max) / 2, 2);
                for (int k = 0; k < 8; k++)
                {
                    if ((k == 0) || (DailyWeath[i].max_pressure < MyWeath[j + k].pressure)) DailyWeath[i].max_pressure = MyWeath[j + k].pressure;
                }
                DailyWeath[i].data = MyWeath[j].data;
                DailyWeath[i].TempDiff = Math.Round(Math.Abs(DailyWeath[i].temp_morning - DailyWeath[i].temp_night), 2);

                j = j + 8;
            }

            int MaxPressure = 0;
            double MinTempDiff = 0;
            string DayOfMinTempDeff = string.Empty;

            for (int i = 0; i < 4; i++)
            {
                if (MaxPressure < DailyWeath[i].max_pressure) MaxPressure = DailyWeath[i].max_pressure;
                if ((i == 0) || (MinTempDiff > DailyWeath[i].TempDiff))
                {
                    DayOfMinTempDeff = DailyWeath[i].data;
                    MinTempDiff = DailyWeath[i].TempDiff;
                }
            }

            TextBox.Text = "\n1. Максимальное давление за предстоящие 4 дня (начиная с завтрашнего): " + MaxPressure.ToString() + "hPa\n";
            TextBox.Text += "2. День с минимальной разницей между ночной и утренней температурами: " + DayOfMinTempDeff;

        }
    }
}
