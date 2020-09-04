using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Double = System.Double;

namespace HealthAnalyzer
{
    public partial class MainWindow : Window
    {
        private int nDaysCount = 25;
        private string sStartingDate = DateTime.Now.Day.ToString("D2") + "." + DateTime.Now.Month.ToString("D2") + "." + DateTime.Now.Year.ToString("D4");
        private string WCFParams = @".\HealthAnalyzer.wcf";

        private double[] dWeights;

        private ListBoxItem[] iItemsD;
        private TextBox[] iItemsW;
        private TextBox[] tEats;

        private int nHoursBeforeEat = 2;
        private int nEatPeriod = 3;

        private readonly BackgroundWorker worker = new BackgroundWorker();
        
        public MainWindow()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            ReadParams();

            InitData();

            FillData();


            /*Thread t = new Thread(ThreadStart);
            t.SetApartmentState(ApartmentState.STA);

            t.Start();*/

        }


        private void ThreadStart()
        {

            Thread.Sleep(3000);

            this.Dispatcher.Invoke(() =>
            {
                Notification cNotification = new Notification();

                cNotification.Show();
            });
        }



        private void InitData()
        {
            tEats = new TextBox[5];

            tEats[0] = textBoxEat1;
            tEats[1] = textBoxEat2;
            tEats[2] = textBoxEat3;
            tEats[3] = textBoxEat4;
            tEats[4] = textBoxEat5;

            for (int i = 0; i < 5; i++)
            {
                tEats[i].Text = "";
            }
        }

        private void FillData()
        {
            textBoxnDaysCount.Text = nDaysCount.ToString();

            string localDate = sStartingDate;

            iItemsD = new ListBoxItem[nDaysCount];
            iItemsW = new TextBox[nDaysCount];

            for (int i = 0; i < nDaysCount; i++)
            {
                iItemsD[i] = new ListBoxItem();
                iItemsD[i].Content = localDate;

                iItemsD[i].Name = "D" + i.ToString();

                localDate = NextDay(localDate);

                iItemsD[i].Height = 25;

                listBoxD.Items.Add(iItemsD[i]);

                iItemsW[i] = new TextBox();

                iItemsW[i].Name = "W" + i.ToString();

                iItemsW[i].Text = dWeights[i] > 0 ? dWeights[i].ToString() : "";

                iItemsW[i].Width = 68;
                iItemsW[i].Height = 21;

                iItemsW[i].LostFocus += ListWeightElem_LostFocus;

                listBoxW.Items.Add(iItemsW[i]);
            }

            textBoxHoursBeforeEat.Text = nHoursBeforeEat.ToString();
            textBoxHoursEatPeriod.Text = nEatPeriod.ToString();
        }

        

