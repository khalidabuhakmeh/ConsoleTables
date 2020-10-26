# ConsoleTable

Have you ever just wanted to output flat structured POCO out to console? Sure you have! This class will let you print a nicely formatted table right to your console as easily as possible.

## Example usage
```csharp
// using ConsoleTables;
static void Main(String[] args)
{
    var table = new ConsoleTable("one", "two", "three");
    table.AddRow(1, 2, 3)
         .AddRow("this line should be longer", "yes it is", "oh");

    table.Write();
    Console.WriteLine();

    var rows = Enumerable.Repeat(new Something(), 10);

    ConsoleTable
        .From<Something>(rows)
        .Configure(o => o.NumberAlignment = Alignment.Right)
        .Write(Format.Alternative);

    Console.ReadKey();
}
```

## Console output
```bat

FORMAT: Default:

 --------------------------------------------------
 | one                        | two       | three |
 --------------------------------------------------
 | 1                          | 2         | 3     |
 --------------------------------------------------
 | this line should be longer | yes it is | oh    |
 --------------------------------------------------

 Count: 2


FORMAT: Alternative:

+----------------------------+-----------+-------+
| one                        | two       | three |
+----------------------------+-----------+-------+
| 1                          | 2         | 3     |
+----------------------------+-----------+-------+
| this line should be longer | yes it is | oh    |
+----------------------------+-----------+-------+

FORMAT: Minimal:

one                         two        three
--------------------------------------------
1                           2          3
this line should be longer  yes it is  oh
```

## Sample Output (Screenshot)

![screenshot](/screenshot.PNG)

## Adding it to your project with nuget

**Package Manager**

```sh
Install-Package ConsoleTables -Version 2.4.2
```

**.NET CLI**

```sh
dotnet add package ConsoleTables --version 2.4.2
```
**PackageReference**

```sh
<PackageReference Include="ConsoleTables" Version="2.4.2" />
```
**Packet CLI**

```sh
paket add ConsoleTables --version 2.4.2
```

## Version History

| Version       | Downloads    | Last updated  |
| ------------- |--------------|---------------|
| Version       |Downloads	   |Last updated   |
| 2.4.2 	    |18,202 	   |4 months ago   |
| 2.4.1 	    |42,442 	   |7 months ago   |
| 2.4.0 	    |19,443 	   |12/18/2019     |
| 2.3.0 	    |107,954 	   |3/20/2019 	   |
| 2.2.4 	    |678           |3/6/2019       |

The MIT License (MIT)

Copyright (c) 2013 Khalid Abuhakmeh

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
