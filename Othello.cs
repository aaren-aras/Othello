#nullable enable 
using System;
using static System.Console;
using System.Threading;

namespace Bme121
{
    record Player(string Colour, string Symbol, string Name);
    
    static partial class Program
    {
        static void Main() 
        {
            bool gameOver, validMove;
            int cols, rows, playerOneScore, playerTwoScore, turn;
            string userMove;
            
            Welcome();
            Instructions();
            
            // Sets up the players and game
            Player[] players = new Player[] 
            {
                NewPlayer(colour: "white", symbol: "O", name: "White"),
                NewPlayer(colour: "black", symbol: "X", name: "Black"),
            };
            
            turn = GetFirstTurn(players, defaultFirst: 0);
            rows = GetBoardSize(direction: "rows",    defaultSize: 8);
            cols = GetBoardSize(direction: "columns", defaultSize: 8);
            string[,] game = NewBoard(rows, cols);
            
            // Clears the pre-game setup (i.e. names? who moves first? board dimensions?)
            Clear();
            
            // Othello board begins with 2 "X" discs and 2 "O" discs at centre in crisscross formation
            playerOneScore = 2;
            playerTwoScore = 2;
            
            // Plays the game
            DisplayBoard(game);            
            WriteLine();
            DisplayScores(game, players, playerOneScore, playerTwoScore);
            
            gameOver = false;
            while(!gameOver)
            {                
                if(turn == 0) WriteLine("Player Turn: {0} {1}", players[0].Name, "(" + players[0].Symbol + ")");
                else WriteLine("Player Turn: {0} {1}", players[1].Name, "(" + players[1].Symbol + ")");
                
                userMove = GetMove(players[turn]);
                if(userMove == "instructions") 
                {
                    Instructions();
                    DisplayBoard(game);
                    WriteLine();
                }
                else if(userMove == "skip") 
                {
                    turn = (turn + 1) % players.Length;
                    WriteLine("------------------------------------");
                    WriteLine();
                }
                else if(userMove == "end") 
                {
                    gameOver = true;
                    WriteLine();
                }
                else
                {             
                    validMove = TryMove(game, players[turn], userMove);
                    if(validMove) 
                    {
                        // Alternates from 0 to 1 to 0 to 1 to 0 to 1... until game ends
                        turn = (turn + 1) % players.Length;
                        WriteLine();
                    }
                    else
                    {
                        Write("Your choice didn't work! Press <Enter> to try again.");
                        ReadLine();
                        if(!gameOver) WriteLine("------------------------------------");
                        WriteLine();
                        
                    }
                }
                // Updates players' scores by counting number of "O" and "X" discs separately
                playerOneScore = GetScore(game, players[0]);
                playerTwoScore = GetScore(game, players[1]);
                if(!gameOver) DisplayScores(game, players, playerOneScore, playerTwoScore);
            }
            // Shows the final results
            DisplayWinners(game, players, playerOneScore, playerTwoScore);
            WriteLine();
        }
    
        // Welcomes the player before the game begins
        static void Welcome()
        {
            WriteLine();
            WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= ");
            Thread.Sleep(300);
            WriteLine();
            WriteLine(" W e l c o m e   t o   O t h e l l o ! ");
            Thread.Sleep(300);
            WriteLine();
            WriteLine(" V e r s i o n   B y   A a r e n   A.  ");
            Thread.Sleep(300);
            WriteLine(); 
            WriteLine("       N o v . 5 t h , 2 0 2 1         ");
            Thread.Sleep(300);
            WriteLine(); 
            WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- ");
            WriteLine();
            Thread.Sleep(2000);
            
            // This is to avoid sentences from starting and ending on different lines
            WriteLine("*Note: It's recommended that the game is played in FULL SCREEN!");
            Thread.Sleep(3000);
            
            Clear();
        }
        
        static void Instructions()
        {
            WriteLine();
            WriteLine("======================================================================================================================================");
            WriteLine("                                                *-*-* G A M E   R U L E S *-*-*                                                       ");
            WriteLine();
            
            WriteLine("1. After entering their names, the 2 players must decide on who moves first and the dimensions of the Othello board.                  ");
            WriteLine();
            
            WriteLine("2. The player who moves first is \"White\" and uses \"O\" discs, while the second player is \"Black\" and uses \"X\" discs.           ");                                       
            WriteLine();
            
            WriteLine("3. A move consists of choosing a cell to place a disc in by entering the row letter followed by the column letter (e.g. 'ad').        ");     
            WriteLine("   The positioned disc must ALWAYS outflank one or more of the opponent's discs; doing so \"flips\" them over to the flanker's colour.");
            WriteLine("   If the player cannot flank any opposing discs, then they must enter 'skip' to end their turn.                                      ");
            WriteLine();
            
            WriteLine("4. A disc may outflank any number of opposing discs in one or more rows, in any direction (horizontal, vertical, or diagonal).        "); 
            WriteLine();
            
            WriteLine("5. A disc may outflank in any number of directions at the same time (theoretically up to 8 at once).                                  ");
            WriteLine();
            
            WriteLine("6. The game is over once the board is filled entirely with discs or it's impossible for either player to move.                        "); 
            WriteLine("   At this point, they must enter 'end'. The one with the most discs on the board is declared the winner!                             "); 
            WriteLine("   Either player can choose to end the game at any point as well, but the rules for winning remain the same.                          ");
            
            WriteLine("======================================================================================================================================");
            WriteLine(); 
        }
        
