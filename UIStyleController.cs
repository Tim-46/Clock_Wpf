using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Forms;


namespace Clock_WPF
{
    public class UIStyleController
    {
        public bool RestoreStyleAndPosition(List<System.Windows.Controls.Label> labelsToRestore, List<System.Windows.Shapes.Rectangle> rectanglesToRestore, MainWindow mainWindow)
        {
            try
            {
                CheckIfFileExist("Clock.ini");

                const Int32 BufferSize = 128;
                using (FileStream fileStream = File.OpenRead(path + @"Clock.ini"))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                    {
                        String line;
                        if ((line = streamReader.ReadLine()) != null)
                        {
                            var values = line.Split(';');
                            BrushConverter converter = new BrushConverter();
                            Brush brush = (Brush)converter.ConvertFromString(values[0]);

                            if (brush != null)
                            {
                                foreach (System.Windows.Controls.Label label in labelsToRestore)
                                {
                                    label.Foreground = brush;
                                }
                                foreach (System.Windows.Shapes.Rectangle rec in rectanglesToRestore)
                                {
                                    rec.Fill = brush;
                                }
                            }

                            int x = 0, y = 0;

                            Int32.TryParse(values[1], out x);
                            if (values[1] == null)
                                x = 0;

                            Int32.TryParse(values[2], out y);
                            if (values[2] == null)
                                y = 0;

                            Screen[] screens = Screen.AllScreens;
                            if (screens.Length == 1)
                            {
                                Int32.TryParse(values[3], out x);
                                if (values[3] == null)
                                    x = 0;
                            }
                            mainWindow.Top = y;
                            mainWindow.Left = x;
                        }
                        else
                        {
                            BrushConverter converter = new BrushConverter();
                            Brush brush = (Brush)converter.ConvertFromString("#FFFFFFFF");
                            foreach (System.Windows.Controls.Label label in labelsToRestore)
                            {
                                label.Foreground = brush;
                            }
                            foreach (System.Windows.Shapes.Rectangle rec in rectanglesToRestore)
                            {
                                rec.Fill = brush;
                            }
                            mainWindow.Left = 0;
                            mainWindow.Top = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public bool SaveStyleAndPosition(Brush styleToSave, int secondaryScreenX, int y, int primaryScreenX, string size, string isFruitReminderChecked, string divider)
        {
            StringBuilder stringBuilder = new StringBuilder();

            try
            {
                using (var fileStream = File.OpenWrite(path + @"Clock.ini"))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        stringBuilder.Append(styleToSave.ToString() + ";");
                        stringBuilder.Append(secondaryScreenX.ToString() + ";");
                        stringBuilder.Append(y.ToString() + ";");
                        stringBuilder.Append(primaryScreenX.ToString() + ";");
                        stringBuilder.Append(size + ";");
                        stringBuilder.Append(isFruitReminderChecked + ";");
                        stringBuilder.Append(divider);
                        streamWriter.WriteLine(stringBuilder.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return false;
        }

        public int GetPrimaryScreenX()
        {
            int x = 0;
            try
            {
                CheckIfFileExist("Clock.ini");

                const Int32 BufferSize = 128;
                using (FileStream fileStream = File.OpenRead(path + @"Clock.ini"))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                    {
                        String line;
                        if ((line = streamReader.ReadLine()) != null)
                        {
                            var values = line.Split(';');
                            Int32.TryParse(values[3], out x);

                            return x;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return x;
        }

        public string getColor()
        {
            string color = "";
            try
            {
                CheckIfFileExist("Clock.ini");

                const Int32 BufferSize = 128;
                using (FileStream fileStream = File.OpenRead(path + @"Clock.ini"))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                    {
                        String line;
                        if ((line = streamReader.ReadLine()) != null)
                        {
                            var values = line.Split(';');
                            color = values[0];

                            return color;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return color;
        }

        public string getSize()
        {
            string size = "";
            try
            {
                CheckIfFileExist("Clock.ini");

                const Int32 BufferSize = 128;
                using (FileStream fileStream = File.OpenRead(path + @"Clock.ini"))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                    {
                        String line;
                        if ((line = streamReader.ReadLine()) != null)
                        {
                            var values = line.Split(';');
                            size = values[4];

                            return size;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return size;
        }

        public string getFruitReminderState()
        {
            string fruitReminder = "";
            try
            {
                CheckIfFileExist("Clock.ini");

                const Int32 BufferSize = 128;
                using (FileStream fileStream = File.OpenRead(path + @"Clock.ini"))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                    {
                        String line;
                        if ((line = streamReader.ReadLine()) != null)
                        {
                            var values = line.Split(';');
                            fruitReminder = values[5];

                            return fruitReminder;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return fruitReminder;
        }

        public string getDividerStatus()
        {
            string dividerStatus = "";
            try
            {
                CheckIfFileExist("Clock.ini");

                const Int32 BufferSize = 128;
                using (FileStream fileStream = File.OpenRead(path + @"Clock.ini"))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                    {
                        String line;
                        if ((line = streamReader.ReadLine()) != null)
                        {
                            var values = line.Split(';');
                            dividerStatus = values[6];

                            return dividerStatus;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dividerStatus;
        }

        private string path { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Clock\");

        private void CheckIfFileExist(string fileName)
        {
            try
            {
                if (!File.Exists(path + @fileName))
                    throw new FileNotFoundException();
            }
            catch (FileNotFoundException)
            {
                System.IO.Directory.CreateDirectory(path);
                var createFile = File.Create(path + @fileName);
                createFile.Close();
            }
        }
    }
}