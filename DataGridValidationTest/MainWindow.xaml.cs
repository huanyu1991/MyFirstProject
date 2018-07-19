using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Interop;
using System.Collections.Generic;

using System.Runtime.InteropServices;

namespace DataGridValidationTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        [DllImport("user32")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lpRect, MonitorEnumProc callback, int dwData);

        private delegate bool MonitorEnumProc(IntPtr hDesktop, IntPtr hdc, ref Rect pRect, int dwData);


        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        public MainWindow()
        {
            InitializeComponent();
            dataGrid1.InitializingNewItem += (sender, e) =>
            {
                Course newCourse = e.NewItem as Course;
                newCourse.StartDate = newCourse.EndDate = DateTime.Today;
            };


            Display(System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Top, System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Left);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }   

        public void Display(int top,int left)
        {
            
            Window window = this as Window;// RootCtrl为Window类实例

            if (window != null)
            {

                window.Top = top;

                window.Left = left;

                window.Show();

            }

        }

        public static List<MonitorInfo> ActualScreens = new List<MonitorInfo>();

        public static void RefreshActualScreens()
        {
            ActualScreens.Clear();
            MonitorEnumProc callback = (IntPtr hDesktop, IntPtr hdc, ref Rect prect, int d) =>
            {
                ActualScreens.Add(new MonitorInfo()
                {
                    Bounds = new Rectangle()
                    {
                        X = prect.left,
                        Y = prect.top,
                        Width = prect.right - prect.left,
                        Height = prect.bottom - prect.top,
                    },
                    IsPrimary = (prect.left == 0) && (prect.top == 0),
                });

                return true;
            };

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, 0);
        }

        private const int WM_DISPLAYCHANGE = 0x007e;
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            RefreshActualScreens();
            // Handle messages...  
            if (msg == WM_DISPLAYCHANGE)
            {
                if (ActualScreens.Count < 2)
                {
                    Display(ActualScreens[0].Bounds.Top, ActualScreens[0].Bounds.Left);
                }
                else
                {
                    Display(ActualScreens[1].Bounds.Top, ActualScreens[1].Bounds.Left);
                }
                
            }
            return IntPtr.Zero;
        }

       
        //private const int WM_DISPLAYCHANGE = 0x007e;

        //protected override void WndProc(ref Message message)
        //{
        //    //base.WndProc(ref message);

        //    if (message.Msg == WM_DISPLAYCHANGE)
        //    {
        //        Display(System.Windows.Forms.Screen.AllScreens[0]);
        //        // do something really interesting here
        //    }
        //}

    }

    public class MonitorInfo
    {
        public bool IsPrimary = false;
        public Rectangle Bounds = new Rectangle();
    }


    public class Courses : ObservableCollection<Course>
    {
        public Courses()
        {
            this.Add(new Course
            {
                Name = "Learning WPF",
                Id = 1001,
                StartDate = new DateTime(2010, 1, 11),
                EndDate = new DateTime(2010, 1, 22)
            });
            this.Add(new Course
            {
                Name = "Learning Silverlight",
                Id = 1002,
                StartDate = new DateTime(2010, 1, 25),
                EndDate = new DateTime(2010, 2, 5)
            });
            this.Add(new Course
            {
                Name = "Learning Expression Blend",
                Id = 1003,
                StartDate = new DateTime(2010, 2, 8),
                EndDate = new DateTime(2010, 2, 19)
            });
            this.Add(new Course
            {
                Name = "Learning LINQ",
                Id = 1004,
                StartDate = new DateTime(2010, 2, 22),
                EndDate = new DateTime(2010, 3, 5)
            });
        }
    }

    public class Course : IEditableObject, INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private int _number;
        public int Id
        {
            get
            {
                return _number;
            }
            set
            {
                if (_number == value) return;
                _number = value;
                OnPropertyChanged("Id");
            }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                if (_startDate == value) return;
                _startDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                if (_endDate == value) return;
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }

        #region IEditableObject

        private Course backupCopy;
        private bool inEdit;

        public void BeginEdit()
        {
            if (inEdit) return;
            inEdit = true;
            backupCopy = this.MemberwiseClone() as Course;
        }

        public void CancelEdit()
        {
            if (!inEdit) return;
            inEdit = false;
            this.Name = backupCopy.Name;
            this.Id = backupCopy.Id;
            this.StartDate = backupCopy.StartDate;
            this.EndDate = backupCopy.EndDate;
        }

        public void EndEdit()
        {
            if (!inEdit) return;
            inEdit = false;
            backupCopy = null;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }

    public class CourseValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            Course course = (value as BindingGroup).Items[0] as Course;
            if (course.StartDate > course.EndDate)
            {
                return new ValidationResult(false,
                    "Start Date must be earlier than End Date.");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}