        // Collects player name or default to form the player record
        static Player NewPlayer(string colour, string symbol, string name)
        {            
            WriteLine("Enter the name of each player one at a time. The default names are \"White\" and \"Black\"."); 
            Write("Enter Player Name: ");
            string playerName = ReadLine()!;
        
            WriteLine();
            
            Player p = new Player(colour, symbol, playerName);
            return p; 
        }
        
        // Determines which player moves first or default
        static int GetFirstTurn(Player[] players, int defaultFirst)
        {
            bool validResponse;
            int r;
            string response;
            
            validResponse = false;
            while(!validResponse)
            {
                WriteLine("Enter the name of the player moving FIRST. Alternatively, you can randomize it by entering 'random'.");
                Write("Enter Player Name: ");
                response = ReadLine()!;
                
                if(response == players[0].Name) return 0;
                else if(response == players[1].Name) return 1;
                else if(response == "random")
                {
                    Random rGen = new Random();
                    r = rGen.Next(0, 2);
                    
                    if(r == 0) return 0;
                    if(r == 1) return 1;
                }
                else 
                {
                    WriteLine();
                    WriteLine("*Invalid response! Please try again.");
                    WriteLine();
                }
            }
            
            return defaultFirst;
        }
        
        // Gets board size (even and between 4 and 26 inclusive) for one direction
        static int GetBoardSize(string direction, int defaultSize)
        {
            bool validLength;
            int length; 
            
            WriteLine();
            
            validLength = false;
            while(!validLength)
            {
                WriteLine("Enter the row and column dimensions one at a time (first row, then column). Ensure they are both EVEN numbers between 4 and 26 inclusive. The default dimensions are 8 x 8.");
                
                Write("Enter Length: ");
                length = int.Parse(ReadLine()!);
                
                if(4 <= length && length <= 26 && length % 2 == 0) 
                {
                   return length;
                }
                else 
                {
                    WriteLine();
                    WriteLine("*Invalid length! Please try again.");
                    WriteLine();
                }
            }
            
            return defaultSize;
        }
        
        // Gets move from player
        static string GetMove(Player player) 
        {
            WriteLine();    
            WriteLine("Enter a cell. Alternatively, enter 'skip' to skip your turn, 'instructions' to view instructions, or 'end' to end the game.");
            
            Write("Enter move: ");
            string move = ReadLine()!;
            
            return move;
        }
        
        // Tests whether or not the move is valid
        static bool TryMove(string[,] board, Player player, string move)
        {
            bool[] passedDirections;
            int colInput, colLimit, rowInput, rowLimit;
            
            // The player's input (move) must only be 2 letters long (e.g. 'cd', 'de', 'af')
            if(move.Length != 2) return false; 
            
            rowInput = IndexAtLetter(move[0].ToString()); 
            colInput = IndexAtLetter(move[1].ToString());
            
            rowLimit = board.GetLength(0); 
            colLimit = board.GetLength(1);   
            
            // The player's move must be within the board's dimensions 
            if(rowInput >= rowLimit || colInput >= colLimit) return false;
            // A player cannot place a disc on a cell already occupied by another disc
            if(board[rowInput, colInput] != " ") return false;
            
            passedDirections = new bool[8];
            
            // Top -> Bottom: Negative Direction (-row); Bottom -> Top: Positive Direction (+row)
            // Left -> Right: Positive Direction (+col); Right -> Left: Negative Direction (-col)
            passedDirections[0] = TryDirection(board, player, rowInput, -1, colInput,  0); // Up
            passedDirections[1] = TryDirection(board, player, rowInput, -1, colInput,  1); // Up-Right
            passedDirections[2] = TryDirection(board, player, rowInput,  0, colInput,  1); // Right
            passedDirections[3] = TryDirection(board, player, rowInput,  1, colInput,  1); // Down-Right 
            passedDirections[4] = TryDirection(board, player, rowInput,  1, colInput,  0); // Down
            passedDirections[5] = TryDirection(board, player, rowInput,  1, colInput, -1); // Down-Left
            passedDirections[6] = TryDirection(board, player, rowInput,  0, colInput, -1); // Left
            passedDirections[7] = TryDirection(board, player, rowInput, -1, colInput, -1); // Up-Left

            foreach (bool direction in passedDirections)
            {
                if(direction == true) return true;
            }
            
            return false;
        }
        
