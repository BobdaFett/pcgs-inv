# pcgs-inv
A coin collection inventory app, written in C# with the Avalonia UI framework utilizing the PCGS Public API.

# Requirements
[.NET Core >= 6](https://dotnet.microsoft.com/en-us/download) - you will be prompted for install before running the program if it is not installed.

# Install
Extract contents of the downloaded file to an accessible folder (ie. Documents/PCGSApp)

As of right now, there's no fancy installer to be used - shortcuts will have to be made by you.

# Usage
Usage is limited as of now, however input, deletion, and export of information is working and should not change much
over the development of the app.

To input a coin, you will need to know the grade and PCGS id (no NGS support, and is not planned yet) of the coin you
wish to input. You can put them into the sidebar to run a request to the PCGS Public API, at which point you will need
to be patient, due to the fact that the API is quite slow.

There is note functionality - with a coin selected, you may use the Notes section on the sidebar in order to input
that information. Multiline notes are not currently supported, but they are planned for the future.

To change the quantity of a specific coin, click on the corresponding value for the displayed coin. This will allow you
to type in a new value for the quantity. The total value of that row and the total value of the collection (displayed
in the lower right corner) will update automatically.

To export into a CSV file, click the Export to CSV button in the lower right corner. This will allow you to choose a
location and name for the file. These files are usable with any application that can open a .csv file, like Excel.