        private void ReadParams()
        {
            if (File.Exists(WCFParams))
            {
                IniFile Ini = new IniFile(WCFParams);

                nDaysCount = Int32.Parse(Ini.Read("all", "nDaysCount"));

                sStartingDate = Ini.Read("all", "sStartingDate");

                nHoursBeforeEat = Int32.Parse(Ini.Read("all", "nHoursBeforeEat"));

                nEatPeriod = Int32.Parse(Ini.Read("all", "nEatPeriod"));

                dWeights = new Double[nDaysCount];

                string localDate = sStartingDate;

                for (int i = 0; i < nDaysCount; i++)
                {

                    //dWeights[i] = Double.Parse(Ini.Read("all", localDate));

                    Double.TryParse(Ini.Read("all", localDate), out dWeights[i]);

                    localDate = NextDay(localDate);
                }
            }
            else
            {
                var myFile = File.Create(WCFParams);

                myFile.Close();


                IniFile Ini = new IniFile(WCFParams);

                Ini.Write("all", "nDaysCount", nDaysCount.ToString());
                Ini.Write("all", "sStartingDate", sStartingDate);
                Ini.Write("all", "nHoursBeforeEat", nHoursBeforeEat.ToString());
                Ini.Write("all", "nEatPeriod", nEatPeriod.ToString());

                dWeights = new Double [nDaysCount];

                dWeights[0] = 0;

                for (int i = 1; i < nDaysCount; i++)
                {
                    dWeights[i] = 0;
                }

                string localDate = sStartingDate;

                for (int i = 0; i < nDaysCount; i++)
                {
                    Ini.Write("all", localDate, dWeights[i].ToString());

                    localDate = NextDay(localDate);
                }

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (File.Exists(WCFParams))
            {
                IniFile Ini = new IniFile(WCFParams);

                Ini.Write("all", "nDaysCount", nDaysCount.ToString());
                
                string localDate = sStartingDate;

                for (int i = 0; i < nDaysCount; i++)
                {
                    Ini.Write("all", localDate, dWeights[i].ToString());

                    localDate = NextDay(localDate);
                }
            }
        }

        private void ListWeightElem_LostFocus(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;

            if (ctrl != null)
            {
                // Get the control name
                string sName = ctrl.Name;

                int iIndex = Int32.Parse(sName.Substring(1));

                Double.TryParse(iItemsW[iIndex].Text, out dWeights[iIndex]);

                if (dWeights[iIndex] == 0)
                {
                    iItemsW[iIndex].Text = "";
                }

            }
        }

        private void ReSetDays()
        {
            int nDaysCountOld = nDaysCount;

            double[] dWeightsOld = new double[nDaysCountOld];

            for (int i = 0; i < nDaysCountOld; i++)
            {
                dWeightsOld[i] = dWeights[i];
            }

            nDaysCount = Int32.Parse(textBoxnDaysCount.Text);

            
            dWeights = new double[nDaysCount];

            for (int i = 0; i < Math.Min(nDaysCount, nDaysCountOld); i++)
            {
                dWeights[i] = dWeightsOld[i];
            }

            for (int i = Math.Min(nDaysCount, nDaysCountOld); i < nDaysCount; i++)
            {
                dWeights[i] = 0;
            }

            iItemsD = new ListBoxItem[nDaysCount];
            iItemsW = new TextBox[nDaysCount];


            listBoxD.Items.Clear();
            listBoxW.Items.Clear();

            string localDate = sStartingDate;

            for (int i = 0; i < nDaysCount; i++)
            {
                iItemsD[i] = new ListBoxItem();
                iItemsD[i].Content = localDate;

                iItemsD[i].Name = "D" + i.ToString();

                localDate = NextDay(localDate);

                iItemsD[i].Height = 25;

                listBoxD.Items.Add(iItemsD[i]);

                iItemsW[i] = new TextBox();

                iItemsW[i].Name = "W" + i.ToString();

                iItemsW[i].Text = dWeights[i] > 0 ? dWeights[i].ToString() : "";

                iItemsW[i].Width = 68;
                iItemsW[i].Height = 21;

                iItemsW[i].LostFocus += ListWeightElem_LostFocus;

                listBoxW.Items.Add(iItemsW[i]);;
            }

        }

        private void textBoxnDaysCount_LostFocus(object sender, RoutedEventArgs e)
        {
            ReSetDays();
        }

        private void textBoxnDaysCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ReSetDays();
            }
        }

        private string NextDay(string sDate)
        {
            DateTime dDate = StringToDateTime(sDate);

            return DateTimeToString(dDate.AddDays(1));
        }

        private DateTime StringToDateTime(string str)
        {
            string[] sParts = str.Split('.');

            return new DateTime(Int32.Parse(sParts[2]), Int32.Parse(sParts[1]), Int32.Parse(sParts[0]));
        }

        private string DateTimeToString(DateTime dDate)
        {
            return dDate.Day.ToString("D2") + "." + dDate.Month.ToString("D2") + "." + dDate.Year.ToString("D4");
        }

        public Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null) return null;
            if (element.GetType() == type) return element;
            Visual foundElement = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                    break;
            }
            return foundElement;
        }
        private void lbx1_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer _listboxScrollViewer1 = GetDescendantByType(listBoxD, typeof(ScrollViewer)) as ScrollViewer;
            ScrollViewer _listboxScrollViewer2 = GetDescendantByType(listBoxW, typeof(ScrollViewer)) as ScrollViewer;
            _listboxScrollViewer2.ScrollToVerticalOffset(_listboxScrollViewer1.VerticalOffset);
        }

        private void lbx2_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer _listboxScrollViewer1 = GetDescendantByType(listBoxD, typeof(ScrollViewer)) as ScrollViewer;
            ScrollViewer _listboxScrollViewer2 = GetDescendantByType(listBoxW, typeof(ScrollViewer)) as ScrollViewer;
            _listboxScrollViewer1.ScrollToVerticalOffset(_listboxScrollViewer2.VerticalOffset);
        }

        private bool ParseTime(string str, out int Hour, out int Minute)
        {
            Hour = 0;
            Minute = 0;

            bool ret = true;

            if (str.Length != 5)
                return false;


            if (str[2] != ':')
            {
                return false;
            }

            if (!Int32.TryParse(str.Substring(0, 2), out Hour) || !Int32.TryParse(str.Substring(3, 2), out Minute))
                return false;

            if (!(Hour >= 0 && Hour <= 23))
                return false;

            if (!(Minute >= 0 && Minute <= 59))
                return false;

            return true;

        }

        private void SetEatTime(object sender)
        {
            Control ctrl = sender as Control;

            if (ctrl != null)
            {
                // Get the control name
                string sName = ctrl.Name;

                int iIndex = Int32.Parse(sName.Substring(10)) - 1;

                string sInputText = tEats[iIndex].Text;

                bool bValidStr = ParseTime(sInputText, out int Hour, out int Minute);

                if (bValidStr == false)
                {

                    for (int i = iIndex; i < 5; i++)
                    {
                        tEats[i].Text = "";
                    }
                }
                else
                {
                    DateTime dDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Hour, Minute, 0);

                    for (int i = iIndex + 1; i < 5; i++)
                    {
                        dDate = dDate.AddHours(nEatPeriod);

                        tEats[i].Text = dDate.Hour.ToString("D2") + ":" + dDate.Minute.ToString("D2");

                    }


                    CheckEatTimeValidation();

                }


            }
        }

        private void CheckEatTimeValidation()
        {
            if (textBoxSet.Text != "")
            {

                int Year = DateTime.Now.Year;
                int Month = DateTime.Now.Month;
                int Day = DateTime.Now.Day;

                int prevHour = 0;

                if (ParseTime(textBoxRise.Text, out int HourRise, out int MinuteRise) == false)
                {
                    return;
                }

                if (ParseTime(textBoxSet.Text, out int HourSet, out int MinuteSet) == false)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        tEats[i].TextDecorations = null;
                    }

                    return;
                }

                DateTime SetTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, HourSet, MinuteSet, 0);

                if (HourSet < HourRise)
                {
                    SetTime = SetTime.AddDays(1);
                }

                for (int i = 0; i < 5; i++)
                {


                    if (ParseTime(tEats[i].Text, out int HourEat, out int MinuteEat) == false)
                    {
                        return;
                    }

                    DateTime EatTime = new DateTime(Year, Month, Day, HourEat, MinuteEat, 0);

                    if (i > 0)
                    {
                        if (HourEat < prevHour)
                        {
                            DateTime fakeTime = new DateTime(Year, Month, Day, HourEat, MinuteEat, 0);

                            fakeTime = fakeTime.AddDays(1);

                            Year = fakeTime.Year;
                            Month = fakeTime.Month;
                            Day = fakeTime.Day;

                            EatTime = new DateTime(Year, Month, Day, HourEat, MinuteEat, 0);
                        }
                    }

                    prevHour = HourEat;

                    DateTime EatTimeNext = EatTime.AddHours(nHoursBeforeEat);



                    if (EatTimeNext > SetTime)
                    {
                        tEats[i].TextDecorations = TextDecorations.Strikethrough;
                    }
                    else
                    {
                        tEats[i].TextDecorations = null;
                    }

                }
            }
        }

        private void textBoxEat1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetEatTime(sender);
            }
        }

        private void textBoxEat1_LostFocus(object sender, RoutedEventArgs e)
        {

            SetEatTime(sender);
            
        }

        private void SetRise(string sRise)
        {
            bool bValidStr = ParseTime(sRise, out int Hour, out int Minute);

            if (bValidStr == false)
            {
                textBoxRise.Text = "";
                textBoxSet.Text = "";
            }
            else
            {
                DateTime dDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Hour, Minute, 0);

                dDate = dDate.AddHours(24 - 8);

                textBoxSet.Text = dDate.Hour.ToString("D2") + ":" + dDate.Minute.ToString("D2");

                CheckEatTimeValidation();
            }
        }


        private void textBoxRise_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetRise(textBoxRise.Text);
            }
        }

        private void textBoxRise_LostFocus(object sender, RoutedEventArgs e)
        {
            SetRise(textBoxRise.Text);
        }

        private void SetSet(string sRise)
        {
            bool bValidStr = ParseTime(sRise, out int Hour, out int Minute);

            if (bValidStr == false)
            {
                textBoxSet.Text = "";
            }
            else
            {
                CheckEatTimeValidation();
            }
        }

        private void textBoxSet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetSet(textBoxSet.Text);
            }
        }

        private void textBoxSet_LostFocus(object sender, RoutedEventArgs e)
        {
            SetSet(textBoxSet.Text);
        }


        private void textBoxHoursBeforeEat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Int32.TryParse(textBoxHoursBeforeEat.Text, out nHoursBeforeEat) == false)
                {
                    textBoxHoursBeforeEat.Text = "0";
                }
                else
                {
                    CheckEatTimeValidation();
                }
            }
        }

        private void textBoxHoursBeforeEat_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(textBoxHoursBeforeEat.Text, out nHoursBeforeEat) == false)
            {
                textBoxHoursBeforeEat.Text = "0";
            }
            else
            {
                CheckEatTimeValidation();
            }
        }

        private void textBoxHoursEatPeriod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Int32.TryParse(textBoxHoursEatPeriod.Text, out nEatPeriod) == false)
                {
                    textBoxHoursEatPeriod.Text = "0";
                }
            }
        }

        private void textBoxHoursEatPeriod_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(textBoxHoursEatPeriod.Text, out nEatPeriod) == false)
            {
                textBoxHoursEatPeriod.Text = "0";
            }

        }
    }
}
