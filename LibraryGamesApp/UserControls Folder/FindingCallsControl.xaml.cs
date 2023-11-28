using LibraryGamesApp.Utilities_Folder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
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
using static LibraryGamesApp.Utilities_Folder.RedBlackTree;

namespace LibraryGamesApp.UserControls_Folder
{
    /// <summary>
    /// Interaction logic for FindingCallsControl.xaml
    /// </summary>
    public partial class FindingCallsControl : UserControl
    {
        RedBlackTree redBlackTree = new RedBlackTree();
        Random random = new Random();
        public static List<string>[] levels;
        private static List<string> correctAnswersList;
        private static SortedDictionary<int, string> sortedDict = new SortedDictionary<int, string>();

        public FindingCallsControl()
        {
            InitializeComponent();
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            levels = saveToTree(levels);

            
            populateButtons1();
        }

        #region populate_buttons
        /// <summary>
        /// Populate the buttons or first level
        /// </summary>
        private void populateButtons1()
        {
            randomQuestion();
            txtDescription.Text = correctAnswersList[3];
            sortedDict.Clear();

            if(levels != null)
            {
                // Access the levels returned from GetLevels method
                List<string> firstLevel = levels[0]; //1st

                //find 3 random 1st level entries to add to the list 
                sortedDict.Add(int.Parse(correctAnswersList[2].Substring(0, 3)), correctAnswersList[2]);
                while (sortedDict.Count < 4)
                {
                    int randomIndex = random.Next(firstLevel.Count);
                    string valueToAdd = firstLevel[randomIndex];

                    int extractInt = int.Parse(valueToAdd.Substring(0, 3));

                    Node item = redBlackTree.Find(extractInt);

                    // Check if the value is unique before adding it to the SortedDictionary
                    if (!sortedDict.ContainsKey(extractInt))
                    {
                        sortedDict.Add(extractInt, (item.data + " " + item.desc));
                    }
                }

                //add texts to buttons 
                // Array of buttons
                Button[] buttons = { btn1, btn2, btn3, btn4 };

                // Assign values from the dictionary to each button's content
                int i = 0;
                foreach (var pair in sortedDict)
                {
                    buttons[i].Content = pair.Value;

                    buttons[i].Click += btn1_Click;
                    i++;
                }


            }
            
        }

        /// <summary>
        /// Populate buttons for second level
        /// </summary>
        private void populateButtons2()
        {
            
            sortedDict.Clear();

            if (levels != null)
            {
                // Access the levels returned from GetLevels method
                List<string> secondLevel = levels[1]; //3rd level

                //find 3 random 1st level entries to add to the list 
                sortedDict.Add(int.Parse(correctAnswersList[1].Substring(0, 3)), correctAnswersList[1]);
                while (sortedDict.Count < 4)
                {
                    int randomIndex = random.Next(secondLevel.Count);
                    string valueToAdd = secondLevel[randomIndex];

                    int extractInt = int.Parse(valueToAdd.Substring(0, 3));

                    Node item = redBlackTree.Find(extractInt);

                    // Check if the value is unique before adding it to the SortedDictionary
                    if (!sortedDict.ContainsKey(extractInt))
                    {
                        sortedDict.Add(extractInt, (item.data + " " + item.desc));
                    }
                }

                //add texts to buttons 
                // Array of buttons
                Button[] buttons = { btn1, btn2, btn3, btn4 };

                // Assign values from the dictionary to each button's content
                int i = 0;
                foreach (var pair in sortedDict)
                {
                    buttons[i].Content = pair.Value;

                    // Remove any existing click event handlers from myButton
                    buttons[i].Click -= btn1_Click;

                    // Assign click event handlers dynamically
                    buttons[i].Click += btn2_Click;
                    i++;
                }

            }

        }

        /// <summary>
        /// Populate the buttons for third level
        /// </summary>
        private void populateButtons3()
        {
            
            sortedDict.Clear();

            if (levels != null)
            {
                // Access the levels returned from GetLevels method
                List<string> thirdlevel = levels[2];

                //find 3 random 1st level entries to add to the list 
                sortedDict.Add(int.Parse(correctAnswersList[0].Substring(0, 3)), correctAnswersList[0]);
                while (sortedDict.Count < 4)
                {
                    int randomIndex = random.Next(thirdlevel.Count);
                    string valueToAdd = thirdlevel[randomIndex];

                    int extractInt = int.Parse(valueToAdd.Substring(0, 3));

                    Node item = redBlackTree.Find(extractInt);

                    // Check if the value is unique before adding it to the SortedDictionary
                    if (!sortedDict.ContainsKey(extractInt))
                    {
                        sortedDict.Add(extractInt, (item.data + " " + item.desc));
                    }
                }

                //add texts to buttons 
                // Array of buttons
                Button[] buttons = { btn1, btn2, btn3, btn4 };

                // Assign values from the dictionary to each button's content
                int i = 0;
                foreach (var pair in sortedDict)
                {
                    buttons[i].Content = pair.Key;

                    // Remove any existing click event handlers from myButton
                    buttons[i].Click -= btn2_Click;
                    buttons[i].Click += (sender, e) => btn3_Click(sender, e);
                    i++;
                }

            }

        }
        #endregion

