using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overpopulated
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new Overpopulated())
            {
                game.Parameters.TrackObjects = true;
                game.Run(args);
            }
        }
    }
}
