using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace RPARobot.Librarys
{
    public class ButtonThreeState:Button
    {
        public string def
        {
            get { return (string)GetValue(defProperty); }
            set { SetValue(defProperty, value); }
        }

        // Using a DependencyProperty as the backing store for src.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty defProperty =
            DependencyProperty.Register("def", typeof(string), typeof(ButtonThreeState), new PropertyMetadata(""));

        public string hover
        {
            get { return (string)GetValue(hoverProperty); }
            set { SetValue(hoverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for src.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty hoverProperty =
            DependencyProperty.Register("hover", typeof(string), typeof(ButtonThreeState), new PropertyMetadata(""));

        public string active
        {
            get { return (string)GetValue(activeProperty); }
            set { SetValue(activeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for src.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty activeProperty =
            DependencyProperty.Register("active", typeof(string), typeof(ButtonThreeState), new PropertyMetadata(""));




        public string forbid
        {
            get { return (string)GetValue(forbidProperty); }
            set { SetValue(forbidProperty, value); }
        }

        // Using a DependencyProperty as the backing store for forbid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty forbidProperty =
            DependencyProperty.Register("forbid", typeof(string), typeof(ButtonThreeState), new PropertyMetadata(""));




        public Brush def_foreground
        {
            get { return (Brush)GetValue(def_foregroundProperty); }
            set { SetValue(def_foregroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for def_foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty def_foregroundProperty =
            DependencyProperty.Register("def_foreground", typeof(Brush), typeof(ButtonThreeState), new PropertyMetadata(Brushes.Black));




        public Brush click_foreground
        {
            get { return (Brush)GetValue(click_foregroundProperty); }
            set { SetValue(click_foregroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for click_foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty click_foregroundProperty =
            DependencyProperty.Register("click_foreground", typeof(Brush), typeof(ButtonThreeState), new PropertyMetadata(Brushes.White));




        public Brush hover_foreground
        {
            get { return (Brush)GetValue(hover_foregroundProperty); }
            set { SetValue(hover_foregroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for hover_foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty hover_foregroundProperty =
            DependencyProperty.Register("hover_foreground", typeof(Brush), typeof(ButtonThreeState), new PropertyMetadata(Brushes.White));




        public Brush forbid_foreground
        {
            get { return (Brush)GetValue(forbid_foregroundProperty); }
            set { SetValue(forbid_foregroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for forbid_foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty forbid_foregroundProperty =
            DependencyProperty.Register("forbid_foreground", typeof(Brush), typeof(ButtonThreeState), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB4B4B4"))));

        
        
        
        
    }


    public class ColorButtonThreeState : Button
    {


        public string def_img
        {
            get { return (string)GetValue(def_imgProperty); }
            set { SetValue(def_imgProperty, value); }
        }

        // Using a DependencyProperty as the backing store for def_img.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty def_imgProperty =
            DependencyProperty.Register("def_img", typeof(string), typeof(ColorButtonThreeState), new PropertyMetadata(""));





        public Brush def
        {
            get { return (Brush)GetValue(defProperty); }
            set { SetValue(defProperty, value); }
        }

        // Using a DependencyProperty as the backing store for src.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty defProperty =
            DependencyProperty.Register("def", typeof(Brush), typeof(ColorButtonThreeState), new PropertyMetadata(Brushes.Transparent));

        public Brush hover
        {
            get { return (Brush)GetValue(hoverProperty); }
            set { SetValue(hoverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for src.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty hoverProperty =
            DependencyProperty.Register("hover", typeof(Brush), typeof(ColorButtonThreeState), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d6d6d6"))));

        public Brush active
        {
            get { return (Brush)GetValue(activeProperty); }
            set { SetValue(activeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for src.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty activeProperty =
            DependencyProperty.Register("active", typeof(Brush), typeof(ColorButtonThreeState), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#afafaf"))));




        public Brush forbid
        {
            get { return (Brush)GetValue(forbidProperty); }
            set { SetValue(forbidProperty, value); }
        }

        // Using a DependencyProperty as the backing store for forbid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty forbidProperty =
            DependencyProperty.Register("forbid", typeof(Brush), typeof(ColorButtonThreeState), new PropertyMetadata(Brushes.Black));




        public Brush def_foreground
        {
            get { return (Brush)GetValue(def_foregroundProperty); }
            set { SetValue(def_foregroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for def_foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty def_foregroundProperty =
            DependencyProperty.Register("def_foreground", typeof(Brush), typeof(ColorButtonThreeState), new PropertyMetadata(Brushes.Black));




        public Brush click_foreground
        {
            get { return (Brush)GetValue(click_foregroundProperty); }
            set { SetValue(click_foregroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for click_foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty click_foregroundProperty =
            DependencyProperty.Register("click_foreground", typeof(Brush), typeof(ColorButtonThreeState), new PropertyMetadata(Brushes.Black));




        public Brush hover_foreground
        {
            get { return (Brush)GetValue(hover_foregroundProperty); }
            set { SetValue(hover_foregroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for hover_foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty hover_foregroundProperty =
            DependencyProperty.Register("hover_foreground", typeof(Brush), typeof(ColorButtonThreeState), new PropertyMetadata(Brushes.Black));




        public Brush forbid_foreground
        {
            get { return (Brush)GetValue(forbid_foregroundProperty); }
            set { SetValue(forbid_foregroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for forbid_foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty forbid_foregroundProperty =
            DependencyProperty.Register("forbid_foreground", typeof(Brush), typeof(ColorButtonThreeState), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB4B4B4"))));


    }



}
