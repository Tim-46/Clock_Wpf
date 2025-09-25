using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Forms;

namespace Clock_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        #region Variables

        private UIStyleController uiStyleHandler;
        private System.Windows.Controls.ContextMenu cm;
        private System.Windows.Controls.MenuItem fruitReminder;
        private static bool fruitClicked = false;
        private bool isFruitReminderChecked;
        private int oldLocationX;
        private int primaryScreenX;
        private string size;

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]

        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;

        #endregion

        public MainWindow()
        {
            CheckIfApplicationRunning();
            InitializeComponent();
            InitializeContextMenu();
        }

        #region Application Startup

        public void CheckIfApplicationRunning()
        {
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length > 1)
            {
                System.Windows.MessageBox.Show("Application is already running!");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            uiStyleHandler = new UIStyleController();
            uiStyleHandler.RestoreStyleAndPosition(GetAllMainWinLabels(), GetAllMainWinRectangles(), this);

            SetSize();

            isFruitReminderChecked = GetFruitReminderStatus();

            SetDividers(uiStyleHandler.getDividerStatus());


            if (isFruitReminderChecked)
                fruitReminder.IsChecked = true;
            else
                fruitReminder.IsChecked = false;

            primaryScreenX = uiStyleHandler.GetPrimaryScreenX();
            oldLocationX = (int)this.Left;

            DispatcherTimerSample();
            NotifyIcon();
            fruit_white.Visibility = Visibility.Hidden;
            fruit_black.Visibility = Visibility.Hidden;

            //hide from alt+tab
            var helper = new WindowInteropHelper(this).Handle;
            SetWindowLong(helper, GWL_EX_STYLE, (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
        }

        public List<System.Windows.Controls.Label> GetAllMainWinLabels()
        {
            List<System.Windows.Controls.Label> tempList = new List<System.Windows.Controls.Label>();

            foreach (UIElement label in gridMain.Children)
            {
                System.Windows.Controls.Label temp = label as System.Windows.Controls.Label;
                if (temp != null)
                    tempList.Add((System.Windows.Controls.Label)label);
            }
            return tempList;
        }

        public List<System.Windows.Shapes.Rectangle> GetAllMainWinRectangles()
        {
            List<System.Windows.Shapes.Rectangle> tempList = new List<System.Windows.Shapes.Rectangle>();

            foreach (UIElement rectangle in gridMain.Children)
            {
                System.Windows.Shapes.Rectangle temp = rectangle as System.Windows.Shapes.Rectangle;
                if (temp != null)
                    tempList.Add((System.Windows.Shapes.Rectangle)rectangle);
            }
            return tempList;
        }

        private void SetSize()
        {
            string tempSize = uiStyleHandler.getSize();

            if (tempSize == "large")
                SetModeLarge();

            else if (tempSize == "normal")
                SetModeNormal();

            else if (tempSize == "small")
                SetModeSmall();
        }

        private bool GetFruitReminderStatus()
        {
            string tempString = uiStyleHandler.getFruitReminderState();
            if (tempString == "true")
                return true;
            else
                return false;
        }

        private void SetDividers(string dividerStatus)
        {
            var dividerRightCM = cm.Items[7] as System.Windows.Controls.MenuItem;
            var dividerLeftCM = cm.Items[8] as System.Windows.Controls.MenuItem;

            if ("left" == dividerStatus)
            {
                dividerRight.Visibility = Visibility.Hidden;
                dividerRightCM.IsChecked = false;
                dividerLeft.Visibility = Visibility.Visible;
                dividerLeftCM.IsChecked = true;
            }
            else if ("right" == dividerStatus)
            {
                dividerRight.Visibility = Visibility.Visible;
                dividerRightCM.IsChecked = true;
                dividerLeft.Visibility = Visibility.Hidden;
                dividerLeftCM.IsChecked = false;
            }
            else if ("none" == dividerStatus)
            {
                dividerRight.Visibility = Visibility.Hidden;
                dividerRightCM.IsChecked = false;
                dividerLeft.Visibility = Visibility.Hidden;
                dividerLeftCM.IsChecked = false;
            }
            else
            {
                dividerRight.Visibility = Visibility.Visible;
                dividerRightCM.IsChecked = true;
                dividerLeft.Visibility = Visibility.Visible;
                dividerLeftCM.IsChecked = true;
            }
        }

        #endregion

        #region Application Close

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveLocation();
        }

        #endregion

        #region Notify Icon 

        private void NotifyIcon()
        {
            NotifyIcon nIcon = new NotifyIcon();
            System.Windows.Forms.ContextMenu nContextMenu = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem nMenuItemClose = new System.Windows.Forms.MenuItem();

            nMenuItemClose.Index = 0;
            nMenuItemClose.Text = "Close";
            nMenuItemClose.Click += NMenuItemClose_Click;

            nContextMenu.MenuItems.Add(nMenuItemClose);

            nIcon.Text = "Clock";
            nIcon.ContextMenu = nContextMenu;
            nIcon.Icon = Properties.Resources.clockIcon;
            nIcon.Visible = true;
        }

        private void NMenuItemClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Timer

        public void DispatcherTimerSample()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private Screen[] screens;

        void timer_Tick(object sender, EventArgs e)
        {
            screens = Screen.AllScreens;
            var screenCount = screens.Length;

            SetNewLocation(screenCount);

            var now = DateTime.Now;
            day.Content = now.ToString("ddd, dd. MMMM");
            hours.Content = now.ToString("HH");
            minutes.Content = now.ToString("mm");

            if (isFruitReminderChecked)
            {
                if (now.DayOfWeek == DayOfWeek.Tuesday || now.DayOfWeek == DayOfWeek.Thursday)
                {
                    if (now.Hour == 9 && now.Minute <= 30 && now.Minute >= 0)
                    {
                        if (!fruitClicked)
                        {
                            if (hours.Foreground.ToString() == "#FFFFFFFF")
                                fruit_white.Visibility = Visibility.Visible;
                            else
                                fruit_black.Visibility = Visibility.Visible;

                            return;
                        }
                    }
                    else
                        fruitClicked = false;
                }
                else
                    fruitClicked = false;
            }
            fruit_white.Visibility = Visibility.Hidden;
            fruit_black.Visibility = Visibility.Hidden;
        }

        #endregion


        #region Application parts

        #region Context Menu

        private void InitializeContextMenu()
        {
            cm = this.FindResource("contextMenu") as System.Windows.Controls.ContextMenu;
            fruitReminder = cm.Items[11] as System.Windows.Controls.MenuItem;
        }

        #endregion

        #region Hours Label Controls

        private void hours_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            cm.PlacementTarget = sender as System.Windows.Controls.Button;
            cm.IsOpen = true;
        }

        private void hours_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.MainWindow.DragMove();
        }

        #endregion

        #region Minute Label Controls

        private void minutes_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            cm.PlacementTarget = sender as System.Windows.Controls.Button;
            cm.IsOpen = true;
        }

        private void minutes_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.MainWindow.DragMove();
        }

        #endregion

        #region Day Label Controls

        private void day_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            cm.PlacementTarget = sender as System.Windows.Controls.Button;
            cm.IsOpen = true;
        }

        private void day_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.MainWindow.DragMove();
        }

        #endregion

        #region Divider Controls

        private void divider_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            cm.PlacementTarget = sender as System.Windows.Controls.Button;
            cm.IsOpen = true;
        }

        private void divider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.MainWindow.DragMove();
        }

        private void dividerRight_Checked(object sender, RoutedEventArgs e)
        {
            dividerRight.Visibility = Visibility.Visible;
        }

        private void dividerRight_Unchecked(object sender, RoutedEventArgs e)
        {
            dividerRight.Visibility = Visibility.Hidden;
        }

        private void dividerLeft_Checked(object sender, RoutedEventArgs e)
        {
            dividerLeft.Visibility = Visibility.Visible;
        }

        private void dividerLeft_Unchecked(object sender, RoutedEventArgs e)
        {
            dividerLeft.Visibility = Visibility.Hidden;
        }

        #endregion

        #region Fruit Reminder Controls

        //Fruit Reminder

        private void fruit_white_MouseUp(object sender, MouseButtonEventArgs e)
        {
            fruitClicked = true;
            fruit_white.Visibility = Visibility.Hidden;
        }

        private void fruit_black_MouseUp(object sender, MouseButtonEventArgs e)
        {
            fruitClicked = true;
            fruit_black.Visibility = Visibility.Hidden;
        }

        private void FruitReminder_Checked(object sender, RoutedEventArgs e)
        {
            fruitReminder = cm.Items[11] as System.Windows.Controls.MenuItem;
            isFruitReminderChecked = true;
            fruitReminder.IsChecked = isFruitReminderChecked;
        }

        private void FruitReminder_Unchecked(object sender, RoutedEventArgs e)
        {
            isFruitReminderChecked = false;
            fruitReminder.IsChecked = isFruitReminderChecked;
        }

        #endregion

        #endregion


        #region Button Click Events

        //Buttons

        private void btnLarge_Click(object sender, RoutedEventArgs e)
        {
            size = "large";
            SetModeLarge();
        }

        private void btnNormal_Click(object sender, RoutedEventArgs e)
        {
            size = "normal";
            SetModeNormal();
        }

        private void btnSmall_Click(object sender, RoutedEventArgs e)
        {
            size = "small";
            SetModeSmall();
        }

        private void btnWhite_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem btnWhite = cm.Items[4] as System.Windows.Controls.MenuItem;

            string color = "#FFFFFFFF";
            SetColorTheme(color);
        }

        private void btnBlack_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem btnBlack = cm.Items[5] as System.Windows.Controls.MenuItem;
            string color = "#FF000000";
            SetColorTheme(color);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region Location

        private int GetLocationX()
        {
            return (int)this.Left;
        }

        private int GetLocationY()
        {
            return (int)this.Top;
        }

        private void SetNewLocation(int newAmount)
        {
            var currentScreen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
            if (screens.Length != newAmount)
            {
                if (screens.Length >= 2)
                {
                    primaryScreenX = (int)this.Left;
                    this.Left = oldLocationX;
                }
                else if (screens.Length == 1)
                {
                    int x = GetLocationX();
                    oldLocationX = x;
                    this.Left = primaryScreenX;
                }
            }
        }

        #endregion

        #region Color Theme

        private System.Windows.Media.Brush BrushToString(string color)
        {
            BrushConverter converter = new BrushConverter();
            System.Windows.Media.Brush brush = (System.Windows.Media.Brush)converter.ConvertFromString(color);
            return brush;
        }

        private void SetColorTheme(string color)
        {
            System.Windows.Media.Brush brush = BrushToString(color);

            foreach (UIElement label in gridMain.Children)
            {
                System.Windows.Controls.Label tempLabel = label as System.Windows.Controls.Label;
                if (tempLabel != null)
                {
                    tempLabel.Foreground = brush;

                    if (color == "#FFFFFFFF")
                    {
                        tempLabel.BorderBrush = Brushes.Black;
                    }
                    else
                    {
                        tempLabel.BorderBrush = Brushes.White;
                    }
                }
                SetFruitIcon(color);
                SetDividerColor(color);
            }
        }

        private void SetFruitIcon(string color)
        {
            if (color == "#FFFFFFFF") //White Color Theme
            {
                if (fruit_black.Visibility == Visibility.Visible)
                {
                    fruit_white.Visibility = Visibility.Visible;
                    fruit_black.Visibility = Visibility.Hidden;
                }
            }
            else if (color == "#FF000000") //Black Color Theme
            {
                if (fruit_white.Visibility == Visibility.Visible)
                {
                    fruit_black.Visibility = Visibility.Visible;
                    fruit_white.Visibility = Visibility.Hidden;
                }
            }
        }

        private void SetDividerColor(string color)
        {
            System.Windows.Media.Brush brush = BrushToString(color);

            if (color == "#FFFFFFFF") //White Color Theme
            {
                dividerRight.Fill = brush;
                dividerLeft.Fill = brush;
            }
            else if (color == "#FF000000") //Black Color Theme
            {
                dividerRight.Fill = brush;
                dividerLeft.Fill = brush;
            }
        }

        #endregion

        #region Sizes

        private void SetModeLarge()
        {
            hours.FontSize = 210;
            hours.Margin = new Thickness(-10, -35, 0, 430);
            minutes.FontSize = 210;
            minutes.Margin = new Thickness(-10, 170, 0, 250);
            day.FontSize = 30;
            day.Margin = new Thickness(-10, 360, 0, 0);

            dividerRight.Height = 400;
            dividerRight.Width = 12;
            dividerRight.Margin = new Thickness(280, 35, 0, 0);
            dividerLeft.Height = 400;
            dividerLeft.Width = 12;
            dividerLeft.Margin = new Thickness(-10, 35, 0, 0);

            fruit_black.Height = 135;
            fruit_black.Width = 135;
            fruit_black.Margin = new Thickness(65, 400, 0, 0);
            fruit_white.Height = 135;
            fruit_white.Width = 135;
            fruit_white.Margin = new Thickness(65, 400, 0, 0);

            size = "large";
        }

        private void SetModeNormal()
        {
            hours.FontSize = 170;
            hours.Margin = new Thickness(0, -50, 0, 430);
            minutes.FontSize = 170;
            minutes.Margin = new Thickness(0, 90, 0, 250);
            day.FontSize = 25;
            day.Margin = new Thickness(0, 300, 0, 0);

            dividerRight.Height = 340;
            dividerRight.Width = 10;
            dividerRight.Margin = new Thickness(280, 35, 0, 0);
            dividerLeft.Height = 340;
            dividerLeft.Width = 10;
            dividerLeft.Margin = new Thickness(5, 35, 0, 0);

            fruit_black.Height = 135;
            fruit_black.Width = 135;
            fruit_black.Margin = new Thickness(80, 350, 0, 0);
            fruit_white.Height = 135;
            fruit_white.Width = 135;
            fruit_white.Margin = new Thickness(80, 350, 0, 0);

            size = "normal";
        }

        private void SetModeSmall()
        {
            hours.FontSize = 130;
            hours.Margin = new Thickness(30, 0, 0, 430);
            minutes.FontSize = 130;
            minutes.Margin = new Thickness(30, 80, 0, 250);
            day.FontSize = 20;
            day.Margin = new Thickness(30, 280, 0, 0);

            dividerRight.Height = 270;
            dividerRight.Width = 8;
            dividerRight.Margin = new Thickness(280, 60, 0, 0);
            dividerLeft.Height = 270;
            dividerLeft.Width = 8;
            dividerLeft.Margin = new Thickness(65, 60, 0, 0);

            fruit_black.Height = 100;
            fruit_black.Width = 100;
            fruit_black.Margin = new Thickness(130, 320, 0, 0);
            fruit_white.Height = 100;
            fruit_white.Width = 100;
            fruit_white.Margin = new Thickness(130, 320, 0, 0);

            size = "small";
        }

        #endregion

        #region Save

        private string DividerStatus()
        {
            string dividerStatus = "";
            dividerStatus = "none";
            if (Visibility.Visible == dividerLeft.Visibility)
            {
                dividerStatus = "left";
                if (Visibility.Visible == dividerRight.Visibility)
                    dividerStatus = "both";
            }
            else if (Visibility.Visible == dividerRight.Visibility)
            {
                dividerStatus = "right";
                if (Visibility.Visible == dividerLeft.Visibility)
                    dividerStatus = "both";
            }
            return dividerStatus;
        }

        private string FruitReminderStatus()
        {
            string fruitReminderStatus = "";
            if (isFruitReminderChecked)
                fruitReminderStatus = "true";
            else
                fruitReminderStatus = "false";
            return fruitReminderStatus;
        }

        private void SaveLocation()
        {
            BrushConverter converter = new BrushConverter();
            System.Windows.Media.Brush brush;
            brush = (System.Windows.Media.Brush)converter.ConvertFromString(hours.Foreground.ToString());

            string fruitReminderStatus = FruitReminderStatus();
            string dividerStatus = DividerStatus();

            Screen[] screens = Screen.AllScreens;
            if (screens.Length == 1)
                uiStyleHandler.SaveStyleAndPosition(brush, oldLocationX, GetLocationY(), GetLocationX(), size, fruitReminderStatus, dividerStatus);
            else
                uiStyleHandler.SaveStyleAndPosition(brush, GetLocationX(), GetLocationY(), primaryScreenX, size, fruitReminderStatus, dividerStatus);
        }
    }

    #endregion
}