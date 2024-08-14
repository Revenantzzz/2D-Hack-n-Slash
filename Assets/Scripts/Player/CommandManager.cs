using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RPG2D
{
    public class CommandManager : MonoBehaviour
    {
        public PlayerController controller;
        public List<List<ICommand>> commandList = new List<List<ICommand>>();
    }
    public class CommandInvoker 
    {
        List<ICommand> currentCommandList;
        public void ExcuteCommand(ICommand command)
        {
            currentCommandList.Add(command);
            command.Execute();
        }
    }
}
