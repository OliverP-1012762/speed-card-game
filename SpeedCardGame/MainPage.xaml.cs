using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SpeedCardGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //global variables
        public List<Player> players = new List<Player> { } ;
        public Windows.UI.Xaml.Thickness baseMarginSetupCards = new Thickness(10,0,10,20);
        public List<String> deckCards = new List<string> { };
        Random rand = new Random();
        public static string[] suits = { "Clubs", "Diamonds", "Hearts", "Spades" };
        public String gameStatus = "not Running";
        public String compStatus = "";

        public MainPage()
        {
            this.InitializeComponent();
            
        }

        public class Player
        {
            public String Name;
            public userField field;
            public userPlay playPile;
            public List<String> cardsInDeck;
        }
        public class userPlay
        {
            public int playerSelectedPlayPile;
            public List<String> playerPlayPile;
            public String nameOfLinkedPlayPile;
        }
        public class userField{
            public List<fieldPile> piles;
            public int playerSelectedFieldPile;
            public bool fieldEmpty;//needed
        }
        public class fieldPile{
            public string topCard;
            public List<string> pileCards;
            public String nameOfLinkedPile;
        }
        private void compTurn(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(gamePage), null);
        }
        
    }
}