        #region game_setup
        /// <summary>
        /// Interaction logic for FindingCallsControl.xaml
        /// </summary>
        private void randomQuestion()
        {
            if(levels != null)
            {
                List<string> thirdLevel = levels[2]; //3rd

                
                int randomNumber;
                int parsedNumber;
                do
                {
                    randomNumber = random.Next(1, 990); 
                    parsedNumber = ExtractNumberFromList(randomNumber, thirdLevel);

                    
                } while (parsedNumber == -1);

                Node item = redBlackTree.Find(parsedNumber);

                //when declaring the list i added an extra item.desc to make it easier to display 
                correctAnswersList = new List<string>
                {
                    (item.data + " " + item.desc),

                    FindParents(item.data.ToString(), 2),

                    FindParents(item.data.ToString(), 1),

                    item.desc, 
                    
                    item.data.ToString()
                };
            }
            
        }

        /// <summary>
        /// Searching the tree for the parents of the thirdlevel entry
        /// </summary>
        private string FindParents(string thirdLevel, int level)
        {
            string extractedString;
            if (level == 2)
            {
                extractedString = thirdLevel.Substring(0, 2);
                //Console.WriteLine(extractedString);
                Node item = redBlackTree.Find((int.Parse(extractedString) * 10));
                
                return (item.data + " " + item.desc);
            }
            else
            {
                extractedString = thirdLevel.Substring(0, 1);
                //Console.WriteLine(extractedString);
                Node item = redBlackTree.Find((int.Parse(extractedString) * 100));

                return (item.data + " " + item.desc);
            }
           
        }

        /// <summary>
        /// Interaction logic for FindingCallsControl.xaml
        /// </summary>
        static int ExtractNumberFromList(int number, List<string> list)
        {
            foreach (var item in list)
            {
                
                string[] parts = item.Split(new[] { "." }, StringSplitOptions.None);

                
                if (int.TryParse(parts[0], out int parsed))
                {
                    if (parsed == number)
                    {
                        return parsed; 
                    }
                }
            }
            return -1; 
        }
        #endregion 

        #region Button_click
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                string content = clickedButton.Content.ToString();

                if (correctAnswersList.Contains(content))
                {
                    //give some feedback for correct answer
                    //then ask next quesiton
                    //Console.WriteLine("Correct1");
                    PlaySoundFromResource(true);
                    populateButtons2();
                }
                else
                {
                    PlaySoundFromResource(false);
                    //Console.WriteLine("InCorrect1");
                    //populateButtons1();
                }
            }
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                string content = clickedButton.Content.ToString();

                if (correctAnswersList.Contains(content))
                {
                    //give some feedback for correct answer
                    //then ask next quesiton
                    //Console.WriteLine("Correct2");
                    PlaySoundFromResource(true);
                    populateButtons3();
                }
                else
                {
                    PlaySoundFromResource(false);
                    //Console.WriteLine("InCorrect2");
                    //populateButtons1();
                }
            }
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                string content = clickedButton.Content.ToString();

                if (correctAnswersList.Contains(content))
                {
                    //give some feedback for correct answer
                    //then ask next quesiton
                    //Console.WriteLine("Correct3");
                    PlaySoundFromResource(true);

                }
                else
                {
                    PlaySoundFromResource(false);
                    //Console.WriteLine("InCorrect3");
                    //populateButtons1();
                }
            }
        }
        #endregion

        #region File_Loading
        /// <summary>
        /// Interaction logic for FindingCallsControl.xaml
        /// </summary>
        public List<string>[] saveToTree(List<string>[] levels)
        {
            //read json file content
            string currentDirectory = Directory.GetCurrentDirectory();
            string FileName = "Tree.txt";

            string filePath = System.IO.Path.Combine(currentDirectory, FileName);


            redBlackTree.PopulateTreeFromFile(filePath);
            redBlackTree.GetTreeValues();
            levels = redBlackTree.DisplayTree();

            return levels;
        }
        #endregion

        #region soundFX
        public static void PlaySoundFromResource(bool correct)
        {
            if (correct == true)
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
            else 
            {
                // Create a SoundPlayer instance using the resource stream
                SoundPlayer player = new SoundPlayer(Properties.Resources.WRONG_ANSWER_SOUND_EFFECT);

                // Play the sound
                player.Play();

                // Optionally, you can wait for the sound to finish playing
                while (player.IsLoadCompleted == false) { /* Wait for the sound to finish loading */ }

                // Release the resources used by the SoundPlayer
                player.Dispose();
            }

        }
    }
    #endregion

    
}
