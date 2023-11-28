using LibraryGamesApp.Utilities_Folder;
using System;
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
using System.Xml.Serialization;

namespace LibraryGamesApp.UserControls_Folder
{
    /// Interaction logic for IdentifyAreasControl.xaml
    /// </summary>
    public partial class IdentifyAreasControl : UserControl
    {
        private static int lifeCounter = 3;
        private static int correctAnswerCounter = 0;

        private static int roundCounter = 1;

        private static int leftButtonsCount = 4;
        private static int rightButtonsCount = 7;

        private static string btnContentR;
        private static string btnContentL;

        private Button leftButton;
        private Button rightButton;

        CallNumberGenerator callNumber = new CallNumberGenerator();
        Random random = new Random();

        //Dictionary<string, int> dictionaryComparison;
        Dictionary<int, string> dictionaryTemp = new Dictionary<int, string>();

        HashSet<int> usedButtons = new HashSet<int>();
        ///-----------------------------------------------------------------------------------------

        /// <summary>
        public IdentifyAreasControl()
        {
            InitializeComponent();
        }

        //event handlers for the user control
        #region event handlers
        private void btnRight1_Click(object sender, RoutedEventArgs e)
        {
            rightButton = sender as Button;
            

            btnContentR =  rightButton.Content.ToString();

            if(btnContentL!= null)
            {
                if (callNumber.CompareDescriptions(btnContentL, btnContentR, roundCounter) == true)
                {
                    //Console.WriteLine("true"); ----------------------------------------------> for testing
                    correctAnswerCounter++;
                    rightButton.IsEnabled = false;
                    leftButton.IsEnabled = false;

                    
                    //enables the contine button when the correct answers = 4, 
                    //meaning youve completed the game
                    if (correctAnswerCounter == 4)
                    {
                        btnSubmit.IsEnabled = true;
                    }
                    resetStrings();
                }
                else
                {
                    lifeCounter --;
                    lifeTracker();
                    resetStrings();
                    //Console.WriteLine("false"); --------------------------------------------> for testing
                }
            }
            else
            {
                
            }
            
        }

        private void btnleft1_Click(object sender, RoutedEventArgs e)
        {
            //finds the button that was clicked
            leftButton = sender as Button;

            //set the variable to the text inside the button
            btnContentL = leftButton.Content.ToString();

            if (btnContentR != null)
            {
                if (callNumber.CompareDescriptions(btnContentL, btnContentR, roundCounter) == true)
                {
                    //Console.WriteLine("true"); ----------------------------------------------> for testing
                    correctAnswerCounter++;
                    rightButton.IsEnabled = false;
                    leftButton.IsEnabled = false;

                    
                    //enables the contine button when the correct answers = 4, 
                    //meaning youve completed the game
                    if (correctAnswerCounter == 4)
                    {
                        btnSubmit.IsEnabled = true;
                    }
                    resetStrings();
                }
                else
                {
                    lifeCounter--;
                    lifeTracker();
                    resetStrings();
                    //Console.WriteLine("false"); --------------------------------------------> for testing
                }
            }
            else
            {

            }
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            changeButtonsTextNormal();
            txtRounds.Text = "Rounds: " + roundCounter.ToString();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //clear dictionary and hashset so that they can be repopulated with new values
            dictionaryTemp.Clear();
            usedButtons.Clear();

            resetStrings();
            EnableButtonsInGrid(leftGrid, true);
            EnableButtonsInGrid(rightGrid, true);
            btnSubmit.IsEnabled = false;

            roundCounter++;
            txtRounds.Text = "Rounds: " + roundCounter.ToString();

            lifeCounter = 3;

            BitmapImage fullBar = new BitmapImage(new Uri("/Resources/fullBar.png", UriKind.RelativeOrAbsolute));

            myImage.Source = fullBar;

            if (roundCounter % 2 == 0)
            {
                changeButtonsTextReverse();
            }
            else
            {
                changeButtonsTextNormal();
            }

            
        }

       
        #endregion

        //this will probably be moved to a HelperClass in future
        #region functions
        //populate right side with descriptions and left side with call numbers 
        private void changeButtonsTextNormal()
        {
            

            //loops 4 times to populate left side with call numbers
            for (int i = 1; i <= leftButtonsCount; i++)
            {
                string btnLeft = ("btnleft" + i);
                
               
                Button buttonLeft = FindName(btnLeft) as Button;

                string callNumb = callNumber.GenCallNumbers();

                if (buttonLeft != null)
                {
                    dictionaryTemp.Add(i, callNumb);
                    buttonLeft.Content = callNumb;
                }
            }

            for(int i = 1;i <= rightButtonsCount; i++)
            {
                int btnNumber;

                do
                {
                    btnNumber = random.Next(1, 8);
                }
                while (usedButtons.Contains(btnNumber));

                usedButtons.Add(btnNumber);

                string btnRight = ("btnright" + btnNumber);

                Button buttonRight = FindName(btnRight) as Button;
                

                string callNumb;

                if(i < 5)
                {
                    if (buttonRight != null)
                    {
                        callNumb = dictionaryTemp[i];
                        buttonRight.Content = callNumber.getmatchingDescription(callNumb, i);

                    }
                }
                else
                {
                    buttonRight.Content = callNumber.getmatchingDescription("0", i);
                }
                

            }
        }