        // Performs the flips along a direction specified by the row and column delta for one step
        static bool TryDirection(string[,] board, Player player, int moveRow, int deltaRow, int moveCol, int deltaCol)
        {
            bool foundEnd; // Checks for player discs along chains of cells 
            int cellCol, cellRow, flipCount; // flipCounter = number of opponent discs flipped
            
            foundEnd = false;
            // Start at cell entered by player 
            cellRow = moveRow;
            cellCol = moveCol;
            flipCount = 0;
            
            // Loops to see if move is possible
            while(!foundEnd)
            {
                // Moves to nearest cell in the directions specified by 'TryMove' method 
                cellRow += deltaRow;
                cellCol += deltaCol;
                
                // Checks if entered cell is on board or not (i.e. coords cannot be -ve or beyond board)
                if(cellRow < 0) return false;
                if(cellRow > board.GetLength(0) - 1) return false;
                
                if(cellCol < 0) return false;
                if(cellCol > board.GetLength(1) - 1) return false;
                
                // Checks if cell is empty or not  
                if(board[cellRow, cellCol] == " ") return false;
                
                // Checks if cell has an opponent (if) or player (else) disc
                if(board[cellRow, cellCol] != player.Symbol) flipCount++;
                else foundEnd = true; 
            }
            
            if(flipCount == 0) return false;
            
            // Starts back at cell entered by player 
            cellRow = moveRow;
            cellCol = moveCol;
            
            board[moveRow, moveCol] = player.Symbol;
            
            for(int flip = 0; flip < flipCount; flip++)
            {
                // Moves to nearest cell in the directions specified by 'TryMove' method 
                cellRow += deltaRow;
                cellCol += deltaCol; 
                
                // Flips opponent disc 
                board[cellRow, cellCol] = player.Symbol;
                Thread.Sleep(1000);
                WriteLine("------------------------------------");
                WriteLine();
                DisplayBoard(board);
            }
            
            return true;
        }
        
        // Counts all discs on board to determine players' scores 
        static int GetScore(string[,] board, Player player)
        {
            int score = 0;
            
            // Checks every square on board for player discs ('X's and 'O's) 
            // r = rows; c = columns
            for(int r = 0; r < board.GetLength(0); r++)
            {
                for(int c = 0; c < board.GetLength(1); c++)
                {
                    // Incrementing score by 1 for every disc found 
                    if(board[r, c] == player.Symbol) score++;   
                }
            }
        
            return score; 
        }
        
        // Displays line of scores for all players
        static void DisplayScores(string[,] board, Player[] players, int p1Score, int p2Score)
        {
            WriteLine("Scores: " + players[0].Name + " " + p1Score + ", " + players[1].Name + " " + p2Score);
        }
        
        // Displays winner(s) and categorizes their win over the defeated player(s)
        static void DisplayWinners(string[,] board, Player[] players, int p1FinalScore, int p2FinalScore)
        {
            int scoreDifference;
            
            WriteLine("\"`-._,-'\"`-._,-'\"`-._,-'\"`-._,-'_,-'");
            WriteLine();
            WriteLine("Final Scores: " + players[0].Name + " " + p1FinalScore + ", " + players[1].Name + " " + p2FinalScore);
            
            if(p1FinalScore > p2FinalScore) 
            {
                scoreDifference = p1FinalScore - p2FinalScore;
                WriteLine("Congratulations to " + players[0].Name + " for winning by " + scoreDifference + " discs!"); 
            WriteLine();
                WriteLine("\"`-._,-'\"`-._,-'\"`-._,-'\"`-._,-'_,-'");
            }
            else if(p2FinalScore > p1FinalScore) 
            {
                scoreDifference = p2FinalScore - p1FinalScore;
                WriteLine("Congratulations to " + players[1].Name + " for winning by " + scoreDifference + " discs!");
                WriteLine();
                WriteLine("\"`-._,-'\"`-._,-'\"`-._,-'\"`-._,-'_,-'");
            }
            else 
            {
                WriteLine("Congratulations to BOTH players for tying!");
                WriteLine();
                WriteLine("\"`-._,-'\"`-._,-'\"`-._,-'\"`-._,-'_,-'");
            }
            
            /* This could be used to further categorize the win, but was removed due to board length 
                customizability. Ratios between endpoints of ranges could be found, but would get 
                messy due in part to decimals and integer truncation. */
            
            /*
            if(54 <= scoreDifference && scoreDifference <= 64) Write("A PERFECT game!");
            else if(40 <= scoreDifference && scoreDifference <= 52) Write("A WALKAWAY game!");
            else if(26 <= scoreDifference && scoreDifference <= 38) Write("A FIGHT game!");
            else if(12 <= scoreDifference && scoreDifference <= 24) Write("A HOT game!");
            else Write("CLOSE game!");
            */
        }
    }
}



