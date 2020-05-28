using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpeedCardGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class gamePage : Page
    {
        //global variables
        public List<Player> players = new List<Player> { };
        public Windows.UI.Xaml.Thickness baseMarginSetupCards = new Thickness(10, 0, 10, 20);
        public List<String> deckCards = new List<string> { };
        Random rand = new Random();
        public static string[] suits = { "Clubs", "Diamonds", "Hearts", "Spades" };
        public String gameStatus = "not Running";
        public String compStatus = "";
        public int page = 1;

        public class Card
        {
            public String Name;
            public int value;
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
        public class userField
        {
            public List<fieldPile> piles;
            public int playerSelectedFieldPile;
            public bool fieldEmpty;//needed
        }
        public class fieldPile
        {
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
        public String checkSlap()
        {
            String oneFieldEmpty = null;
            foreach (var UsersToCheckField in players)
            {
                var amountEmpty = 0;
                foreach (var pileCount in UsersToCheckField.field.piles)
                {
                    if (pileCount.pileCards.Count == 0)
                    {
                        amountEmpty++;
                    }
                }
                if (amountEmpty == UsersToCheckField.field.piles.Count)
                {
                    oneFieldEmpty = UsersToCheckField.Name;
                }
            }
            return oneFieldEmpty;
        }
        public bool checkWin()
        {
            var Win = false;
            foreach (Player checkWinP in players)
            {
                if (checkWinP.cardsInDeck.Count <= 1)
                {
                    if (Win)
                    {
                        System.Diagnostics.Debug.WriteLine("error, more than 1 player wins");
                    }
                    else
                    {
                        Win = true;
                    }
                }
            }
            return Win;
        }
        public async void winMsg()
        {
            var winner = players[0].Name;
            var dialog = new MessageDialog($"Player {winner} wins");
            dialog.Commands.Add(new UICommand { Label = "back", Id = 0});
            //new UICommandInvokedHandler(gamePage.("HMMMMMMMMMM", .StatusMessage));
            //_ = await dialog.ShowAsync();
            var res = await dialog.ShowAsync();
            if ((int)res.Id == 0)
            {
                System.Diagnostics.Debug.WriteLine("a");
            }
        }
        public void moveCard(List<String> from, List<String> moveTo, String whatMoved, String whereMove)
        {
            if (whereMove == "")
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
            else if (whereMove == "front")
            {
                if (whatMoved == "random")
                {
                    var moveCard = from[rand.Next(0, from.Count)];
                    moveTo.Insert(0, moveCard);
                    from.Remove(moveCard);
                }
                else if (whatMoved == "first")
                {
                    var moveCard = from[0];
                    moveTo.Insert(0, moveCard);
                    from.Remove(moveCard);
                }
                else if (whatMoved == "last")
                {
                    var moveCard = from[from.Count];
                    moveTo.Insert(0, moveCard);
                    from.Remove(moveCard);
                }
            }
        }
        public void setUpGame()
        {
            if (gameStatus == "not Running")
            {
                //intialise deck
                deckCards = new List<String> { };
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
                //initialise players
                if (players.Count == 0)
                {
                    players.Add(new Player());
                    players[0].Name = "player";
                    players.Add(new Player());
                    players[1].Name = "comp";
                }
                var CardsToEachPlayer = deckCards.Count / players.Count;

                foreach (var playersToSetUp in players)
                {
                    playersToSetUp.field = new userField();
                    playersToSetUp.cardsInDeck = new List<string> { };
                    playersToSetUp.playPile = new userPlay();


                    //basic intialisation of class add more when random cards in deck implimetnted
                    for (int dealCard = Convert.ToInt32(Math.Floor(Convert.ToDecimal(CardsToEachPlayer))); dealCard > 0; dealCard--)
                    {
                        moveCard(deckCards, playersToSetUp.cardsInDeck, "random", "");
                    }
                    playersToSetUp.field.piles = new List<fieldPile> { };
                    for (int i = 1; i < 6; i++)
                    {
                        fieldPile pile = new fieldPile();
                        pile.pileCards = new List<string> { };
                        for (int j = i; j > 0; j--)
                        {
                            moveCard(playersToSetUp.cardsInDeck, pile.pileCards, "first", "");
                            updateVisual($"{playersToSetUp.Name}Play");
                        }
                        pile.nameOfLinkedPile = $"{playersToSetUp.Name}FieldPile{i}";
                        playersToSetUp.field.piles.Add(pile);
                    }
                    playersToSetUp.field.playerSelectedFieldPile = 0;
                    UpdateFieldCards("selectedFieldChangedP2");
                    UpdateFieldCards("selectedFieldChangedP1");
                    playersToSetUp.playPile.playerSelectedPlayPile = 0;
                    UpdateFieldCards("selectedPlayChanged");
                    playersToSetUp.playPile.nameOfLinkedPlayPile = $"{playersToSetUp.Name}PlayPile";
                    playersToSetUp.field.fieldEmpty = false;
                    playersToSetUp.playPile.playerPlayPile = new List<String> { };
                    //move to round start moveCard(playersToSetUp.cardsInDeck, playersToSetUp.playPile.playerPlayPile, "first","");
                }
                Grid.FocusVisualSecondaryBrushProperty.Equals("#FF297837");
                updateVisualPilesAll();
                playerFieldPile1.Margin = baseMarginSetupCards;
                playerFieldPile2.Margin = baseMarginSetupCards;
                playerFieldPile3.Margin = baseMarginSetupCards;
                playerFieldPile4.Margin = baseMarginSetupCards;
                playerFieldPile5.Margin = baseMarginSetupCards;
                compFieldPile1.Margin = baseMarginSetupCards;
                compFieldPile2.Margin = baseMarginSetupCards;
                compFieldPile3.Margin = baseMarginSetupCards;
                compFieldPile4.Margin = baseMarginSetupCards;
                compFieldPile5.Margin = baseMarginSetupCards;
                gameStatus = "set Up";
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Round running can't reset");
            }
        }
        public void NewRound()
        {
            //from roundEnd
            if (gameStatus == "round end")
            {
                foreach (var playersToSetUp in players)
                {
                    //reset players field
                    if (playersToSetUp.cardsInDeck.Count >= 15)
                    {
                        for (int i = 0; i <= 4; i++)
                        {
                            for (int j = i + 1; j > 0; j--)
                            {
                                moveCard(playersToSetUp.cardsInDeck, playersToSetUp.field.piles[i].pileCards, "first", "");
                            }
                        }

                    }
                    else
                    {
                        int pileToAdd = 0;
                        while (playersToSetUp.cardsInDeck.Count != 0)
                        {
                            if (playersToSetUp.field.piles[pileToAdd].pileCards.Count != pileToAdd + 1)
                            {
                                playersToSetUp.field.piles[pileToAdd].pileCards.Add(playersToSetUp.cardsInDeck[0]);
                                playersToSetUp.cardsInDeck.RemoveAt(0);
                            }
                            if (pileToAdd == 4)
                            {
                                pileToAdd = -1;
                            }
                            pileToAdd++;
                        }
                    }
                    playersToSetUp.field.playerSelectedFieldPile = 0;
                    UpdateFieldCards("selectedFieldChangedP2");
                    UpdateFieldCards("selectedFieldChangedP1");
                    playersToSetUp.playPile.playerSelectedPlayPile = 1;
                    UpdateFieldCards("selectedPlayChanged");
                    playersToSetUp.field.fieldEmpty = false;
                    playerHandPileNum.Text = Convert.ToString(playersToSetUp.cardsInDeck.Count);
                }
                updateVisualPilesAll();
                playerFieldPile1.Margin = baseMarginSetupCards;
                playerFieldPile2.Margin = baseMarginSetupCards;
                playerFieldPile3.Margin = baseMarginSetupCards;
                playerFieldPile4.Margin = baseMarginSetupCards;
                playerFieldPile5.Margin = baseMarginSetupCards;
                compFieldPile1.Margin = baseMarginSetupCards;
                compFieldPile2.Margin = baseMarginSetupCards;
                compFieldPile3.Margin = baseMarginSetupCards;
                compFieldPile4.Margin = baseMarginSetupCards;
                compFieldPile5.Margin = baseMarginSetupCards;
                gameStatus = "set Up";
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Round running can't reset");
            }
        }
        public void roundEnd(String wonRound)
        {
            if (gameStatus == "round Ended")/////////////////////////////////////////////////////////////////////////////////////
            {
                if (wonRound == players[0].Name)
                {
                    foreach (var cardInPlayPile in players[Convert.ToInt32(1 - players[0].playPile.playerSelectedPlayPile)].playPile.playerPlayPile)
                    {
                        players[0].cardsInDeck.Add(cardInPlayPile);
                    }
                    foreach (var cardInPlayPileC in players[players[0].playPile.playerSelectedPlayPile].playPile.playerPlayPile)
                    {
                        players[1].cardsInDeck.Add(cardInPlayPileC);
                    }
                    //clear piles on field
                    foreach (var playersToClear in players)
                    {
                        playersToClear.playPile.playerPlayPile.Clear();
                    }
                }
                else
                {
                    // test
                    System.Diagnostics.Debug.WriteLine("player didn't win");
                    //add chance to chose wrong pile for lower difficultiesS
                }
                // add cards not played in field
                foreach (var refillDecks in players)
                {
                    foreach (var fields in refillDecks.field.piles)
                    {
                        foreach (var pilesOnField in fields.pileCards)
                        {
                            refillDecks.cardsInDeck.Add(pilesOnField);
                        }
                        fields.pileCards.Clear();
                    }
                    //shuffle(refillDecks.cardsInDeck);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("game still going");
            }
            gameStatus = "round end";
        }
        public void flipPP()
        {
            if (players[0].cardsInDeck.Count != 0 || players[1].cardsInDeck.Count != 0)
            {
                if (players[0].cardsInDeck.Count == 0)
                {
                    moveCard(players[1].cardsInDeck, players[1].playPile.playerPlayPile, "first", "");
                    compHandPileNum.Text = Convert.ToString(players[1].cardsInDeck.Count);
                }
                else if (players[1].cardsInDeck.Count == 0)
                {
                    moveCard(players[0].cardsInDeck, players[0].playPile.playerPlayPile, "first", "");
                    playerHandPileNum.Text = Convert.ToString(players[0].cardsInDeck.Count);
                }
                else
                {
                    moveCard(players[1].cardsInDeck, players[1].playPile.playerPlayPile, "first", "");
                    moveCard(players[0].cardsInDeck, players[0].playPile.playerPlayPile, "first", "");
                    playerHandPileNum.Text = Convert.ToString(players[0].cardsInDeck.Count);
                    compHandPileNum.Text = Convert.ToString(players[1].cardsInDeck.Count);
                }
            }
            else
            {
                //resets game as tie 
                gameStatus = "not Running";
                setUpGame();
            }
            updateVisual($"{players[0].Name}Play");
            updateVisual($"{players[1].Name}Play");
            //System.Diagnostics.Debug.WriteLine(players[0].cardsInDeck.Count + ":Pd " + players[1].cardsInDeck.Count + ":Cd ");
            if (false)//visual representation of pile having no cards
            { }
        }
        public void updateVisual(String elementToUpdate)
        {
            switch (elementToUpdate)
            {
                case ("playerField"):
                    foreach (var set in players[0].field.piles)
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
                    break;
                case ("playerPlay"):
                    if (players[0].playPile != null)
                    {
                        if (players[0].playPile.playerPlayPile != null && players[0].playPile.playerPlayPile.Count != 0)
                        {
                            playerPlayPile.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{players[0].playPile.playerPlayPile[players[0].playPile.playerPlayPile.Count - 1]}.png"));
                        }
                    }
                    break;
                case ("compField"):
                    foreach (var set in players[1].field.piles)
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
                            case ("compFieldPile1"):
                                compFieldPile1.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                                break;
                            case ("compFieldPile2"):
                                compFieldPile2.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                                break;
                            case ("compFieldPile3"):
                                compFieldPile3.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                                break;
                            case ("compFieldPile4"):
                                compFieldPile4.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                                break;
                            case ("compFieldPile5"):
                                compFieldPile5.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                                break;
                        }
                    }
                    break;
                case ("compPlay"):
                    if (players[1].playPile != null)
                    {
                        if (players[1].playPile.playerPlayPile != null && players[1].playPile.playerPlayPile.Count != 0)
                        {
                            compPlayPile.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{players[1].playPile.playerPlayPile[players[1].playPile.playerPlayPile.Count - 1]}.png"));
                        }
                    }
                    break;
                default:
                    updateVisualPilesAll();
                    break;
            }

        }
        public void updateVisualPilesAll()
        {
            foreach (var playersToUpdate in players)
            {
                //field piles
                if (playersToUpdate.field != null)
                {
                    if (playersToUpdate.field.piles != null)
                    {
                        foreach (var set in playersToUpdate.field.piles)
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
                                case ("compFieldPile1"):
                                    compFieldPile1.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                                    break;
                                case ("compFieldPile2"):
                                    compFieldPile2.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                                    break;
                                case ("compFieldPile3"):
                                    compFieldPile3.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                                    break;
                                case ("compFieldPile4"):
                                    compFieldPile4.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                                    break;
                                case ("compFieldPile5"):
                                    compFieldPile5.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{set.topCard}.png"));
                                    break;
                            }
                        }
                    }
                }
                //play piles
                if (playersToUpdate.playPile != null)
                {
                    if (playersToUpdate.playPile.playerPlayPile != null)
                    {
                        if (playersToUpdate.playPile.playerPlayPile.Count != 0)
                        {
                            switch (playersToUpdate.playPile.nameOfLinkedPlayPile)
                            {
                                //add special case for round win event
                                case ("compPlayPile"):
                                    compPlayPile.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{playersToUpdate.playPile.playerPlayPile[playersToUpdate.playPile.playerPlayPile.Count - 1]}.png"));
                                    PlayerS2.Margin = new Thickness(70 * Convert.ToInt32(((Frame)Window.Current.Content).ActualHeight) / 1000, Convert.ToInt32(((Frame)Window.Current.Content).ActualWidth) / 1500, 0, 0);
                                    break;
                                case ("playerPlayPile"):
                                    playerPlayPile.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{playersToUpdate.playPile.playerPlayPile[playersToUpdate.playPile.playerPlayPile.Count - 1]}.png"));
                                    PlayerS1.Margin = new Thickness(70 * Convert.ToInt32(((Frame)Window.Current.Content).ActualHeight) / 1000, Convert.ToInt32(((Frame)Window.Current.Content).ActualWidth) / 1500, 0, 0);
                                    break;
                            }
                        }
                        else
                        {
                            compPlayPile.Source = new BitmapImage(new Uri($"ms-appx:///Assets/crdB1.jpg"));
                            playerPlayPile.Source = new BitmapImage(new Uri($"ms-appx:///Assets/crdB0.jpg"));
                        }
                    }
                }
            }
        }
        public void UpdateFieldCards(String sendingOpperation)
        {
            //update selected
            if (sendingOpperation == "selectedFieldChangedP1")
            {
                // don't know a way to convert string to Windows.UI.Xaml.Controls.Image/ dynamic calling of images
                if (players[0].field != null)
                {
                    switch (players[0].field.playerSelectedFieldPile)
                    {
                        case (0):
                            playerFieldPile1.Margin = new Thickness(0);
                            playerFieldPile2.Margin = baseMarginSetupCards;
                            break;
                        case (1):
                            playerFieldPile1.Margin = baseMarginSetupCards;
                            playerFieldPile2.Margin = new Thickness(0);
                            playerFieldPile3.Margin = baseMarginSetupCards;
                            break;
                        case (2):
                            playerFieldPile3.Margin = new Thickness(0);
                            playerFieldPile2.Margin = baseMarginSetupCards;
                            playerFieldPile4.Margin = baseMarginSetupCards;
                            break;
                        case (3):   
                            playerFieldPile4.Margin = new Thickness(0);
                            playerFieldPile3.Margin = baseMarginSetupCards;
                            playerFieldPile5.Margin = baseMarginSetupCards;
                            break;
                        case (4):
                            playerFieldPile5.Margin = new Thickness(0);
                            playerFieldPile4.Margin = baseMarginSetupCards;
                            break;
                    }

                }

                //MainPage.((stack.nameOfLinkedPile).To).Margin = new Thickness(0, 0, 0, 0);
            }
            else if (sendingOpperation == "selectedFieldChangedP2")
            {
                    // don't know a way to convert string to Windows.UI.Xaml.Controls.Image/ dynamic calling of images
                    if (players[1].field != null)
                    {
                    switch (players[1].field.playerSelectedFieldPile)
                    {
                        case (0):
                            compFieldPile1.Margin = new Thickness(0);
                            compFieldPile2.Margin = baseMarginSetupCards;
                            break;
                        case (1):
                            compFieldPile1.Margin = baseMarginSetupCards;
                            compFieldPile2.Margin = new Thickness(0);
                            compFieldPile3.Margin = baseMarginSetupCards;
                            break;
                        case (2):
                            compFieldPile3.Margin = new Thickness(0);
                            compFieldPile2.Margin = baseMarginSetupCards;
                            compFieldPile4.Margin = baseMarginSetupCards;
                            break;
                        case (3):
                            compFieldPile4.Margin = new Thickness(0);
                            compFieldPile3.Margin = baseMarginSetupCards;
                            compFieldPile5.Margin = baseMarginSetupCards;
                            break;
                        case (4):
                            compFieldPile5.Margin = new Thickness(0);
                            compFieldPile4.Margin = baseMarginSetupCards;
                            break;
                    }

                    //MainPage.((stack.nameOfLinkedPile).To).Margin = new Thickness(0, 0, 0, 0);
                }
            }
            if (sendingOpperation == "selectedPlayChanged")
            {
                switch (players[0].playPile.playerSelectedPlayPile)
                {
                    case (0):
                        Grid.SetColumn(PlayerS1, 4);
                        PlayerS1.Margin = new Thickness(70*Convert.ToInt32(((Frame)Window.Current.Content).ActualHeight)/1000, Convert.ToInt32(((Frame)Window.Current.Content).ActualWidth) / 1500, 0,0);
                        break;
                    case (1):
                        Grid.SetColumn(PlayerS1, 7);
                        PlayerS1.Margin = new Thickness(70 * Convert.ToInt32(((Frame)Window.Current.Content).ActualHeight) / 1000, Convert.ToInt32(((Frame)Window.Current.Content).ActualWidth) / 1500, 0, 0);
                        break;
                }
                if (players[1].playPile != null)
                {
                    switch (players[1].playPile.playerSelectedPlayPile)
                    {
                        case (0):
                            Grid.SetColumn(PlayerS2, 5);
                            PlayerS2.Margin = new Thickness(0,Convert.ToInt32(((Frame)Window.Current.Content).ActualWidth) / 1500, 70 * Convert.ToInt32(((Frame)Window.Current.Content).ActualHeight) / 1000, 0);
                            break;
                        case (1):
                            Grid.SetColumn(PlayerS2, 8);
                            PlayerS2.Margin = new Thickness(0, Convert.ToInt32(((Frame)Window.Current.Content).ActualWidth) / 1500, 70 * Convert.ToInt32(((Frame)Window.Current.Content).ActualHeight) / 1000, 0);
                            break;
                    }
                }
            }

            //update card placed

            //update top card
        }
        public gamePage()
        {
            this.InitializeComponent();


            PlayerS1.Margin = new Thickness(70 * Convert.ToInt32(((Frame)Window.Current.Content).ActualHeight) / 1000, Convert.ToInt32(((Frame)Window.Current.Content).ActualWidth) / 1500, 0, 0);
            PlayerS2.Margin = new Thickness(0, Convert.ToInt32(((Frame)Window.Current.Content).ActualWidth) / 1500, 70 * Convert.ToInt32(((Frame)Window.Current.Content).ActualHeight) / 1000, 0);
        }
        private void KeyPressed(object sender, KeyRoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(e.Key);
            //System.Diagnostics.Debug.WriteLine(sender);
            if (gameStatus != "not Running")
            {
                if (Convert.ToString(e.Key) == "A")
                {
                    if (players[0].field.playerSelectedFieldPile != 0)
                    {
                        players[0].field.playerSelectedFieldPile--;
                        UpdateFieldCards("selectedFieldChangedP1");
                    }
                }
                else if (Convert.ToString(e.Key) == "Left")//djfkdsjfdsjfjdsklfjsdklfjsdklfjsdkljfklsdjfkls
                {
                    if (players[1].field.playerSelectedFieldPile != 0)
                    {
                        players[1].field.playerSelectedFieldPile--;
                        UpdateFieldCards("selectedFieldChangedP2");
                    }
                }
                else if (Convert.ToString(e.Key) == "Right")
                {
                    if (players[1].field.playerSelectedFieldPile != 4)
                    {
                        players[1].field.playerSelectedFieldPile++;
                        UpdateFieldCards("selectedFieldChangedP2");
                    }
                }
                else if (Convert.ToString(e.Key) == "D" )
                {
                    if (players[0].field.playerSelectedFieldPile != 4)
                    {
                        players[0].field.playerSelectedFieldPile++;
                        UpdateFieldCards("selectedFieldChangedP1");
                    }
                }
                else if (Convert.ToString(e.Key) == "W")
                {
                    if (players[0].playPile.playerSelectedPlayPile != 1)
                    {
                        players[0].playPile.playerSelectedPlayPile++;
                        UpdateFieldCards("selectedPlayChanged");
                    }
                }
                else if (Convert.ToString(e.Key) == "Up")//////////////////////////////////////////////
                {
                    if (players[1].playPile.playerSelectedPlayPile != 1)
                    {
                        players[1].playPile.playerSelectedPlayPile++;
                        UpdateFieldCards("selectedPlayChanged");
                    }
                }
                else if (e.Key == Windows.System.VirtualKey.Down)
                {
                    if (players[1].playPile.playerSelectedPlayPile != 0)
                    {
                        players[1].playPile.playerSelectedPlayPile--;
                        UpdateFieldCards("selectedPlayChanged");
                    }
                }
                else if (e.Key == Windows.System.VirtualKey.S)
                {
                    if (players[0].playPile.playerSelectedPlayPile != 0)
                    {
                        players[0].playPile.playerSelectedPlayPile--;
                        UpdateFieldCards("selectedPlayChanged");
                    }
                }
                else if (e.Key == Windows.System.VirtualKey.N)
                {
                    if (gameStatus != "set Up")
                    {
                        flipPP();
                    }
                }
            }
        }
        private void spacePressed(object sender, RoutedEventArgs e)
        {
            if (gameStatus == "not Running")
            {
                setUpGame();
            }
            //activates game
            else if (gameStatus == "set Up")
            {
                gameStatus = "round Start";
                flipPP();
            }
            //comparing system
            else if (gameStatus == "round Start")
            {
                if (checkSlap() == null)
                {
                    if (players[0].field.piles[players[0].field.playerSelectedFieldPile].pileCards.Count != 0 && players[Convert.ToInt32(1 - players[0].playPile.playerSelectedPlayPile)].playPile.playerPlayPile.Count != 0)
                    {
                        players[0].field.piles[players[0].field.playerSelectedFieldPile].topCard = players[0].field.piles[players[0].field.playerSelectedFieldPile].pileCards[0];
                        var topCardFieldValue = Convert.ToInt32((players[0].field.piles[players[0].field.playerSelectedFieldPile].topCard).Substring((players[0].field.piles[players[0].field.playerSelectedFieldPile].topCard).Length - 2));
                        //     
                        var topCardPlayValue = Convert.ToInt32((players[Convert.ToInt32(1 - players[0].playPile.playerSelectedPlayPile)].playPile.playerPlayPile[(players[Convert.ToInt32(1 - players[0].playPile.playerSelectedPlayPile)].playPile.playerPlayPile.Count - 1)]).Substring((players[Convert.ToInt32(1 - players[0].playPile.playerSelectedPlayPile)].playPile.playerPlayPile[players[Convert.ToInt32(1 - players[0].playPile.playerSelectedPlayPile)].playPile.playerPlayPile.Count - 1].Length - 2)));
                        if ((topCardPlayValue == topCardFieldValue + 1 || topCardPlayValue == topCardFieldValue - 1) || (topCardPlayValue == 1 && topCardFieldValue == 13) || (topCardPlayValue == 13 && topCardFieldValue == 1))
                        {
                            moveCard(players[0].field.piles[players[0].field.playerSelectedFieldPile].pileCards, players[Convert.ToInt32(1 - players[0].playPile.playerSelectedPlayPile)].playPile.playerPlayPile, "first", "");
                            updateVisual($"{players[0].Name}Field");
                            updateVisual($"{players[0].Name}Play");
                            updateVisual($"{players[1].Name}Play");
                        }
                        else
                        {
                            //System.Diagnostics.Debug.WriteLine("no match");
                        }
                        playerHandPileNum.Text = Convert.ToString(players[0].cardsInDeck.Count);
                        compHandPileNum.Text = Convert.ToString(players[1].cardsInDeck.Count);
                    }
                    if (players[1].field.piles[players[1].field.playerSelectedFieldPile].pileCards.Count != 0 && players[Convert.ToInt32(1 - players[1].playPile.playerSelectedPlayPile)].playPile.playerPlayPile.Count != 0)
                    {
                        players[1].field.piles[players[1].field.playerSelectedFieldPile].topCard = players[1].field.piles[players[1].field.playerSelectedFieldPile].pileCards[0];
                        var topCardFieldValue = Convert.ToInt32((players[1].field.piles[players[1].field.playerSelectedFieldPile].topCard).Substring((players[1].field.piles[players[1].field.playerSelectedFieldPile].topCard).Length - 2));
                        //                                             takes pile selected                                                    from pile choses last element (as it is the one shown)                                                                                            takes length of element shown and -2 to get a shorter string of number 0? || ??
                        var topCardPlayValue = Convert.ToInt32((players[Convert.ToInt32(1 - players[1].playPile.playerSelectedPlayPile)].playPile.playerPlayPile[(players[Convert.ToInt32(1 - players[1].playPile.playerSelectedPlayPile)].playPile.playerPlayPile.Count - 1)]).Substring((players[Convert.ToInt32(1 - players[1].playPile.playerSelectedPlayPile)].playPile.playerPlayPile[players[Convert.ToInt32(1 - players[1].playPile.playerSelectedPlayPile)].playPile.playerPlayPile.Count - 1].Length - 2)));
                        if ((topCardPlayValue == topCardFieldValue + 1 || topCardPlayValue == topCardFieldValue - 1) || (topCardPlayValue == 1 && topCardFieldValue == 13) || (topCardPlayValue == 13 && topCardFieldValue == 1))
                        {
                            moveCard(players[1].field.piles[players[1].field.playerSelectedFieldPile].pileCards, players[Convert.ToInt32(1 - players[1].playPile.playerSelectedPlayPile)].playPile.playerPlayPile, "first", "");
                            updateVisual($"{players[1].Name}Field");
                            updateVisual($"{players[1].Name}Play");
                            updateVisual($"{players[0].Name}Play");
                        }
                        else
                        {
                            //System.Diagnostics.Debug.WriteLine("no match");
                        }

                        playerHandPileNum.Text = Convert.ToString(players[0].cardsInDeck.Count);
                        compHandPileNum.Text = Convert.ToString(players[1].cardsInDeck.Count);
                    }
                }
                else
                {
                    //snap 
                    gameStatus = "round Ended";
                    if (checkSlap() == players[0].Name)
                    {
                        //player has no cards in hand   
                        roundEnd(players[0].Name);// cards in deck = 0
                        if (checkWin())
                        {
                            //end game
                            winMsg();
                            this.Frame.Navigate(typeof(MainPage), null);
                        }
                        else
                        {
                            NewRound();
                        }
                    }
                    else//comp
                    {

                    }
                }
            }
        }
    }
}
