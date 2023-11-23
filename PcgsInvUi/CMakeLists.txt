cmake_minimum_required(VERSION 3.25)

project(PcgsInvUi LANGUAGES CSharp)

include(CSharpUtilities)

add_executable(PcgsInvUi
  App.axaml.cs
  Program.cs
  ViewLocator.cs
  App.axaml
  app.manifest
  Models/Coin.cs
  Services/CoinCollection.cs
  Services/CoinDatabase.cs
  Services/PcgsClient.cs
  ViewModels/CoinListViewModel.cs
  ViewModels/DeleteWindowViewModel.cs
  ViewModels/EditViewModel.cs
  ViewModels/ErrorWindowViewModel.cs
  ViewModels/FindWindowViewModel.cs
  ViewModels/MainWindowViewModel.cs
  ViewModels/NewViewModel.cs
  ViewModels/ViewModelBase.cs
  Views/DeleteWindow.axaml
  Views/DeleteWindow.axaml.cs
  Views/ErrorWindow.axaml
  Views/ErrorWindow.axaml.cs
  Views/FindWindow.axaml
  Views/FindWindow.axaml.cs
  Views/MainWindow.axaml
  Views/MainWindow.axaml.cs
  Views/NewView.axaml
  Views/NewView.axaml.cs
)

# Set the .NET Framework version for the executable.
set_property(TARGET PcgsInvUi PROPERTY VS_DOTNET_TARGET_FRAMEWORK_VERSION "v4.7.2")
# Set the executable to be 64-bit.
set_property(TARGET PcgsInvUi PROPERTY WIN64_EXECUTABLE TRUE)
# Set the C# language version (defaults to 3.0)
set(CMAKE_CSharp_FLAGS "/langversion:latest")

# Add the .NET reference libraries as dependencies.
set_property(TARGET PcgsInvUi PROPERTY VS_DOTNET_REFERENCES
  "Microsoft.CSharp"
)
