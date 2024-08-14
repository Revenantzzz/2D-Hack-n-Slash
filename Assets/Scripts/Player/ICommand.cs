using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using UnityEngine;

namespace RPG2D
{
    public interface ICommand 
    {
        void Execute(); 
    }
}
