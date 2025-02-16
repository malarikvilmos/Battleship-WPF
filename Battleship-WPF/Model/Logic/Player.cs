namespace Battleship_WPF.Logic
{
    public class Player
    {
        public int Identifier { get; set; }
        public int Win { get; set; }
        public int Lose { get; set; }
        public bool isReady { get; set; } = false;
        public bool wantRematch { get; set; } = false;
        public Board Board { get; set; }

        public Player()
        {
            Board = new Board();
        }

        public void Reset()
        {
            isReady = false;
            wantRematch = false;
            Board = new Board();
        }
    }

}
