using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    public Vector2 Move { get; set; }
    public bool IsInvincible { get; set; } 
    public bool IsBlocking { get; set; }
    public bool IsIncreasingAtk { get; set; }
    public bool IsIncreasingDef { get; set; }   
}
