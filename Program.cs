
using System;
using TowerOfHanoiSimulation;
Console.WriteLine("Enter the number of Disks:");
int num = Int32.Parse(Console.ReadLine()); ;
using var game = new TowerOfHanoiGame(num);
game.Run();
