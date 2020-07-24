using System;
using System.Collections.Generic;
using System.Text;

namespace AdventureRoller.Services
{
    public interface IDiceService
    {
        List<int> RollExactDice(string dice);
    }
}
