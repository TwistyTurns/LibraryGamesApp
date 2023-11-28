using LibraryGamesApp.Utilities_Folder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
using System.Windows.Threading;

namespace LibraryGamesApp.UserControls_Folder
{
    /// <summary>
    /// Interaction logic for BookReplacingControl.xaml
    /// </summary>
    public partial class BookReplacingControl : UserControl
    {
        CallNumberGenerator callNumber = new CallNumberGenerator();
        Random random = new Random();

        private DispatcherTimer timer;
        private TimeSpan elapesedTime = TimeSpan.Zero;

        private List<string> bookList = new List<string> { };
        private List<Rectangle> rectanglesList = new List<Rectangle>();
        private static SortedDictionary<string, int> correctAnswerDict = new SortedDictionary<string, int>();


        private Point startPoint;
        private bool isDragging = false;

        private double originalLeft;
        ///-----------------------------------------------------------------------------------------

        public BookReplacingControl()
        {
            InitializeComponent();
            InitiliazeTimer();
        }
        ///-----------------------------------------------------------------------------------------

        #region setup_Game
        private void InitiliazeTimer()
        {
            timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            Panel.SetZIndex(timerLabel, 999);
        }

        private void CreateBooks()
        {
            double x = 10;

            //loops and creates 10 books
            for (int i = 0; i < 10; i++)
            {
                string calls = callNumber.GenCallNumbers();

                bookList.Add(calls);

                correctAnswerDict.Add(calls, i);
            }
            

            foreach(string item in bookList)
            {
                SolidColorBrush brushCol = GetRandomBrush();

                Rectangle rectangle = new Rectangle
                {
                    Width = 60,
                    Height = 398,
                    Fill = brushCol,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Tag = item
                };
                rectangle.IsHitTestVisible = true;

                //----new stuff
                TextBlock txtBlock = new TextBlock
                {
                    Text = item.ToString(),
                    Tag = item,
                    Width = 60,
                    Foreground = Brushes.Black,
                    Background = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center

                };
                

                //-----previous stuff
                Canvas.SetLeft(rectangle, x);
                Canvas.SetTop(rectangle, 1);
                CanvasBook.Children.Add(rectangle);

                Canvas.SetLeft(txtBlock, x);
                Canvas.SetBottom(txtBlock, 50);
                CanvasBook.Children.Add(txtBlock);

                rectangle.MouseLeftButtonDown += Rectangle_MouseLeftButtonDown;
                rectangle.MouseMove += Rectangle_MouseMove;
                rectangle.MouseLeftButtonUp += Rectangle_MouseLeftButtonUp;
                //-----


                rectanglesList.Add(rectangle);
                x += 60;
            }
        }
        ///-----------------------------------------------------------------------------------------

        public SolidColorBrush GetRandomBrush()
        {
            byte red = (byte)random.Next(256);
            byte blue = (byte)random.Next(256);
            byte green = (byte)random.Next(256);

            SolidColorBrush randomBrush = new SolidColorBrush(Color.FromRgb(red, green, blue));

            return randomBrush;
        }
        ///-----------------------------------------------------------------------------------------
        
        // Logic for when the left mouse button is pressed on a rectangle
        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
            isDragging = true;

           
            Rectangle rectangle = sender as Rectangle;

            originalLeft = Canvas.GetLeft(rectangle);

            startPoint = e.GetPosition (null);
            rectangle.CaptureMouse();
        }
        ///----------------------------------------------------------------------------------------- <summary>
        /// -----------------------------------------------------------------------------------------
        /// 
        private void Timer_Tick(object sender, EventArgs e)
        {
            elapesedTime = elapesedTime.Add(TimeSpan.FromSeconds(1));
            updateTimerLabel();
        }
        ///-----------------------------------------------------------------------------------------

        private void updateTimerLabel()
        {
            timerLabel.Content = $"Time:{elapesedTime:mm\\:ss}";
        }
        ///-----------------------------------------------------------------------------------------
        
        #endregion

        #region event_handler
        // Logic for when the rectangle is moving
        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Rectangle rectangle = sender as Rectangle;
                Point currentPosition = e.GetPosition(null);

                double deltaX = currentPosition.X - startPoint.X;
                double newLeft = Canvas.GetLeft(rectangle) + deltaX;

