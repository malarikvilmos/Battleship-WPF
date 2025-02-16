using Battleship_WPF.ViewModel;
using Battleship_WPF.ViewModel.Base;

namespace Battleship_WPF.Commands
{
    public class SelectItemCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            ServerListItemViewModel.SetSelectedServer(parameter);
        }
    }
}
