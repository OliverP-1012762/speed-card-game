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
        Player mainPlayer = new Player();
        public Windows.UI.Xaml.Thickness baseMarginSetupCards = new Thickness(10,0,10,20);
        public List<String> deckCards = new List<string> { };
        Random rand = new Random();
        public static string[] suits = { "Clubs", "Diamonds", "Hearts", "Spades" };

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

        public List<String> shuffle(List<String> inList)
        {
            List<String> outList = new List<String> { };
            while (inList.Count > 0)
            {
                var randomCard = inList[rand.Next(0, inList.Count)];
                outList.Add(randomCard);
                inList.Remove(randomCard);
            }
            return outList;
        }

        public void moveCard(List<String> from, List<String> moveTo, String whatMoved)
        {
            if (whatMoved == "random")
            {
                var moveCard = from[rand.Next(0, from.Count)];
                moveTo.Add(moveCard);
                from.Remove(moveCard);
            }
            else if (whatMoved == "first")
            {
                var moveCard = from[0];
                moveTo.Add(moveCard);
                from.Remove(moveCard);
            }
            else if (whatMoved == "last")
            {
                var moveCard = from[from.Count];
                moveTo.Add(moveCard);
                from.Remove(moveCard);
            }
        }

        public void setUpGame()
        {
            mainPlayer.field = new userField();
            mainPlayer.cardsInDeck = new List<string> { };
            mainPlayer.playPile = new userPlay();
            //intialise deck
            foreach (string suit in suits)
            {
                string adCardtoDeck = suit;
                for (int vals = 1; vals < 14; vals++)
                {
                    if (vals < 10)
                    {
                        deckCards.Add($"{adCardtoDeck}0{vals}");
                    }
                    else
                    {
                        deckCards.Add($"{adCardtoDeck}{vals}");
                    }
                }
            }
            deckCards = shuffle(deckCards);
            //basic intialisation of class add more when random cards in deck implimetnted
            for (int dealCard = Convert.ToInt32(Math.Floor(Convert.ToDecimal(deckCards.Count / 2))); dealCard > 0; dealCard--)
            {
                moveCard(deckCards,mainPlayer.cardsInDeck,"random");
            }
            mainPlayer.field.piles = new List<fieldPile> { };
            for (int i = 1; i < 6; i++)
            {
                fieldPile pile = new fieldPile();
                pile.pileCards = new List<string> { };
                for (int j = i; j > 0; j--) {
                    moveCard(mainPlayer.cardsInDeck, pile.pileCards, "first");
                }
                pile.topCard = pile.pileCards[0];
                pile.nameOfLinkedPile =  $"playerFieldPile{i}";    
                mainPlayer.field.piles.Add(pile);
            }
            mainPlayer.field.playerSelectedFieldPile = 0;
            UpdateFieldCards("selectedFieldChanged");
            mainPlayer.playPile.playerSelectedPlayPile = 1;
            UpdateFieldCards("selectedPlayChanged");
            mainPlayer.field.fieldEmpty = false;
            mainPlayer.playPile.playerPlayPile = new List<String> { };
            moveCard(mainPlayer.cardsInDeck, mainPlayer.playPile.playerPlayPile, "first");
            Grid.FocusVisualSecondaryBrushProperty.Equals( "#FF297837");
            updateVisualPilesAll();
        }
        public void updateVisualPilesAll()
        {
            if (mainPlayer.field.piles != null)
            {
                foreach (var set in mainPlayer.field.piles)
                {
                    if (set.pileCards.Count != 0)
                    {
                        set.topCard = set.pileCards[0];
                    }
                    else
                    {
                        set.topCard = "Card_back";
                    }
                    switch (set.nameOfLinkedPile)
                    {
                        //don't know how to dynamicaly call elements
                        // don't know a way to convert string to Windows.UI.Xaml.Controls.Image/ dynamic calling of images
                        case ("playerFieldPile1"):
                            playerFieldPile1.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                            break;
                        case ("playerFieldPile2"):
                            playerFieldPile2.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                            break;
                        case ("playerFieldPile3"):
                            playerFieldPile3.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                            break;
                        case ("playerFieldPile4"):
                            playerFieldPile4.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                            break;
                        case ("playerFieldPile5"):
                            playerFieldPile5.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                            break;
                    }
                }
            }
            if (mainPlayer.playPile.playerPlayPile != null) {
                playerPlayPile.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{mainPlayer.playPile.playerPlayPile[mainPlayer.playPile.playerPlayPile.Count-1]}.png"));//add special case for round win event
            }
        }
        public void UpdateFieldCards(String sendingOpperation)
        {
            //update selected
            if (sendingOpperation == "selectedFieldChanged")
            {
                // don't know a way to convert string to Windows.UI.Xaml.Controls.Image/ dynamic calling of images
                switch (mainPlayer.field.playerSelectedFieldPile)
                {
                    case(0):
                        playerFieldPile2.Margin = baseMarginSetupCards;
                        playerFieldPile1.Margin = new Thickness(0);
                        break;
                    case (1):
                        playerFieldPile1.Margin = baseMarginSetupCards;
                        playerFieldPile3.Margin = baseMarginSetupCards;
                        playerFieldPile2.Margin = new Thickness(0);
                        break;
                    case (2):
                        playerFieldPile2.Margin = baseMarginSetupCards;
                        playerFieldPile4.Margin = baseMarginSetupCards;
                        playerFieldPile3.Margin = new Thickness(0);
                        break;
                    case (3):
                        playerFieldPile3.Margin = baseMarginSetupCards;
                        playerFieldPile5.Margin = baseMarginSetupCards;
                        playerFieldPile4.Margin = new Thickness(0);
                        break;
                    case (4):
                        playerFieldPile4.Margin = baseMarginSetupCards;
                        playerFieldPile5.Margin = new Thickness(0);
                        break;
                }

                //MainPage.((stack.nameOfLinkedPile).To).Margin = new Thickness(0, 0, 0, 0);
            }
            if (sendingOpperation == "selectedPlayChanged")
            {
                switch (mainPlayer.playPile.playerSelectedPlayPile)
                {
                    case (0):
                        playerPlayPile.Margin = baseMarginSetupCards;
                        compPlayPile.Margin = new Thickness(0);
                        break;
                    case (1):
                        compPlayPile.Margin = baseMarginSetupCards;
                        playerPlayPile.Margin = new Thickness(0);
                        break;
                }
            }

            //update card placed

            //update top card
        }
        public MainPage()
        {
            this.InitializeComponent();
            setUpGame();
            
        }

        private void KeyPressed(object sender, KeyRoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(e.Key);
            //System.Diagnostics.Debug.WriteLine(sender);
            if (Convert.ToString(e.Key) == "A"|| Convert.ToString(e.Key) == "Left")
            {
                if (mainPlayer.field.playerSelectedFieldPile != 0)
                {
                    mainPlayer.field.playerSelectedFieldPile--;
                    UpdateFieldCards("selectedFieldChanged");
                }
            }
            else if (Convert.ToString(e.Key) == "D" || Convert.ToString(e.Key) == "Right")
            {
                if (mainPlayer.field.playerSelectedFieldPile != 4)
                {
                    mainPlayer.field.playerSelectedFieldPile++;
                    UpdateFieldCards("selectedFieldChanged");
                }
            }
            else if (Convert.ToString(e.Key) == "W" || Convert.ToString(e.Key) == "Up")
            {
                if (mainPlayer.playPile.playerSelectedPlayPile != 1)
                {
                    mainPlayer.playPile.playerSelectedPlayPile++;
                    UpdateFieldCards("selectedPlayChanged");
                }
            }
            else if (Convert.ToString(e.Key) == "S" || Convert.ToString(e.Key) == "Down")
            {
                if (mainPlayer.playPile.playerSelectedPlayPile != 0)
                {
                    mainPlayer.playPile.playerSelectedPlayPile--;
                    UpdateFieldCards("selectedPlayChanged");
                }
            }
        }
    }
}
