# Quick Enum Strings

This library was created due to a call to action in Nick Chapsas' ([@Elfocrash](https://github.com/Elfocrash)) [video on enum performance](https://www.youtube.com/watch?v=BoE5Y6Xkm6w).

As detailed in the video, this library uses a [source generator](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) to generate an extension method that will convert any non-flag enum into a string with zero memory allocation. Mark your enum declaration with `[QuickEnum]` to generate the method.