        //populate right side with call numbers and left side with descriptions
        private void changeButtonsTextReverse()
        {
            //loops 7 times to populate right side with call numbers
            for (int i = 1; i <= rightButtonsCount; i++)
            {
                string btnRight = ("btnright" + i);

                Button buttonRight = FindName(btnRight) as Button;

                string callNumb = callNumber.GenCallNumbers();

                if (buttonRight != null)
                {
                    dictionaryTemp.Add(i, callNumb);
                    buttonRight.Content = callNumb;
                }
            }

            for (int i = 1; i <= leftButtonsCount; i++)
            {
                string callNumb;

                //gets a random number
                //adds it to a hashset
                //this is for assigning the value to a random button
                //use of hashset ensures that values arent written onto already written buttons
                int btnNumber;

                do
                {
                    btnNumber = random.Next(1, 5);
                }
                while (usedButtons.Contains(btnNumber));

                usedButtons.Add(btnNumber);

                //finding the buttons
                string btnLeft = ("btnleft" + btnNumber);

                Button buttonLeft = FindName(btnLeft) as Button;

                //assigning values to the buttons
                if (i <= rightButtonsCount)
                {
                    if (buttonLeft != null)
                    {
                        callNumb = dictionaryTemp[i];
                        buttonLeft.Content = callNumber.getmatchingDescription(callNumb, i);

                    }
                }
                else
                {
                    buttonLeft.Content = callNumber.getmatchingDescription("0", i);
                }
            }
        }
        
        //enables the buttons again
        private void EnableButtonsInGrid(Grid grid, bool enable)
        {
            foreach (UIElement element in grid.Children)
            {
                if (element is Button button)
                {
                    button.IsEnabled = enable;
                }
            }
        }

        private void lifeTracker()
        {
            //change img each time the life counter goes down
            switch (lifeCounter)
            {
                case 0:
                    
                    BitmapImage emptybar = new BitmapImage(new Uri("/Resources/emptyBar.png", UriKind.RelativeOrAbsolute));
                    
                    myImage.Source = emptybar;

                    MessageBox.Show("Reloading Columns...", "<---You Failed--->");
                    retryAfterDeath();
                    break;
                case 1:
                    
                    BitmapImage oneBar = new BitmapImage(new Uri("/Resources/1Bar.png", UriKind.RelativeOrAbsolute));

                    myImage.Source = oneBar;
                    break;
                case 2:
                    
                    BitmapImage twoBar = new BitmapImage(new Uri("/Resources/2Bar.png", UriKind.RelativeOrAbsolute));

                    myImage.Source = twoBar;
                    break;  
                    
                default:
                    //full health

                    BitmapImage fullBar = new BitmapImage(new Uri("/Resources/fullBar.png", UriKind.RelativeOrAbsolute));

                    myImage.Source = fullBar;
                    break;

            }
            
        }

        private void retryAfterDeath()
        {
            dictionaryTemp.Clear();
            usedButtons.Clear();

            EnableButtonsInGrid(leftGrid, true);
            EnableButtonsInGrid(rightGrid, true);
            btnSubmit.IsEnabled = false;

            roundCounter = 1;
            lifeCounter = 3;

            BitmapImage fullBar = new BitmapImage(new Uri("/Resources/fullBar.png", UriKind.RelativeOrAbsolute));

            myImage.Source = fullBar;

            changeButtonsTextNormal();
            txtRounds.Text = "Rounds: " + roundCounter.ToString();
            
        }

        //after testing i found that the values for the strings were being saved
        //after they were used for comparison, which allowed user to continue comparing using the old values
        //fixed that by making them null again
        private void resetStrings()
        {
            btnContentL = null;
            btnContentR = null;

            if(correctAnswerCounter == 4)
            {
                correctAnswerCounter = 0;
            }
            
        }
        #endregion

        public static void PlaySoundFromResource(bool complete)
        {
            if(complete == true)
            {
                // Create a SoundPlayer instance using the resource stream
                SoundPlayer player = new SoundPlayer(Properties.Resources.Victory_Sound);

                // Play the sound synchronously
                player.PlaySync();

                // Wait for 1 second (adjust the milliseconds as needed)
                Thread.Sleep(3000); // 1000 milliseconds = 1 second

                player.Dispose();
            }
            else
            {
                // Create a SoundPlayer instance using the resource stream
                SoundPlayer player = new SoundPlayer(Properties.Resources.Correct_Answer_Sound_Effect);

                // Play the sound
                player.Play();

                // Optionally, you can wait for the sound to finish playing
                while (player.IsLoadCompleted == false) { /* Wait for the sound to finish loading */ }

                // Release the resources used by the SoundPlayer
                player.Dispose();
            }
            

        }
    }
}
///-----------------------------------------------END-----------------------------------------------