using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HealthAnalyzer
{
    public partial class Notification : Window
    {

        DoubleAnimation anim;
        int left;
        int top;
        DependencyProperty prop;
        int end;
        public Notification()
        {
            InitializeComponent();

            TrayPos tpos = new TrayPos();
            tpos.getXY((int)this.Width, (int)this.Height, out top, out left, out prop, out end);
            this.Top = top;
            this.Left = left;
            anim = new DoubleAnimation(end, TimeSpan.FromSeconds(1));



            //MessageBox.Show(String.Format("top: {0}, left: {1}", top, left));
        }

        private void Notification_Loaded(object sender, RoutedEventArgs e)
        {
            AnimationClock clock = anim.CreateClock();
            this.ApplyAnimationClock(prop, clock);
        }
    }
}
