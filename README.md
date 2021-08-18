# Procedural object placement


| | [Pach1](Pach1/Readme.md) | [Pach2](Pach2/Readme.md) | Pach3 | Pach4 |
|-|-------|-------|-------|--------|
| | <img src="Pach1/example.svg" width="100" height="100"/> | <img src="Pach2/example.svg" width="100" height="100"/> | <img src="Pach3/example.svg" width="100" height="100"/> | <img src="Pach4/example.svg" width="100" height="100"/> |
| Min [Square Distance](SquareDistance.md)| :x: n/2 | :white_check_mark: n | :white_check_mark: n | :white_check_mark: n | 
| Min Euclidean Distance | :x: n/sqrt(2) | :white_check_mark: n | :white_check_mark: n | :white_check_mark: n|
| [Initialization Benchmark](InitBenchmark.md) | :white_check_mark: 4.7 ns | :white_check_mark: 4.6 ns | :white_check_mark: 4.7 ns | :x: 6 ms O(n*n) |
| [1M samples benchmark](1MBenchmark.md) | :white_check_mark: 29.822 ms | :x: 96.02 ms | :x: 61 ms | :white_check_mark: 9.9 ms |
| [Memory use](memory.md) | :white_check_mark: 8 B | :white_check_mark: 8 B | :white_check_mark: 8 B | :x: 786,432 B O(n*n) |
| [Tiled](Tiles.md) | :white_check_mark: No | :white_check_mark: No | :white_check_mark: No | :x: Yes |
| [Square Coverage](Coverage.md) | :white_check_mark: 100%| :white_check_mark: 92%| :x: 50% | :white_check_mark: 100%|
| [Randomness](Randomness.md) | :x: C | :white_check_mark: A| :x: B | :white_check_mark: A-|
| [Repeatable](Repeatable.md) | :white_check_mark: Yes | :white_check_mark: Yes | :white_check_mark: Yes | :white_check_mark: Yes |

Here I present 4 algorithms I invented for placing square objects on a plane such that they don't touch each other.  The algorithms are named after me, and all have different pros and cons, which I present in the table above.

I am making a game with procedural world generation.
In this game, I want to generate villages.
In each village, I want to place houses, fountains, wells, churches, etc...

I can just place random structures in random places in the village,  however this looks terrible because I my put two houses too close to each other and they might overlap.

I can place them on a grid making sure that each structure stays in it's assigned cell. But this is not interesting enough for players. The objects should be placed in seemingly random places.

I have not come up with the absolute perfect solution yet although these ones are already good enough for me. 

## Run it:

Pach1
```sh
dotnet run -- --algorithm=1
```

Pach2
```sh
dotnet run -- --algorithm=2
```

Pach3
```sh
dotnet run -- --algorithm=3
```

Pach4
```sh
dotnet run -- --algorithm=4
```

These will generate samples in csv format in 30x30 grid with a cell size of 256