                foreach(UIElement element in CanvasBook.Children)
                {
                    if(element is TextBlock txtBlock && txtBlock.Tag == rectangle.Tag)
                    {
                        Canvas.SetLeft(txtBlock, newLeft);
                    }
                }

                //----test
                int index = rectanglesList.IndexOf(rectangle);

                if(index >= 0 && index < rectanglesList.Count)
                {
                    rectanglesList[index] = rectangle;
                }
                //----


                Canvas.SetLeft(rectangle, newLeft);
                startPoint = currentPosition;
            }
        }
        ///-----------------------------------------------------------------------------------------
        
        // Logic for when the left mouse button is released on a rectangle
        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            Rectangle movingRectangle = sender as Rectangle;
            movingRectangle.ReleaseMouseCapture();

            double newLeft = Canvas.GetLeft(movingRectangle);
            double newTop = Canvas.GetTop(movingRectangle);

            bool collisionDetected = false;

            foreach(Rectangle rectangle1 in rectanglesList)
            {
                if(rectangle1 != movingRectangle && CollisionTrue(movingRectangle, rectangle1, newLeft, newTop))
                {
                    Canvas.SetLeft(movingRectangle, originalLeft);

                    foreach (UIElement element in CanvasBook.Children)
                    {
                        if (element is TextBlock txtBlock && txtBlock.Tag == movingRectangle.Tag)
                        {
                            Canvas.SetLeft(txtBlock, originalLeft);
                        }
                    }

                    collisionDetected = true;

                    //for debugging
                    //Console.WriteLine("You Failed");
                    break;
                }
                else 
                {
                    Canvas.SetLeft(movingRectangle, newLeft);
                }
            }

            
            if(collisionDetected == false)
            {
                
                //update list of books
                UpdateBookList();

                //for debugging
                //Console.WriteLine("list updated");
                
            }
        }
        ///-----------------------------------------------------------------------------------------
        #endregion

        // Update the bookList based on the new order
        private void UpdateBookList()
        {
            //sort list by their left position relative to the canvas
            //used to populate the booklist with the callnumbers of the books
            //this is done so that the booklist order matches the order on screen
            rectanglesList = rectanglesList.OrderBy(Rectangle => Canvas.GetLeft(Rectangle)).ToList();

            if (checkCompletion())
            {
                //Console.WriteLine("Complete: 269");
            }
            
                
        }
        ///-----------------------------------------------------------------------------------------

        private bool CollisionTrue(Rectangle rect1, Rectangle rect2, double newLeft, double newTop)
        {
            Rect rect1Bounds = new Rect(newLeft, newTop, rect1.Width, rect1.Height);

            double rect2Left = Canvas.GetLeft(rect2);
            double rect2Top = Canvas.GetTop(rect2);
            Rect rect2Bounds = new Rect(rect2Left, rect2Top, rect2.Width, rect2.Height);

           
            return rect1Bounds.IntersectsWith(rect2Bounds);
        }
        ///-----------------------------------------------------------------------------------------

        /// <summary>
        /// method to check for completion,
        /// returns true if order matches
        /// </summary>
        private bool checkCompletion()
        {
            bool orderMatches = correctAnswerDict.Keys.SequenceEqual(bookList);

            return orderMatches;
        }
        ///-----------------------------------------------------------------------------------------

        //noticed that createBooks() was being called when the application ran,
        //instead of when i actually called it (i.e. when the user clicks start game)
        //this means that when the app starts it created the 10 books even before the user selected anything
        //solution: moved it out of the constructor and put it here so that its only used when the control loads
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CreateBooks();
            timer.Start();
        }
        ///-----------------------------------------------------------------------------------------

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            updateTimerLabel();

            checkCompletion();
        }
        ///-----------------------------------------------------------------------------------------


        public static void PlaySoundFromResource(bool complete)
        {
            
            // Create a SoundPlayer instance using the resource stream
            SoundPlayer player = new SoundPlayer(Properties.Resources.Victory_Sound);

            // Play the sound synchronously
            player.PlaySync();

            // Wait for 1 second (adjust the milliseconds as needed)
            Thread.Sleep(3000); // 1000 milliseconds = 1 second

            player.Dispose();
            
            
        }
    }
}
///-----------------------------------------------END-----------------------------------------------
